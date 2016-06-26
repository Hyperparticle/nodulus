using System.Collections.Generic;
using System.Linq;
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
        private readonly List<IMove> _moveHistory = new List<IMove>();

        public Node StartNode { get { return _gameBoard.StartNode; } }
        public bool Win { get { return _player.Win; } }
        public Point BoardSize { get { return _gameBoard.Size; } }

        public PlayerState PlayerState { get { return _player.PlayerState; } }

        public Arc PulledArc { get; private set; }
        public bool IsPulled { get { return PulledArc != null; } }

        public Puzzle(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;
            _player = new Player(gameBoard);
        }

        public bool PullArc(Arc arc, Direction pullDir)
        {
            if (IsPulled || arc == null) {
                return false;
            }

            var move = new PullMove(_gameBoard, _player, arc, pullDir);
            var result = _player.PlayMove(move);
            if (result) {
                _moveHistory.Add(move);
                PlayerState.UpdatePush(arc);
            }

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

            var move = new PushMove(_gameBoard, _player, PulledArc, field);
            var result = _player.PlayMove(move);

            if (result) {
                _moveHistory.Add(move);
                PlayerState.UpdatePush(null);
            }

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
