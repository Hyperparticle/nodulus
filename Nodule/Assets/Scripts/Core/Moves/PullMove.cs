using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;

namespace Assets.Scripts.Core.Moves
{
    public class PullMove : IMove
    {
        private readonly Player _player;
        private readonly Arc _arc;
        private readonly Direction _direction;
        //private readonly Field _field;

        //private bool _played;

        public PullMove(Player player, Arc arc, Direction dir)
        {
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
                if (_player == null || _arc == null) return false;
                return !_arc.IsPulled && _player.IsProximal(_arc.Field);
            }
        }

        public bool Play()
        {
            if (!IsValid) return false;

            _arc.Pull();
            _player.MoveTo(_arc.Field.Root(_direction));
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
