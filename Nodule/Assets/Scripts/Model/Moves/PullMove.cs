using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.Moves
{
    public class PullMove : IMove
    {
        private readonly Player _player;
        private readonly Field _field;

        private bool _played;

        public Arc Arc { get; }

        public PullMove(Player player, Arc arc)
        {
            _player = player;
            Arc = arc;
            _field = arc.Field;
        }

        public bool IsValid
        {
            // A pull move on an arc is valid if:
            // 1. The arc is not in a pulled state
            // 2. Arc's field is proximal to the player island

            get
            {
                if (_player == null || Arc == null) return false;
                return !Arc.IsPulled && _player.IsProximal(Arc.Field);
            }
        }

        public bool Play()
        {
            return Play(Direction.None);
        }

        public bool Play(Direction direction)
        {
            if (!IsValid) return false;
            Arc.Pull();
            _player.MoveTo(Arc.Field.Root(direction));
            _played = true;
            return true;
        }

        public bool Undo()
        {
            // TODO: validation?

            Arc.Push(_field);
            _player.MoveTo(_field.ParentNode);
            _played = false;
            return true;
        }

        //public bool Equals(Arc arc)
        //{
        //    return Arc != null && Arc.Equals(arc);
        //}
    }
}
