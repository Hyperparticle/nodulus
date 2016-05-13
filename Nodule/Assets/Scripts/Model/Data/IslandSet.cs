using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.Data
{
    public class IslandSet
    {
        private readonly HashSet<Island> _islands = new HashSet<Island>();
        private readonly IDictionary<Node, Island> _nodeMap = new Dictionary<Node, Island>();

        public IEnumerable<Island> Islands { get { return _islands; } }

        public void Add(Node node)
        {
            _islands.Add(new Island(node));
        }

        public void Connect(Arc arc, Field field)
        {
            var removedIsland = _island.Join(node._island);

            node._island = removedIsland;
        }

        public void Disconnect(Arc arc)
        {
            if (this != field.ParentNode)
            {
                Console.Error.WriteLine("Incorrect use: this node must be parent of field");
                return;
            }

            var newIsland = _island.Split(field);
            field.ConnectedNode._island = newIsland;
        }
        
        public bool IsConnected(Node start, Node end)
        {
            // If the nodes point to the same island, they are connected
            return _nodeMap[start].Equals(_nodeMap[end]);
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
