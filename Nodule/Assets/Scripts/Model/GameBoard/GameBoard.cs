using System.Collections.Generic;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.GameBoard
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
        
        public Node StartNode { get; set; }

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

        /// <summary>
        /// Adds the specified Arc to the game board.
        /// </summary>
        public bool Push(Arc arc, Field field)
        {
            if (!field.ValidPlacement(arc)) { return false; }

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
            if (arc.IsPulled) { return false; }
            
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
