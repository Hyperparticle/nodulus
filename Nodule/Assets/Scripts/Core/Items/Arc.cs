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
        public Point ConnectedPosition { get { return ConnectedNode.Position; } }
        public bool IsEnabled { get { return true; } }
        public int Length { get; private set; }
        public Direction Direction { get { return Field == null ? Direction.None : Field.Direction; } }

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
        }

        public void Pull()
        {
            IsPulled = true;

            // Disconnect this Arc from an existing field
            if (Field != null) {
                Field.DisconnectArc(this);
            }

            Field = null;
        }

        public void Push(Field field)
        {
            //if (field == null) {
            //    // TODO: logging
            //    return;
            //}

            // Pull before pushing
            Pull();

            // Connect this Arc to the new field
            Field = field;
            field.ConnectArc(this);
            IsPulled = false;
        }

        public Node Root(Direction dir)
        {
            return Field.Root(dir);
        }


        public override string ToString()
        {
            return IsPulled ? "PULLED" : string.Format("{0} -> {1}", Position, ConnectedPosition);
        }
    }
}
