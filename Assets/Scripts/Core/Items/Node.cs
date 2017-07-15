using System.Collections.Generic;
using System.Linq;
using Core.Data;

namespace Core.Items
{
    /// <summary>
    /// A Node represents a point in grid space, allowing arcs to 
    /// connect between them.
    /// </summary>
    public class Node : IBoardItem
    {
        public Point Position { get; private set; }
        public bool IsEnabled { get { return true; } }
        public int Length { get { return 0; } }
        public Direction Direction { get { return Direction.None; } }

        private readonly Dictionary<Direction, Field> _fields =
            new Dictionary<Direction, Field>();
        public Dictionary<Direction, Field> Fields { get { return _fields; } }

        public IEnumerable<Field> Connections
        {
            get
            {
                return _fields.Values
                    .Where(field => field.HasArc && !field.Arc.IsPulled);
            }
        }
        
        public bool Final { get; set; }

        public Node(Point pos, bool final = false)
        {
            Position = pos;
            Final = final;
        }

        public bool HasConnection(Direction dir)
        {
            Field field;
            if (!_fields.TryGetValue(dir, out field)) return false;
            return !field.HasArc;
        }

        public void DisconnectField(Direction dir)
        {
            Fields[dir].DisconnectNodes();
        }
        
        public Direction GetDirection(Node end)
        {
            var diff = end.Position - Position;
            return diff.ToDirection;
        }

        public int GetDistance(Node end)
        {
            var diff = end.Position - Position;
            return diff.X != 0 ? diff.X : diff.Y;
        }

        public override string ToString()
        {
            return Position.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            var node = obj as Node;
            return node != null && Equals(node);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public bool Equals(Node other)
        {
            return Position.Equals(other.Position);
        }
    }
}
