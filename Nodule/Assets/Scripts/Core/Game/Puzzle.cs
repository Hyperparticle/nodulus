using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.Core.Moves;

namespace Assets.Scripts.Core.Game
{
    /// <summary>
    /// A Puzzle specifies the rules and methods for interacting 
    /// with a <seealso cref="GameBoard"/>.
    /// </summary>
    public class Puzzle
    {
        private readonly GameBoard _gameBoard;
        private readonly Player _player;

        public Node StartNode { get { return _gameBoard.StartNode; } }
        public bool Win { get { return _player.Win; } }
        public Point BoardSize { get { return _gameBoard.Size; } }


        public IEnumerable<Field> PullFields { get { return _player.PullFields; } }
        public IEnumerable<Field> PushFields { get { return _player.PushFields; } }

        public Arc PulledArc { get; private set; }
        public bool IsPulled { get { return PulledArc != null; } }

        public Puzzle(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;
            _player = new Player(_gameBoard.IslandSet, _gameBoard.StartNode);
        }

        public bool PullArc(Arc arc, Direction pullDir)
        {
            if (IsPulled || arc == null) {
                return false;
            }

            var result = _player.PlayMove(new PullMove(_gameBoard, _player, arc, pullDir));
            PulledArc = result ? arc : PulledArc;
            return result;
        }

        public bool PullArc(Point nodePos, Direction pullDir)
        {
            var arc = _gameBoard.GetArcAt(nodePos, pullDir.Opposite());
            return PullArc(arc, pullDir);
        }

        public bool PushArc(Field field)
        {
            if (!IsPulled || field == null) {
                return false;
            }

            var result = _player.PlayMove(new PushMove(_gameBoard, _player, PulledArc, field));
            PulledArc = result ? null : PulledArc;
            return result;
        }

        public bool PushArc(Point nodePos, Direction pushDir)
        {
            var field = _gameBoard.GetFieldAt(nodePos, pushDir);
            return PushArc(field);
        }
    }
}
