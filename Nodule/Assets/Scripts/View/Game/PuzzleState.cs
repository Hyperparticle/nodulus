using System.Collections.Generic;
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

        private PuzzleSpawner _puzzleSpawner;
        private PuzzleView _puzzleView;
        private BoardInput _boardInput;

        private Puzzle _puzzle;
        private int _currentLevel;

        public ArcView PulledArcView { get; private set; }
        public bool IsPulled { get { return _puzzle.IsPulled; } }

        public bool HasArcAt(Point pos, Direction direction) { return _arcMap.ContainsArc(pos, direction); }

        public ICollection<ArcView> GetArcs(Point pos)
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
            _currentLevel = level;

            // Reset the puzzle view state
            _arcMap.Reset(_puzzleSpawner.ArcMap);
            _fieldMap.Reset(_puzzleSpawner.FieldMap);
        }

        public void NextLevel()
        {
            Init(_currentLevel == Level.LevelCount - 1 ? _currentLevel : _currentLevel + 1);
        }

        public void PrevLevel()
        {
            Init(_currentLevel == 0 ? 0 : _currentLevel - 1);
        }

        public bool Play(NodeView nodeView, Direction direction)
        {
            // Push move
            if (IsPulled) {
                FieldView fieldView;
                return _fieldMap.TryGetField(nodeView.Position, direction, out fieldView) &&
                       PushArc(fieldView);
            }

            // Pull move
            ArcView arcView;
            return _arcMap.TryGetArc(nodeView.Position, direction.Opposite(), out arcView) &&
                   PullArc(arcView, direction);
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

        public bool PullArc(ArcView arcView, Direction direction)
        {
            // If an arc exists, try to play the move
            var movedPlayed = _puzzle.PullArc(arcView.Arc, direction);

            if (!movedPlayed) {
                return false;
            }

            Debug.Log("Pull");

            // If the move was played, update the arc map
            PulledArcView = arcView;
            _arcMap.Remove(PulledArcView.Arc.Position);
            _arcMap.Remove(PulledArcView.Arc.ConnectedPosition);

            return true;
        }
    }
}
