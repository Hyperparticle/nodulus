using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.GameBoard
{
    /// <summary>
    /// A Gameboard is a <seealso cref="Grid"/> that keeps track of 
    /// nodes connected via edges.
    /// </summary>
    public class GameBoard
    {
        private readonly Grid _grid;

        private readonly ICollection<Edge> _edges = new HashSet<Edge>();
        private readonly ICollection<Island> _islands = new HashSet<Island>();
        
        public IEnumerable<Node> Nodes { get { return _grid.Nodes; } }
        public IEnumerable<Field> Fields { get { return _grid.Fields; } }
        public IEnumerable<Edge> Edges { get { return _edges; } }
        public IEnumerable<Island> Islands { get { return _islands; } }

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

        /// <summary>
        /// Adds the specified edge to the grid.
        /// </summary>
        public void AddEdge(Edge edge)
        {
            _edges.Add(edge);
        }

        /// <summary>
        /// An event handler that should fire whenever an edge is connected
        /// </summary>
        public void EdgeConnectedHandler(object sender, Island removedIsland)
        {
            _islands.Remove(removedIsland);
        }

        /// <summary>
        /// An event handler that should fire whenever an edge is disconnected
        /// </summary>
        public void EdgeDisconnectedHandler(object sender, Island newIsland)
        {
            _islands.Add(newIsland);
        }
    }
}
