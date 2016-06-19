namespace Assets.Scripts.Core.Data
{
    /// <summary>
    /// A Point with a direction. Useful for uniquely identifying 
    /// an arc or a field in the game grid.
    /// </summary>
    public struct PointDir
    {
        public readonly Point point;
        public readonly Direction direction;

        public PointDir Opposite { get { return new PointDir(point, direction.Opposite()); } }

        public PointDir(Point point, Direction direction)
        {
            this.point = point;
            this.direction = direction;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PointDir && Equals((PointDir)obj);
        }

        public bool Equals(PointDir other)
        {
            return point.Equals(other.point) && direction == other.direction;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (point.GetHashCode() * 397) ^ (int)direction;
            }
        }
        
        public override string ToString()
        {
            return string.Format("{{{0}, {1}}}", point, direction);
        }
    }
}
