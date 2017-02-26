using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Core.Items;

namespace Assets.Scripts.Core.Data
{
    /// <summary>
    /// An Island is a subgraph of the game board grid. It maintains a collection 
    /// of nodes that are all connected (have a path) to each other via arcs.
    /// </summary>
    public class Island {

        private readonly HashSet<Field> _connectedFields;

        public HashSet<Node> ConnectedNodes { get; private set; }
        /// <summary>
        /// True if this island contains a final node
        /// </summary>
        public bool IsFinal { get { return ConnectedNodes.Any(node => node.Final); } }
        
        /// <summary>
        /// All directly connected fields with arcs
        /// </summary>
        public IEnumerable<Field> Inskirts { get { return _connectedFields.Where(field => field.HasArc); } }

        /// <summary>
        /// All directly connected fields without arcs
        /// </summary>
        public IEnumerable<Field> Outskirts { get { return _connectedFields.Where(field => !field.HasArc); } }

        /// <summary>
        /// Creates a new island with one node in it
        /// </summary>
        public Island(Node node)
        {
            ConnectedNodes = new HashSet<Node> { node };
            _connectedFields = new HashSet<Field>(node.Fields.Values);
        }

        /// <summary>
        /// Creates a new island with a set of nodes
        /// </summary>
        private Island(HashSet<Node> connectedNodes, HashSet<Field> connectedFields) 
        {
            ConnectedNodes = connectedNodes;
            _connectedFields = connectedFields;
        }

        /// <summary>
        /// Returns true if the node is contained inside this island
        /// </summary>
        public bool Contains(Node node)
        {
            return ConnectedNodes.Contains(node);
        }

        /// <summary>
        /// Joins this island with another.
        /// </summary>
        /// <returns>
        /// The island passed as a parameter, now 
        /// containing the union of both node sets.
        /// </returns>
        public Island Join(Island island)
        {
            island.ConnectedNodes.UnionWith(ConnectedNodes);
            island._connectedFields.UnionWith(_connectedFields);
            return island;
        }

        /// <summary>
        /// Splits this island across the 
        /// specified field. This island will contain 
        /// the field's parent node, while the returned island 
        /// will contain the connected node. Note that this 
        /// does not gurantee that the island will be split 
        /// into two pieces (if the nodes are still connected).
        /// </summary>
        /// <returns>The newly created island</returns>
        public Island Split(Field field)
        {
            var parent = field.ParentNode;
            var connected = field.ConnectedNode;

            // Find all current connections with the parent node
            ConnectedNodes.Clear();
            _connectedFields.Clear();
            FindConnected(parent, ConnectedNodes, _connectedFields);              

            // Find all current connections with the connected node
            var splitNodes = new HashSet<Node>();
            var splitFields = new HashSet<Field>();
            FindConnected(connected, splitNodes, splitFields);

            // Return the island containing the connected node
            return new Island(splitNodes, splitFields);
        }

        /// <summary>
        /// Recursively collects nodes that are connected to the specified node.
        /// </summary>
        private static void FindConnected(Node start, HashSet<Node> nodes, HashSet<Field> fields) 
        {
            nodes.Add(start);
            fields.UnionWith(start.Fields.Values);

            // Accumulates all connected nodes in the collection

            // Find all parent node connections
            foreach (var connection in start.Connections
                .Where(connection => !nodes.Contains(connection.ParentNode)))
            {
                FindConnected(connection.ParentNode, nodes, fields);
            }

            // Find all connected node connections
            foreach (var connection in start.Connections
                .Where(connection => !nodes.Contains(connection.ConnectedNode)))
            {
                FindConnected(connection.ConnectedNode, nodes, fields);
            }
        }

        /// <summary>
        /// Finds the node path between the start and end nodes in the island.
        /// </summary>
        public static bool FindPath(Node start, Node end, HashSet<Node> nodes, List<Node> path)
        {
            nodes.Add(start);

            if (start.Equals(end)) {
                path.Add(end);
                return true;
            }

            // Find all parent node connections
            foreach (var connection in start.Connections
                .Where(connection => !nodes.Contains(connection.ParentNode)))
            {
                var found = FindPath(connection.ParentNode, end, nodes, path);
                if (found) {
                    path.Add(start);
                    return true;
                }
            }

            // Find all connected node connections
            foreach (var connection in start.Connections
                .Where(connection => !nodes.Contains(connection.ConnectedNode)))
            {
                var found = FindPath(connection.ConnectedNode, end, nodes, path);
                if (found) {
                    path.Add(start);
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            var join = string.Join(",", ConnectedNodes.Select(node => node.ToString()).ToArray());
            return string.Format("[{0}]", join);
        }
    }
}
