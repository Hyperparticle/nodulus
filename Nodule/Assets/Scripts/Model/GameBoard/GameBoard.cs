using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.GameBoard
{
    public class GameBoard
    {
        private readonly Grid _grid;

        public IEnumerable<Node> Nodes { get { return _grid.Nodes; } }
        public IEnumerable<Edge> Edges { get { return _grid.Edges; } }
        public IEnumerable<Field> Fields { get { return _grid.Fields; } }
        public Node StartNode { get; set; }

        public Point Size { get; private set; }
        
        public GameBoard()
        {
            _grid = new Grid();
            Size = _grid.Size;
        }

        public bool PlaceNode(Node node)
        {
            var added = _grid.AddNode(node);
            Size = _grid.Size;
            return added;
        }

        public bool CreateEdge(Point pos, Direction direction)
        {
            var node = _grid.NodeAt(pos);
            if (node == null) return false;

            Field field;
            if (!node.Fields.TryGetValue(direction, out field)) return false;

            CreateEdge(field);
            return true;
        }

        public void CreateEdge(Field field)
        {
            _grid.AddEdge(new Edge(field), field);
        }

        public static bool IsConnected(Node start, Node end)
        {
            return IsConnected(start, end, new HashSet<Node>());
        }

        private static bool IsConnected(Node start, Node end, ICollection<Node> nodes)
        {
            if (start.Equals(end)) return true;

            nodes.Add(start);

            // Simple recursive breadth-first search (no loops)
            return start.Connections
                .Where(connection => !nodes.Contains(connection.ConnectedNode))          // Prevent endless looping
                .Any(connection => IsConnected(connection.ConnectedNode, end, nodes));   // True if connection exists
        }

        public static IEnumerable<Node> ConnectedNodes(Node start)
        {
            var nodes = new HashSet<Node>();
            FindConnectedNodes(start, nodes);

            return nodes;
        }

        private static void FindConnectedNodes(Node start, ICollection<Node> nodes)
        {
            nodes.Add(start);

            // Accumulates all connected nodes in the collection
            foreach (var connection in start.Connections
                .Where(connection => !nodes.Contains(connection.ConnectedNode)))
            {
                FindConnectedNodes(connection.ConnectedNode, nodes);
            }

            foreach (var connection in start.Connections
                .Where(connection => !nodes.Contains(connection.ParentNode)))
            {
                FindConnectedNodes(connection.ParentNode, nodes);
            }
        }
    }
}
