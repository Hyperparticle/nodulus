using System;
using System.Collections.Generic;
using Assets.Scripts.Model.Items;

namespace Assets.Scripts.Model.Data
{
    public class Island : EventArgs {

        public event EventHandler<Island> IslandJoined;
        public event EventHandler<Island> IslandSplit;

        private readonly ICollection<Node> _connectedNodes = new HashSet<Node>();

        public Island(Node node)
        {
            _connectedNodes.Add(node);
        }

        public bool ContainsNode(Node node)
        {
            return _connectedNodes.Contains(node);
        }

        /// <summary>
        /// Joins this island with another. The island given as 
        /// a parameter will be discarded, and this island will 
        /// become the union of both islands.
        /// </summary>
        /// <returns>The discarded island</returns>
        public Island Join(Island island)
        {
            throw new NotImplementedException();

            // Fire off an event
            if (IslandJoined != null)
                IslandJoined(this, removedIsland);

            return island;
        }

        /// <summary>
        /// Splits this island into two pieces, across the 
        /// specified field. This island will become the 
        /// island corresponding to the field's parent node,
        /// while the returned island will correspond to 
        /// the connected node.
        /// </summary>
        /// <returns>The newly created island</returns>
        public Island Split(Field field)
        {
            throw new NotImplementedException();
            
            // Fire off an event
            if (IslandSplit != null)
                IslandSplit(this, newIsland);

            return newIsland;
        }
    }
}
