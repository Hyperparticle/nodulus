using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;

namespace Assets.Scripts.Core.Game
{
    /// <summary>
    /// A GameBoard is a <seealso cref="Grid"/> that keeps track of 
    /// arcs that connect nodes.
    /// </summary>
    public class GameBoard
    {
        private readonly Grid _grid;

        private readonly ICollection<Arc> _arcs = new HashSet<Arc>();
        private readonly IslandSet _islandSet = new IslandSet();

        public IEnumerable<Node> Nodes
        {
            get { return _grid.Nodes; }
        }

        public IEnumerable<Arc> Arcs
        {
            get { return _arcs; }
        }

        public IEnumerable<Field> Fields
        {
            get { return _grid.Fields; }
        }

        public Node StartNode { get; set; }

        public Island StartIsland
        {
            get { return _islandSet.Get(StartNode); }
        }

        public IslandSet IslandSet
        {
            get { return _islandSet; }
        }

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
            _islandSet.Add(node);
            return added;
        }

        public bool CreateArc(Point pos, Direction direction)
        {
            var node = _grid.NodeAt(pos);
            if (node == null) return false;

            Field field;
            return node.Fields.TryGetValue(direction, out field) && CreateArc(field);
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
            _arcs.Add(arc);

            // Notify the island set of connected nodes
            _islandSet.Connect(field);

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
            arc.Pull();
            _arcs.Remove(arc);

            // Notify the island set of disconnected nodes
            _islandSet.Disconnect(arc.Field);

            return true;
        }

        /// <summary>
        /// Checks if the two nodes are connected via arcs
        /// </summary>
        public bool IsConnected(Node start, Node end)
        {
            return _islandSet.IsConnected(start, end);
        }
    }
}
