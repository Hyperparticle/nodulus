using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.Data
{
    /// <summary>
    /// An Island is a subgraph of the game board grid. It maintains a collection 
    /// of nodes that are all connected (have a path) to each other via arcs.
    /// </summary>
    public class Island {

        private readonly HashSet<Node> _connectedNodes;

        /// <summary>
        /// True if this island contains a final node
        /// </summary>
        public bool IsFinal { get { return _connectedNodes.Any(node => node.Final); } }
        
        /// <summary>
        /// Creates a new island with one node in it
        /// </summary>
        public Island(Node node)
        {
            _connectedNodes = new HashSet<Node> { node };
        }

        /// <summary>
        /// Creates a new island with a set of nodes
        /// </summary>
        private Island(HashSet<Node> connectedNodes)
        {
            _connectedNodes = connectedNodes;
        }

        /// <summary>
        /// Returns true if the node is contained inside this island
        /// </summary>
        public bool ContainsNode(Node node)
        {
            return _connectedNodes.Contains(node);
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
            island._connectedNodes.UnionWith(_connectedNodes);
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

            // Find all current connections with the connected node
            var split = ConnectedNodes(connected);

            // If the new set contains the parent 
            // node, then this island has not been split
            if (split.Contains(parent)) { return this; }
            
            // Subtract the split set from this island (so that
            // this island has only nodes connected to the parent)
            _connectedNodes.ExceptWith(split);
            
            // Return the island containing the connected node
            return new Island(split);
        }

        /// <summary>
        /// Generates a list of all nodes that are connected to the specified node.
        /// </summary>
        private static HashSet<Node> ConnectedNodes(Node start)
        {
            var nodes = new HashSet<Node>();
            FindConnectedNodes(start, nodes);

            return nodes;
        }

        /// <summary>
        /// Recursively collects nodes that are connected to the specified node.
        /// </summary>
        private static void FindConnectedNodes(Node start, HashSet<Node> nodes)
        {
            nodes.Add(start);

            // Accumulates all connected nodes in the collection

            // Find all parent node connections
            foreach (var connection in start.Connections
                .Where(connection => !nodes.Contains(connection.ParentNode)))
            {
                FindConnectedNodes(connection.ParentNode, nodes);
            }

            // Find all connected node connections
            foreach (var connection in start.Connections
                .Where(connection => !nodes.Contains(connection.ConnectedNode)))
            {
                FindConnectedNodes(connection.ConnectedNode, nodes);
            }
        }
    }
}
