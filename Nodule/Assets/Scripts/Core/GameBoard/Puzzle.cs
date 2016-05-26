using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.Core.Moves;

namespace Assets.Scripts.Core.GameBoard
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

        public bool PullArc(Arc arc, Direction direction)
        {
            if (IsPulled) { return false; }

            var result = _player.PlayMove(new PullMove(_player, arc, direction));
            PulledArc = result ? arc : PulledArc;
            return result;
        }

        public bool PushArc(Field field)
        {
            if (!IsPulled) { return false; }

            var result = _player.PlayMove(new PushMove(_player, PulledArc, field));
            PulledArc = result ? null : PulledArc;
            return result;
        }
 
    }
}
