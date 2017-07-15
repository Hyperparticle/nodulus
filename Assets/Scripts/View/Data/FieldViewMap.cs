using System.Collections.Generic;
using Core.Data;
using View.Items;

namespace View.Data
{
    public class FieldViewMap
    {
        private readonly IDictionary<PointDir, FieldView> _fieldMap = new Dictionary<PointDir, FieldView>();
        public IEnumerable<FieldView> Fields { get { return _fieldMap.Values; } }

        public void Reset(FieldViewMap fieldViewMap)
        {
            _fieldMap.Clear();
            foreach (var pair in fieldViewMap._fieldMap) {
                _fieldMap.Add(pair.Key, pair.Value);
            }
        }

        public bool TryGetField(Point pos, Direction dir, out FieldView fieldView)
        {
            return _fieldMap.TryGetValue(new PointDir(pos, dir), out fieldView);
        }

        public void Add(Point pos, Direction dir, FieldView fieldView)
        {
            _fieldMap.Add(new PointDir(pos, dir), fieldView);
        }

        public void Clear()
        {
            _fieldMap.Clear();
        }

        public FieldView this[PointDir pointDir]
        {
            get { return _fieldMap[pointDir]; }
        }
    }
}
