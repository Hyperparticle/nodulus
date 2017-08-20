namespace Core.Data
{
    /// <summary>
    /// A Point with a direction. Useful for uniquely identifying 
    /// an arc or a field in the game grid.
    /// </summary>
    public struct PointDir
    {
        public Point Point { get; }
        public Direction Direction { get; }

        public PointDir Opposite => new PointDir(Point, Direction.Opposite());

        public PointDir(Point point, Direction direction)
        {
            Point = point;
            Direction = direction;
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
            unchecked {
                return (Point.GetHashCode() * 397) ^ (int) Direction;
            }
        }
        
        public override string ToString()
        {
            return $"{{{Point}, {Direction}}}";
        }
    }
}
