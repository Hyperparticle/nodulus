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

        public Arc Arc { get; private set; }
        public bool HasArc { get { return Arc != null; } }
        
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

        public bool ValidPlacement(Arc arc)
        {
            // A placement is valid if:
            // 1. No Arc exists in the field
            // 2. Arc length is equal to field length
            // 3. Arcs do not overlap

            var overlap = Overlap.Any(field => field.HasArc);
            var noArc = !HasArc || Arc.Equals(arc);
            return noArc && arc.Length == Length && !overlap;
        }

        public void ConnectArc(Arc arc)
        {
            Arc = arc;
        }

        public void DisconnectArc(Arc arc)
        {
            Arc = null;
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
