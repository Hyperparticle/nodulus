using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Items;

namespace Assets.Scripts.View.Data
{
    public class ArcViewMap
    {
        private readonly IDictionary<PointDir, ArcView> _arcMap = new Dictionary<PointDir, ArcView>();
        private readonly IDictionary<Point, HashSet<ArcView>> _arcSet = new Dictionary<Point, HashSet<ArcView>>();

        public IEnumerable<ArcView> Arcs
        {
            get { return _arcMap.Values; }
        }

        public void Reset(ArcViewMap arcViewMap)
        {
            _arcMap.Clear();
            _arcSet.Clear();

            foreach (var pair in arcViewMap._arcMap) {
                _arcMap.Add(pair.Key, pair.Value);
            }

            foreach (var pair in arcViewMap._arcSet) {
                _arcSet.Add(pair.Key, pair.Value);
            }
        }

        public bool ContainsArc(Point pos, Direction direction)
        {
            return _arcMap.ContainsKey(new PointDir(pos, direction));
        }

        public ICollection<ArcView> GetArcs(Point pos)
        {
            HashSet<ArcView> arcViews;
            if (_arcSet.TryGetValue(pos, out arcViews)) {
                return arcViews;
            }

            arcViews = new HashSet<ArcView>();
            _arcSet.Add(pos, arcViews);
            return arcViews;
        }

        public bool TryGetArc(Point position, Direction direction, out ArcView arcView)
        {
            return _arcMap.TryGetValue(new PointDir(position, direction), out arcView);
        }

        public void Add(Point position, Direction direction, ArcView arcView)
        {
            _arcMap.Add(new PointDir(position, direction), arcView);

            HashSet<ArcView> arcs;
            if (_arcSet.TryGetValue(position, out arcs)) {
                arcs.Add(arcView);
                return;
            }

            arcs = new HashSet<ArcView> {arcView};
            _arcSet.Add(position, arcs);
        }

        public bool Remove(Point pos)
        {
            Directions.All
                .Select(dir => new PointDir(pos, dir))
                .ToList()
                .ForEach(pointDir => _arcMap.Remove(pointDir)); // Remove all possible point directions
            return _arcSet.Remove(pos);
        }

        public void Clear()
        {
            _arcMap.Clear();
            _arcSet.Clear();
        }
    }
}
