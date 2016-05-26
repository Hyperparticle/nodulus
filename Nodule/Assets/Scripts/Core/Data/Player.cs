using System.Collections.Generic;
using Assets.Scripts.Core.Items;
using Assets.Scripts.Core.Moves;

namespace Assets.Scripts.Core.Data
{
    public class Player
    {
        private readonly IslandSet _islandSet;
        private Island _island;

        private readonly HashSet<Field> _pullFields = new HashSet<Field>();
        private readonly HashSet<Field> _pushFields = new HashSet<Field>();

        public IEnumerable<Field> PullFields { get { return _pullFields; } }
        public IEnumerable<Field> PushFields { get { return _pushFields; } }

        public Arc PulledArc { get; private set; }
        public int NumMoves { get; private set; }

        public bool Win
        {
            get { return _island.IsFinal; }
        }

        public Player(IslandSet islandSet, Node start)
        {
            _islandSet = islandSet;
            _island = islandSet.Get(start);
        }

        public void MoveTo(Node node)
        {
            _island = _islandSet.Get(node);
            UpdateFields();
        }

        public bool IsProximal(Field field)
        {
            return _island.Contains(field.ParentNode) || _island.Contains(field.ConnectedNode);
        }

        public bool PlayMove(IMove move)
        {
            var result = move.Play();
            NumMoves = result ? NumMoves + 1 : NumMoves;
            return result;
        }

        /// <summary>
        /// Updates the fields that can be pulled and pushed.
        /// </summary>
        private void UpdateFields()
        {
            _pullFields.Clear();
            _pushFields.Clear();

            _pullFields.UnionWith(_island.Inskirts);
            _pushFields.UnionWith(_island.Inskirts);
        }
    }
}
