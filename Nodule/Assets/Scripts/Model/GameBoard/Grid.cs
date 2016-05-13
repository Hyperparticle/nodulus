using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model.Builders;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.GameBoard
{
    /// <summary>
    /// A Grid is a collection of <seealso cref="Node"/>s in a 2D space. 
    /// It defines the positioning of all nodes, and auto-generates fields 
    /// (which specify potential connection points for edges) as nodes are 
    /// added and removed.
    /// </summary>
    public class Grid
    {
        // Keep a simple collection of fields
        private ICollection<Field> _fields = new HashSet<Field>();

        
        // Map the positions of nodes
        private readonly IDictionary<Point, Node> _nodeMap = new Dictionary<Point, Node>();

        private readonly FieldBuilder _fieldBuilder;
        
        public IEnumerable<Node> Nodes { get { return _nodeMap.Values; } }
        public IEnumerable<Field> Fields { get { return _fields; } }


        public Grid()
        {
             _fieldBuilder = new FieldBuilder();
        }

        /// <summary>
        /// The Size is defined as the smallest rectangular bounds to contain
        /// all nodes in the Grid.
        /// </summary>
        public Point Size
        {
            get
            {
                return Point.Boundary(_nodeMap.Keys);
            }
        }

        /// <summary>
        /// Finds the node at the specified position, null otherwise
        /// </summary>
        public Node NodeAt(Point pos)
        {
            return NodeAt(pos, _nodeMap);
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
            _nodeMap.Add(node.Position, node);

            // Build fields, and update the list
            _fields = _fieldBuilder.BuildFields(node, _nodeMap);

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

            // Remove the node itself
            _nodeMap.Remove(node.Position);

            // Destroy fields, and update the list
            _fields = _fieldBuilder.DestroyFields(node);

            return true;
        }

        public static Node NodeAt(Point pos, IDictionary<Point, Node> nodeMap)
        {
            Node node;
            return nodeMap.TryGetValue(pos, out node) ? node : null;
        }

    }
}
