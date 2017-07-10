namespace Assets.Scripts.Core.Data
{
    /// <summary>
    /// A Point with a direction. Useful for uniquely identifying 
    /// an arc or a field in the game grid.
    /// </summary>
    public struct PointDir
    {
        public readonly Point Point;
        public readonly Direction Direction;

        public PointDir Opposite { get { return new PointDir(Point, Direction.Opposite()); } }

        public PointDir(Point point, Direction direction)
        {
            this.Point = point;
            this.Direction = direction;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PointDir && Equals((PointDir)obj);
        }

        public bool Equals(PointDir other)
        {
            return Point.Equals(other.Point) && Direction == other.Direction;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Point.GetHashCode() * 397) ^ (int)Direction;
            }
        }
        
        public override string ToString()
        {
            return string.Format("{{{0}, {1}}}", Point, Direction);
        }
    }
}
