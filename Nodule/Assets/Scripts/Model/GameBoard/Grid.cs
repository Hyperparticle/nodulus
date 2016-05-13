using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model.Builders;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.GameBoard
{
    /// <summary>
    /// A Grid is a collection of <seealso cref="Node"/>s, <seealso cref="Edge"/>s, and 
    /// <seealso cref="Field"/>s in space. It defines the positioning of all nodes. It 
    /// contains methods to place and remove nodes and edges, in which the Grid will 
    /// auto-create fields between nodes as necessary. It also contains methods to find 
    /// a node at a given point, and find all nodes connected to a point (called an 
    /// <seealso cref="Island"/>).
    /// </summary>
    public class Grid
    {
        // Keep a simple collection of nodes, edges, fields, and islands
        private readonly ICollection<Node> _nodes = new HashSet<Node>();
        private readonly ICollection<Edge> _edges = new HashSet<Edge>();
        private ICollection<Field> _fields = new HashSet<Field>();
        private readonly ICollection<Island> _islands = new HashSet<Island>();
        
        // Map the positions of nodes
        private readonly IDictionary<Point, Node> _nodeMap = new Dictionary<Point, Node>();

        private readonly FieldBuilder _fieldBuilder;
        
        public IEnumerable<Node> Nodes { get { return _nodes; } }
        public IEnumerable<Edge> Edges { get { return _edges; } }
        public IEnumerable<Field> Fields { get { return _fields; } }

        public Grid()
        {
             _fieldBuilder = new FieldBuilder(EdgeConnectedHandler, EdgeDisconnectedHandler);
        }

        /// <summary>
        /// The Size is defined as the smallest rectangular bounds to contain
        /// all nodes in the Grid.
        /// </summary>
        public Point Size
        {
            get
            {
                return Point.Boundary(_nodes.Select(node => node.Position).ToList());
            }
        }

        /// <summary>
        /// Finds the node at the specified position, null otherwise
        /// </summary>
        public Node NodeAt(Point pos)
        {
            Node node;
            return _nodeMap.TryGetValue(pos, out node) ? node : null;
        }

        /// <summary>
        /// True if the grid has a node at the specified position
        /// </summary>
        public bool HasNodeAt(Point pos)
        {
            return _nodeMap.ContainsKey(pos);
        }

        /// <summary>
        /// Adds the specified node to the grid. A node will only be added 
        /// if there does not exist a node in the grid at the same position.
        /// </summary>
        /// <returns>True if the operation succeeded</returns>
        public bool AddNode(Node node)
        {
            // Don't add the node if another one already exists
            if (HasNodeAt(node.Position)) return false;
            
            // Add the node based on its position
            _nodes.Add(node);
            _nodeMap.Add(node.Position, node);

            // Build fields, and update the list
            _fields = _fieldBuilder.BuildFields(node);

            return true;
        }

        /// <summary>
        /// Removes the node at the specified position from the grid. 
        /// A node will only be removed if there exists a node at 
        /// that position.
        /// </summary>
        /// <returns>True if the operation succeeded</returns>
        public bool RemoveNode(Point pos)
        {
            // Don't continue if the node doesn't exist
            var node = NodeAt(pos);
            if (node == null) return false;

            // Remove the node
            _nodes.Remove(node);
            _nodeMap.Remove(node.Position);

            // Remove any attached edges
            node.Fields.Values
                .Where(field => field.HasEdge)
                .ToList()
                .ForEach(field => {
                    _edges.Remove(field.Edge);
                });

            // Destroy fields, and update the list
            _fields = _fieldBuilder.DestroyFields(node);

            return true;
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
