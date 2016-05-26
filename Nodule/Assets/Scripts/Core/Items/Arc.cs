using Assets.Scripts.Core.Data;

namespace Assets.Scripts.Core.Items
{
    /// <summary>
    /// An Arc represents a visible connection between two nodes, allowing a traversible
    /// path between them.
    /// </summary>
    public class Arc : IBoardItem
    {
        public Point Position { get { return ParentNode.Position; } }
        public bool IsEnabled { get { return true; } }
        public int Length { get; private set; }
        public Direction Direction { get { return Field.Direction; } }

        public Node ParentNode { get { return Field.ParentNode; } }
        public Node ConnectedNode { get { return Field.ConnectedNode; } }
        public Field Field { get; private set; }

        /// <summary>
        /// True if this arc is in its pulled state.
        /// </summary>
        public bool IsPulled { get; private set; }

        public Arc(Field field)
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
            // Disconnect this Arc from an existing field
            if (Field != null)
            {
                Field.DisconnectArc(this);
            }
                

            // Connect this Arc to the new field
            Field = field;
            field.ConnectArc(this);
            IsPulled = false;
        }

        public Node Root(Direction direction)
        {
            return Field.Root(direction);
        }
    }
}
