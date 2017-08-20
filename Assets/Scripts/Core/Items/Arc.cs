using Core.Data;

namespace Core.Items
{
    /// <summary>
    /// An Arc represents a visible connection between two nodes, allowing a traversible
    /// path between them.
    /// </summary>
    public class Arc : IBoardItem
    {
        public Point Position => ParentNode.Position;
        public Point ConnectedPosition => ConnectedNode.Position;
        public bool IsEnabled => true;
        public int Length { get; }
        public Direction Direction => Field?.Direction ?? Direction.None;

        public Node ParentNode => Field.ParentNode;
        public Node ConnectedNode => Field.ConnectedNode;
        public Field Field { get; private set; }

        /// <summary>
        /// True if this arc is in its pulled state.
        /// </summary>
        public bool IsPulled => Field == null;

        public Arc(Field field)
        {
            Length = field.Length;
        }

        public void Pull()
        {
            // Disconnect this Arc from an existing field
            Field?.DisconnectArc(this);
            Field = null;
        }

        public void Push(Field field)
        {
            //if (field == null) {
            //    // TODO: logging
            //    return;
            //}

            // Pull before pushing
            Pull();

            // Connect this Arc to the new field
            Field = field;
            field.ConnectArc(this);
        }

        public Node Root(Direction dir)
        {
            return Field.Root(dir);
        }


        public override string ToString()
        {
            return IsPulled ? $"PULLED [{Length}]" : Field.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            var arc = obj as Arc;
            return arc != null && Equals(arc);
        }

        public override int GetHashCode()
        {
            return Field != null ? Field.GetHashCode() : 0;
        }

        public bool Equals(Arc other)
        {
            return IsPulled || Field.Equals(other.Field);
        }
    }

    public class ArcState
    {
        public Point Point { get; }
        public Direction Direction { get; }
        public bool Pulled { get; }

        public ArcState(Point point, Direction direction, bool pulled)
        {
            Point = point;
            Direction = direction;
            Pulled = pulled;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ArcState && Equals((ArcState) obj);
        }

        public bool Equals(ArcState other)
        {
            return Point.Equals(other.Point) && Direction == other.Direction && Pulled == other.Pulled;
        }

        public override int GetHashCode()
        {
            unchecked {
                var hashCode = Point.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Direction;
                hashCode = (hashCode * 397) ^ Pulled.GetHashCode();
                return hashCode;
            }
        }
        
        public override string ToString()
        {
            return $"{{{Point}, {Direction}, {Pulled}}}";
        }
    }
}
