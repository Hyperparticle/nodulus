using System.Collections.Generic;
using Core.Data;
using Core.Items;
using Core.Moves;

namespace Core.Game
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

        public Node StartNode => _gameBoard.StartNode;
        public bool Win => _player.Win;
        public Point BoardSize => _gameBoard.Size;
        public int NumMoves => _player.NumMoves;

        public PlayerState PlayerState => _player.PlayerState;

        public Arc PulledArc { get; private set; }
        public bool IsPulled => PulledArc != null;

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
