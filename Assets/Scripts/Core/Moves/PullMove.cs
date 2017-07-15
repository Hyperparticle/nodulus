using Core.Data;
using Core.Game;
using Core.Items;

namespace Core.Moves
{
    public class PullMove : IMove
    {
        private readonly GameBoard _gameBoard;
        private readonly Player _player;
        private readonly Arc _arc;
        private readonly Direction _direction;
        //private readonly Field _field;

        //private bool _played;

        public PullMove(GameBoard gameBoard, Player player, Arc arc, Direction dir)
        {
            _gameBoard = gameBoard;
            _player = player;
            _arc = arc;
            _direction = dir;
            //_field = arc.Field;
        }

        public bool IsValid
        {
            // A pull move on an arc is valid if:
            // 1. The arc is not in a pulled state
            // 2. The arc's field is proximal to the player island

            get
            {
                // TODO: create an enum to show validation status
                if (_player == null || _arc == null) return false;
                return !_arc.IsPulled && _player.IsProximal(_arc.Field);
            }
        }

        public bool Play()
        {
            if (!IsValid) return false;

            var field = _arc.Field;
            _gameBoard.Pull(_arc);
            _player.MoveTo(field.Root(_direction));
            //_played = true;

            return true;
        }

        public bool Undo()
        {
            // TODO: validation?

            //_arc.Push(_field);
            //_player.MoveTo(_field.ParentNode);
            //_played = false;

            return false;
        }
    }
}
