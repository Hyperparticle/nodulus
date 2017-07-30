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
        public Point Position { get; }
        public bool IsEnabled => true;
        public int Length => 0;
        public Direction Direction => Direction.None;

        public Dictionary<Direction, Field> Fields { get; } = new Dictionary<Direction, Field>();

        public IEnumerable<Field> Connections
        {
            get
            {
                return Fields.Values
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
            if (!Fields.TryGetValue(dir, out field)) return false;
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
            return diff.x != 0 ? diff.x : diff.y;
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
