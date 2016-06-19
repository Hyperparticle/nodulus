using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.Core.Items;
using Assets.Scripts.Utility;
using Assets.Scripts.View.Control;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    public class PuzzleState : MonoBehaviour
    {
        private readonly MultiMap<Point, Direction, FieldView> _fieldMap = new MultiMap<Point, Direction, FieldView>();
        private readonly MultiMap<Point, Direction, ArcView> _arcMap = new MultiMap<Point, Direction, ArcView>();

        private PuzzleSpawner _puzzleSpawner;
        private PuzzleView _puzzleView;
        private BoardInput _boardInput;

        private Puzzle _puzzle;
        private int _currentLevel;

        private ArcView _pulledArcView;

        // TODO
        //public bool IsPulled { get { return _puzzle.IsPulled; } }

        void Start()
        {
            _puzzleSpawner = GetComponent<PuzzleSpawner>();
            _puzzleView = GetComponent<PuzzleView>();
            _boardInput = GetComponent<BoardInput>();

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
            _arcMap.Reset(_puzzleSpawner.ArcMap); // TODO
            _fieldMap.Reset(_puzzleSpawner.ArcMap);
        }

        public void NextLevel()
        {
            Init(_currentLevel == Level.LevelCount - 1 ? _currentLevel : _currentLevel + 1);
        }

        public void PrevLevel()
        {
            Init(_currentLevel == 0 ? 0 : _currentLevel - 1);
        }

        // TODO
        public bool Play(NodeView nodeView, Direction direction)
        {
            // Play the corresponding move and modify the game view accordingly
            if (_puzzle.IsPulled)
            {
                // Check if there is a field at the position and direction
                var fieldExists = _fieldMap.ContainsKeys(pos, direction);
                if (fieldExists)
                {
                    var fieldView = _fieldMap[pos, direction];
                    return _puzzleView.PushArc(fieldView.Field);
                }

                PushNode(nodeView, direction, movePlayed);
            }
            else
            {
                // Check if there is an arc at the position and direction
                var arcExists = _arcMap.ContainsKeys(pos, direction);
                if (arcExists)
                {
                    var arcView = _arcMap[pos, direction];
                    movePlayed = _puzzleView.PullArc(arcView.Arc, direction);
                }

                PullNode(nodeView, direction, movePlayed);
            }
        }

        // TODO
        public bool PullArc(Arc arc, Direction direction)
        {
            return Puzzle.PullArc(arc, direction);
        }

        public bool PushArc(Field field)
        {
            return Puzzle.PushArc(field);
        }
    }
}
