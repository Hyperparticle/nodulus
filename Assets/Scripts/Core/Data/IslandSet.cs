using System.Collections.Generic;
using Core.Items;

namespace Core.Data
{
    /// <summary>
    /// Maintains a disjoint set of node islands. This is used to determine if two nodes are 
    /// connected via arcs.
    /// </summary>
    public class IslandSet
    {
        private readonly IDictionary<Node, Island> _islandMap = new Dictionary<Node, Island>();

        //public IEnumerable<Island> Islands { get { return _islandMap.Values; } }

        public Island Get(Node node)
        {
            return _islandMap[node];
        }

        public void Add(Node node)
        {
            _islandMap.Add(node, new Island(node));
        }

        public void Connect(Field field)
        {
            var parent = field.ParentNode;
            var connected = field.ConnectedNode;
            var start = _islandMap[parent];
            var end = _islandMap[connected];

            // Join the islands into one
            var joinedIsland = start.Join(end);

            // Update the map with all nodes that are pointing to the new island
            foreach (var node in joinedIsland.ConnectedNodes) {
                _islandMap[node] = joinedIsland;
            }
        }

        public void Disconnect(Field field)
        {
            var parent = field.ParentNode;
            var connected = field.ConnectedNode;
            var start = _islandMap[parent];
            var end = _islandMap[connected];

            // if the nodes' islands are not equal, they are already disconnected
            if (!start.Equals(end)) {
                return;
            }

            // Try to split the islands (but can result in no split)
            var newIsland = start.Split(field);

            // Update the map with all nodes that are pointing to the new island
            foreach (var node in newIsland.ConnectedNodes) {
                _islandMap[node] = newIsland;
            }
        }

        /// <summary>
        /// True if the two nodes are connected (i.e., in the same island)
        /// </summary>
        public bool IsConnected(Node start, Node end)
        {
            // If the nodes point to the same island, they are connected
            return _islandMap[start].Equals(_islandMap[end]);
        }
    }
}
