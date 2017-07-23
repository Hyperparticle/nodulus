using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data;

namespace Core.Items
{
    public class Field : IBoardItem
    {
        public const int MaxLength = 10;
        
        public Point Position => ParentNode.Position;
        public Point ConnectedPosition => ConnectedNode.Position;
        public bool IsEnabled => true;
        public int Length { get; }
        public Direction Direction { get; }
        public PointDir PointDir { get; }

        public Node ParentNode { get; }
        public Node ConnectedNode { get; }

        public Arc Arc { get; private set; }
        public bool HasArc => Arc != null;

        public ICollection<Field> Overlap { get; }
        public PointDir ConnectedPointDir { get; }


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

            PointDir = new PointDir(Position, Direction);
            ConnectedPointDir = new PointDir(ConnectedPosition, Direction.Opposite());
        }

        public bool ValidPlacement(Arc arc)
        {
            // A placement is valid if:
            // 1. No Arc exists in the field
            // 2. Arc length is equal to field length
            // 3. Arcs do not overlap

            var noArc = !HasArc;// || Arc.Equals(arc);
            var overlap = Overlap.Any(field => field.HasArc);
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

        public Node Root(Direction dir)
        {
            return dir == Direction ? ConnectedNode : ParentNode;
        }

        public bool ContainsNode(Node node)
        {
            return ParentNode.Equals(node) || ConnectedNode.Equals(node);
        }

        public override string ToString()
        {
            return $"{Position} -> {ConnectedPosition} [{Length}]";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            var field = obj as Field;
            return field != null && Equals(field);
        }

        public override int GetHashCode()
        {
            return PointDir.GetHashCode();
        }

        public bool Equals(Field other)
        {
            return PointDir.Equals(other.PointDir);
        }
    }
}
