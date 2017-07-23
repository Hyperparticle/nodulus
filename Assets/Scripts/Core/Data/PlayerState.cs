using System.Collections.Generic;
using System.Linq;
using Core.Game;
using Core.Items;

namespace Core.Data
{
    public class PlayerState
    {
        private readonly IslandSet _islandSet;
        private Island _playerIsland;

        private readonly HashSet<Node> _nodes;
        private readonly HashSet<Field> _fields;
        private readonly HashSet<Arc> _arcs;

        // Separate player board items from non-player board items
        private readonly HashSet<Node> _playerNodes = new HashSet<Node>();
        private readonly HashSet<Field> _pushFields = new HashSet<Field>();
        private readonly HashSet<Arc> _playerArcs = new HashSet<Arc>();

        private readonly HashSet<Node> _nonPlayerNodes = new HashSet<Node>();
        private readonly HashSet<Field> _nonPushFields = new HashSet<Field>();
        private readonly HashSet<Arc> _nonPlayerArcs = new HashSet<Arc>();

        public IEnumerable<Node> PlayerNodes => _playerNodes;
        public IEnumerable<Field> PushFields => _pushFields;
        public IEnumerable<Arc> PlayerArcs => _playerArcs;

        public IEnumerable<Node> NonPlayerNodes => _nonPlayerNodes;
        public IEnumerable<Field> NonPushFields => _nonPushFields;
        public IEnumerable<Arc> NonPlayerArcs => _nonPlayerArcs;

        public bool IsFinal => _playerIsland.IsFinal;
        public Point PullPosition { get; private set; }


        public bool Contains(Node node)
        {
            return _playerIsland.Contains(node);
        }

        public PlayerState(GameBoard gameBoard)
        {
            _islandSet = gameBoard.IslandSet;

            _nodes = new HashSet<Node>(gameBoard.Nodes);
            _fields = new HashSet<Field>(gameBoard.Fields);
            _arcs = new HashSet<Arc>(gameBoard.Arcs);
        }

        public void MoveTo(Node node)
        {
            PullPosition = node.Position;
            _playerIsland = _islandSet.Get(node);
            UpdateState();
        }

        /// <summary>
        /// Updates the fields that can be pulled and pushed.
        /// </summary>
        private void UpdateState()
        {
            // TODO: improve efficiency
            _playerNodes.Clear();
            _playerArcs.Clear();
            _nonPlayerNodes.Clear();
            _nonPlayerArcs.Clear();

            _playerNodes.UnionWith(_playerIsland.ConnectedNodes);
            _nonPlayerNodes.UnionWith(_nodes);
            _nonPlayerNodes.ExceptWith(_playerNodes);

            _playerArcs.UnionWith(_playerIsland.Inskirts.Select(field => field.Arc));
            _nonPlayerArcs.UnionWith(_arcs);
            _nonPlayerArcs.ExceptWith(_playerArcs);
        }

        public void UpdatePush(Arc pullArc)
        {
            _pushFields.Clear();
            _nonPushFields.Clear();

            if (pullArc == null) {
                _nonPushFields.UnionWith(_fields);
                return;
            } 

            _pushFields.UnionWith(_playerIsland.Outskirts.Where(field => field.ValidPlacement(pullArc)));
            _nonPushFields.UnionWith(_fields);
            _nonPushFields.ExceptWith(_pushFields);
        }

        public bool HasNodeAt(Node node)
        {
            return _playerIsland.Contains(node);
        }

        public List<Node> PushPath(Node start, Node end)
        {
            // Make sure the nodes are in the player island
            if (!_playerIsland.Contains(start) || !_playerIsland.Contains(end)) {
                return new List<Node>();
            }

            var nodes = new HashSet<Node>();
            var path = new List<Node>();

            Island.FindPath(start, end, nodes, path);
            path.Reverse();

            return path;
        }
    }
}
