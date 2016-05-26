using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.Moves
{
    public class PushMove : IMove
    {
        private readonly Player _player;
        private readonly Arc _arc;

        public Field Field { get; private set; } 

        public PushMove(Player player, Arc arc, Field field)
        {
            _player = player;
            _arc = arc;
            Field = field;
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
                if (_player == null || _arc == null || Field == null) return false;
                return _arc.IsPulled && _player.IsProximal(Field) && Field.ValidPlacement(_arc);
            }
        }

        public bool Play()
        {
            if (!IsValid) return false;

            _arc.Push(Field);
            _player.MoveTo(Field.ParentNode);

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
