using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model.Data;

namespace Assets.Scripts.Model.Items
{
    /// <summary>
    /// A Node represents a point in grid space, allowing edges to 
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

        private Island _island;

        public IEnumerable<Field> Connections
        {
            get
            {
                return _fields.Values
                    .Where(field => field.HasEdge && !field.Edge.IsPulled);
            }
        }
        
        public bool Final { get; set; }

        public Node(Point position, bool final = false)
        {
            Position = position;
            Final = final;

            _island = new Island(this);
        }

        public bool HasConnection(Direction direction)
        {
            Field field;
            if (!_fields.TryGetValue(direction, out field)) return false;
            return !field.HasEdge;
        }

        public void DisconnectField(Direction direction)
        {
            Fields[direction].DisconnectNodes();
        }

        public void JoinIsland(Node node)
        {
            var removedIsland = _island.Join(node._island);

            node._island = removedIsland;
        }

        public void SplitIsland(Field field)
        {
            if (this != field.ParentNode)
            {
                Console.Error.WriteLine("Incorrect use: this node must be parent of field");
                return;
            }

            var newIsland = _island.Split(field);
            field.ConnectedNode._island = newIsland;
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
    }
}
