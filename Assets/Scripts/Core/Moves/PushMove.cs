using Core.Data;
using Core.Game;
using Core.Items;

namespace Core.Moves
{
    public class PushMove : IMove
    {
        private readonly GameBoard _gameBoard;
        private readonly Player _player;
        private readonly Arc _arc;
        private readonly Field _field;

        public PushMove(GameBoard gameBoard, Player player, Arc arc, Field field)
        {
            _gameBoard = gameBoard;
            _player = player;
            _arc = arc;
            _field = field;
        }

        public bool IsValid
        {
            // A push move is valid if:
            // 1. The arc is in a pulled state
            // 2. The field is empty (contains no arc)
            // 3. The arc's length is equal to the field's length
            // 4. The arc will not overlap with another arc
            // 5. The field is proximal to the player island

            get
            {
                // TODO: create an enum to show validation status
                if (_player == null || _arc == null || _field == null) return false;
                return _arc.IsPulled && _player.IsProximal(_field) && _field.ValidPlacement(_arc);
            }
        }

        public bool Play()
        {
            if (!IsValid) return false;

            _gameBoard.Push(_arc, _field);
            _player.MoveTo(_field.ParentNode);

            return true;
        }

        public bool Undo()
        {
            // TODO: validation?

            return false;
        }

        //public bool Equals(Arc arc, Field field)
        //{
        //    return _arc != null && Field != null && _arc.Equals(arc) && Field.Equals(field);
        //}
    }
}
