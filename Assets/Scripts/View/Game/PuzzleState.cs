using System.Collections.Generic;
using System.Linq;
using Core.Data;
using Core.Game;
using UnityEngine;
using View.Control;
using View.Data;
using View.Items;

namespace View.Game
{
    public class PuzzleState : MonoBehaviour
    {
        private readonly FieldViewMap _fieldMap = new FieldViewMap();
        private readonly ArcViewMap _arcMap = new ArcViewMap();
        private IDictionary<Point, NodeView> _nodeMap;

        private PuzzleSpawner _puzzleSpawner;
        private PuzzleView _puzzleView;
        private BoardInput _boardInput;
        private BoardAction _boardAction;
        private GameAudio _gameAudio;

        private Puzzle _puzzle;
        private PlayerState _playerState;

        private NodeView _lastNodePulled;

        public ArcView PulledArcView { get; private set; }
        public bool IsPulled => _puzzle.IsPulled;
        public Point PullPosition => _playerState.PullPosition;
        public int NumMoves => _puzzle.NumMoves;
        public float ElapsedTime { get; private set; }

        public int CurrentLevel { get; private set; }
        public List<NodeView> PushNodePath { get; private set; }

        public IEnumerable<NodeView> PlayerNodes
        {
            get { return _playerState.PlayerNodes.Select(node => _nodeMap[node.Position]); }
        }

        public IEnumerable<ArcView> PlayerArcs
        {
            get
            {
                return _playerState.PlayerArcs
                    .Where(arc => !arc.IsPulled)
                    .Select(arc => _arcMap[new PointDir(arc.Position, arc.Direction)]);
            }
        }

        public IEnumerable<FieldView> PushFields
        {
            get
            {
                return _playerState.PushFields
                    .Select(field => _fieldMap[new PointDir(field.Position, field.Direction)]);
            }
        }

        public IEnumerable<NodeView> NonPlayerNodes
        {
            get { return _playerState.NonPlayerNodes.Select(node => _nodeMap[node.Position]); }
        }

        public IEnumerable<ArcView> NonPlayerArcs
        {
            get {
                return _playerState.NonPlayerArcs
                    .Where(arc => !arc.IsPulled)
                    .Select(arc => _arcMap[new PointDir(arc.Position, arc.Direction)]);
            }
        }

        public IEnumerable<FieldView> NonPushFields
        {
            get
            {
                return _playerState.NonPushFields
                    .Select(field => _fieldMap[new PointDir(field.Position, field.Direction)]);
            }
        }

        /// <summary>
        /// Get centroid (average position) of the player island
        /// </summary>
        public Vector2 IslandAverage
        {
            get
            {
                var islandSize = _playerState.PlayerNodes.Count();
                var sumPos = _playerState.PlayerNodes.Select(node => node.Position)
                    .Aggregate(Point.Zero, (a, b) => a + b);

                return (Vector2) sumPos / islandSize;
            }
        }

        public bool Win => _puzzle.Win;

        private bool _boardEnabled;
        public bool BoardEnabled
        {
            get { return _boardEnabled;  }
            set {
                _boardInput.enabled = value;
                _boardAction.enabled = value;
                _gameAudio.enabled = value;
                
                _boardEnabled = value;
            }
        }

        public bool HasArcAt(Point pos, Direction dir) { return _arcMap.ContainsArc(pos, dir); }
        public bool NodeOccupies(Point pos) { return _nodeMap.ContainsKey(pos); }
        public bool ArcOccupies(Point pos) { return false; }

        public IDictionary<Direction, ArcView> GetArcs(Point pos)
        {
            return _arcMap.GetArcs(pos);
        }

        private void Awake()
        {
            _puzzleSpawner = GetComponent<PuzzleSpawner>();
            _puzzleView = GetComponent<PuzzleView>();
            _boardInput = GetComponent<BoardInput>();
            _boardAction = GetComponent<BoardAction>();
            _gameAudio = GameObject.FindGameObjectWithTag("GameAudio").GetComponent<GameAudio>();
        }

        private void Update()
        {
            ElapsedTime += Time.deltaTime;
        }

        /// <summary>
        /// Entry point to create a new level
        /// </summary>
        public void Init(int level, float animationSpeed = 1f, float delayScale = 1f)
        {
            if (level < 0 || level > _puzzleSpawner.LevelCount) {
                Debug.LogWarning("Requested level is outside of bounds, ignoring request");
                return;
            }

            // Destroy the previous level
            _puzzleSpawner.DestroyBoard();

            _saveState.Started = true;
            
            // Spawn the new level
            _puzzle = _puzzleSpawner.SpawnBoard(level, animationSpeed, delayScale);
            _playerState = _puzzle.PlayerState;
            CurrentLevel = level;

            // Reset the puzzle view state
            _arcMap.Reset(_puzzleSpawner.ArcMap);
            _fieldMap.Reset(_puzzleSpawner.FieldMap);
            _nodeMap = _puzzleSpawner.NodeMap;

            // Init all scripts that require additional information on startup
            _boardAction.Init();
            _puzzleView.Init(_puzzle.StartNode.Position, _puzzle.BoardSize);
            _boardInput.Init(_puzzleSpawner.NodeMap);
            
            gameObject.name = $"PuzzleGame ({level})*";
        }

        public void NextLevel(float delay = 0f)
        {
            Init(CurrentLevel == Levels.LevelCount - 1 ? CurrentLevel : CurrentLevel + 1);
        }

        public void PrevLevel(float delay = 0f)
        {
            Init(CurrentLevel == 0 ? 0 : CurrentLevel - 1);
        }
        
        public bool Play(NodeView nodeView, Direction dir)
        {
            // Push move
            if (IsPulled) {
                FieldView fieldView;
                var pushMove = _fieldMap.TryGetField(nodeView.Position, dir, out fieldView) &&
                       PushArc(nodeView, fieldView);

                if (pushMove) {
                    PushNodePath = _playerState.PushPath(_lastNodePulled.Node, nodeView.Node)
                        .Select(node => _nodeMap[node.Position])
                        .ToList();
                }

                return pushMove;
            }

            // Pull move
            ArcView arcView;
            var pullMove = _arcMap.TryGetArc(nodeView.Position, dir.Opposite(), out arcView) &&
                   PullArc(arcView, dir);
            _lastNodePulled = nodeView;
            return pullMove;
        }

        public bool PushArc(NodeView nodeView, FieldView fieldView)
        {
            // Validate that the node is in the island
            if (!_playerState.HasNodeAt(nodeView.Node)) {
                return false;
            }

            // If a field exists, try to play the move
            var movePlayed = _puzzle.PushArc(fieldView.Field);

            if (!movePlayed) {
                return false;
            }

            Debug.Log("Push");

            // If the move was played, update the arc map
            var arc = PulledArcView.Arc;
            _arcMap.Add(arc.Position, arc.Direction, PulledArcView);
            _arcMap.Add(arc.ConnectedPosition, arc.Direction.Opposite(), PulledArcView);

            return true;
        }

        public bool PullArc(ArcView arcView, Direction dir)
        {
            var arcPos = arcView.Arc.Position;
            var arcConnPos = arcView.Arc.ConnectedPosition;
            var arcDir = arcView.Arc.Direction;
            var arcConnDir = arcView.Arc.Direction.Opposite();

            // If an arc exists, try to play the move
            var movedPlayed = _puzzle.PullArc(arcView.Arc, dir);

            if (!movedPlayed) {
                return false;
            }

            Debug.Log("Pull");

            // If the move was played, update the arc map
            PulledArcView = arcView;
            _arcMap.Remove(arcPos, arcDir);
            _arcMap.Remove(arcConnPos, arcConnDir);

            return true;
        }
        
        // TODO: clean this up
        public struct SaveState
        {
            public bool Started;
            public int Level;
            public float AnimationSpeed;
            public float DelayScale;
        }

        private SaveState _saveState;
        public SaveState State => _saveState;
        
        public void Save(int level, float animationSpeed = 1f, float delayScale = 1f)
        {
            _saveState = new SaveState {
                Started = false,
                Level = level,
                AnimationSpeed = animationSpeed,
                DelayScale = delayScale
            };
        }

        public void InitSaved()
        {
            if (_saveState.Started) {
                return;
            }

            Init(_saveState.Level, _saveState.AnimationSpeed, _saveState.DelayScale);
        }

        public void DestroyBoard(bool playSound = true)
        {
            _puzzleSpawner.DestroyBoard(playSound);
            _saveState.Started = false;
        }
    }
}
