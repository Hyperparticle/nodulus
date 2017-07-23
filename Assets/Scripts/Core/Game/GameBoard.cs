using System.Collections.Generic;
using Core.Builders;
using Core.Data;
using Core.Items;

namespace Core.Game
{
    /// <summary>
    /// A GameBoard is a <seealso cref="Grid"/> that keeps track of 
    /// arcs that connect nodes.
    /// </summary>
    public class GameBoard
    {
        private readonly Grid _grid;

        public HashSet<Node> Nodes => _grid.Nodes;

        public HashSet<Arc> Arcs { get; } = new HashSet<Arc>();

        public HashSet<Field> Fields => _grid.Fields;

        public Node StartNode { get; set; }

        public Island StartIsland => IslandSet.Get(StartNode);

        public IslandSet IslandSet { get; } = new IslandSet();

        public Point Size { get; private set; }

        public GameBoard()
        {
            _grid = new Grid();
            Size = _grid.Size;
        }

        public bool PlaceNode(Node node)
        {
            var added = _grid.AddNode(node);
            Size = _grid.Size;
            IslandSet.Add(node);
            return added;
        }

        public bool CreateArc(Point pos, Direction dir)
        {
            var node = _grid.NodeAt(pos);
            if (node == null) return false;

            Field field;
            return node.Fields.TryGetValue(dir, out field) && CreateArc(field);
        }

        public Arc GetArcAt(Point arcPos, Direction arcDir)
        {
            return _grid.GetArcAt(arcPos, arcDir);
        }

        public Field GetFieldAt(Point fieldPos, Direction fieldDir)
        {
            return _grid.GetFieldAt(fieldPos, fieldDir);
        }

        public bool CreateArc(Field field)
        {
            if (field.HasArc) return false;
            var arc = new Arc(field);

            return Push(arc, field);
        }

        /// <summary>
        /// Adds the specified Arc to the game board.
        /// </summary>
        public bool Push(Arc arc, Field field)
        {
            if (!field.ValidPlacement(arc)) {
                return false;
            }

            // Push the arc onto the field
            arc.Push(field);
            Arcs.Add(arc);

            // Notify the island set of connected nodes
            IslandSet.Connect(field);

            return true;
        }

        /// <summary>
        /// Removes the specified Arc from the game board.
        /// </summary>
        public bool Pull(Arc arc)
        {
            if (arc.IsPulled) {
                return false;
            }

            // Pull the arc from the field
            var field = arc.Field;
            arc.Pull();
            Arcs.Remove(arc);

            // Notify the island set of disconnected nodes
            IslandSet.Disconnect(field);

            return true;
        }

        /// <summary>
        /// Checks if the two nodes are connected via arcs
        /// </summary>
        public bool IsConnected(Node start, Node end)
        {
            return IslandSet.IsConnected(start, end);
        }

        /// <summary>
        /// Obtain a string representation of the game board
        /// </summary>
        public string GetBoard(IEnumerable<Field> pullFields, IEnumerable<Field> pushFields)
        {
            return BoardPrinter.GetBoard(Size, Nodes, Arcs, pullFields, pushFields);
        }
    }
}
