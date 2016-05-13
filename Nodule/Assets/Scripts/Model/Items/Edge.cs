using Assets.Scripts.Model.Data;

namespace Assets.Scripts.Model.Items
{
    /// <summary>
    /// An Edge represents a visible connection between two nodes, allowing a traversible
    /// path between them.
    /// </summary>
    public class Edge : IBoardItem
    {
        public Point Position { get { return ParentNode.Position; } }
        public bool IsEnabled { get { return true; } }
        public int Length { get; private set; }
        public Direction Direction { get { return Field.Direction; } }

        public Node ParentNode { get { return Field.ParentNode; } }
        public Node ConnectedNode { get { return Field.ConnectedNode; } }
        public Field Field { get; private set; }

        public bool IsPulled { get; private set; }

        public Edge(Field field)
        {
            Length = field.Length;
            Push(field);
        }

        public void Pull()
        {
            IsPulled = true;
        }

        public void Push(Field field)
        {
            // Disconnect this edge from an existing field
            if (Field != null)
                Field.DisconnectEdge(this);

            // Connect this edge to the new field
            Field = field;
            field.ConnectEdge(this);
            IsPulled = false;
        }

        public Node Root(Direction direction)
        {
            return Field.Root(direction);
        }
    }
}
