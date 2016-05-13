using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model.Data;

namespace Assets.Scripts.Model.Items
{
    public class Field : IBoardItem
    {
        public const int MaxLength = 10;
        
        public Point Position { get { return ParentNode.Position; } }
        public bool IsEnabled { get { return true; } }
        public int Length { get; private set; }
        public Direction Direction { get; private set; }

        public Node ParentNode { get; private set; }
        public Node ConnectedNode { get; private set; }

        public Edge Edge { get; private set; }
        public bool HasEdge { get { return Edge != null; } }
        
        public ICollection<Field> Overlap { get; private set; }

        // Note: parent should be the top left node
        public Field(int length, Node parentNode, Node connectedNode)
        {
            Length = Math.Abs(length);
            ParentNode = parentNode;
            ConnectedNode = connectedNode;

            Overlap = new HashSet<Field>();

            Direction = parentNode.GetDirection(connectedNode);
            parentNode.Fields.Add(Direction, this);
            connectedNode.Fields.Add(Direction.Opposite(), this);
        }

        public bool ValidPlacement(Edge edge)
        {
            // A placement is valid if:
            // 1. No edge exists in the field
            // 2. Edge length is equal to field length
            // 3. Edges do not overlap

            var overlap = Overlap.Any(field => field.HasEdge);
            var noEdge = !HasEdge || Edge.Equals(edge);
            return noEdge && edge.Length == Length && !overlap;
        }

        public void ConnectEdge(Edge edge)
        {
            Edge = edge;

            // Union the nodes' islands together
            ParentNode.JoinIsland(ConnectedNode);
        }

        public void DisconnectEdge(Edge edge)
        {
            Edge = null;

            // Split the nodes' islands apart
            ParentNode.SplitIsland(this);
        }

        public void DisconnectNodes()
        {
            ParentNode.Fields.Remove(Direction);
            ConnectedNode.Fields.Remove(Direction.Opposite());
        }

        public Node Root(Direction direction)
        {
            return direction == Direction.Opposite() ? ConnectedNode : ParentNode;
        }

        public bool ContainsNode(Node node)
        {
            return ParentNode.Equals(node) || ConnectedNode.Equals(node);
        }
    }
}
