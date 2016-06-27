using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.View.Control;
using Assets.Scripts.View.Data;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    public class PuzzleState : MonoBehaviour
    {
        private readonly FieldViewMap _fieldMap = new FieldViewMap();
        private readonly ArcViewMap _arcMap = new ArcViewMap();
        private IDictionary<Point, NodeView> _nodeMap;

        private PuzzleSpawner _puzzleSpawner;
        private PuzzleView _puzzleView;
        private BoardInput _boardInput;

        private Puzzle _puzzle;
        private PlayerState _playerState;
        private int _currentLevel;

        public ArcView PulledArcView { get; private set; }
        public bool IsPulled { get { return _puzzle.IsPulled; } }
        public Point PullPosition { get { return _playerState.PullPosition; } }

        // TODO: get nodes, arcs, and fields of player
        //public IEnumerable<ArcView> PullArcs
        //{
        //    get { return _playerState.PullFields.Select(field => _arcMap[field.PointDir]); }
        //}

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

        public bool HasArcAt(Point pos, Direction dir) { return _arcMap.ContainsArc(pos, dir); }

        public IDictionary<Direction, ArcView> GetArcs(Point pos)
        {
            return _arcMap.GetArcs(pos);
        }

        void Awake()
        {
            _puzzleSpawner = GetComponent<PuzzleSpawner>();
            _puzzleView = GetComponent<PuzzleView>();
            _boardInput = GetComponent<BoardInput>();
        }

        void Start()
        {
            // Start with level 0
            Init(0);

            // Init all scripts that require additional information on startup
            _puzzleView.Init(_puzzle.StartNode.Position, _puzzle.BoardSize);
            _boardInput.Init(_puzzleSpawner.NodeMap);
        }

        /// <summary>
        /// Entry point to create a new level
        /// </summary>
        public void Init(int level)
        {
            // Destroy the previous level
            _puzzleSpawner.DestroyBoard();

            // Spawn the new level
            _puzzle = _puzzleSpawner.SpawnBoard(level);
            _playerState = _puzzle.PlayerState;
            _currentLevel = level;

            // Reset the puzzle view state
            _arcMap.Reset(_puzzleSpawner.ArcMap);
            _fieldMap.Reset(_puzzleSpawner.FieldMap);
            _nodeMap = _puzzleSpawner.NodeMap;
        }

        public void NextLevel()
        {
            Init(_currentLevel == Level.LevelCount - 1 ? _currentLevel : _currentLevel + 1);
        }

        public void PrevLevel()
        {
            Init(_currentLevel == 0 ? 0 : _currentLevel - 1);
        }

        public bool Play(NodeView nodeView, Direction dir)
        {
            // Push move
            if (IsPulled) {
                FieldView fieldView;
                return _fieldMap.TryGetField(nodeView.Position, dir, out fieldView) &&
                       PushArc(fieldView);
            }

            // Pull move
            ArcView arcView;
            return _arcMap.TryGetArc(nodeView.Position, dir.Opposite(), out arcView) &&
                   PullArc(arcView, dir);
        }

        public bool PushArc(FieldView fieldView)
        {
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
    }
}
