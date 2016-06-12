namespace Assets.Scripts.Core.Data
{
    /// <summary>
    /// A Point with a direction. Useful for uniquely identifying 
    /// an arc or a field in the game grid.
    /// </summary>
    public struct PointDir
    {
        private readonly Point _point;
        private readonly Direction _direction;

        public PointDir Opposite { get { return new PointDir(_point, _direction.Opposite()); } }

        public PointDir(Point point, Direction direction)
        {
            _point = point;
            _direction = direction;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PointDir && Equals((PointDir)obj);
        }

        public bool Equals(PointDir other)
        {
            return _point.Equals(other._point) && _direction == other._direction;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_point.GetHashCode() * 397) ^ (int)_direction;
            }
        }

        public override string ToString()
        {
            return string.Format("{{{0}, {1}}}", _point, _direction);
        }
    }
}