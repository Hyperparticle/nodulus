using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Utility;
using Assets.Scripts.View.Items;

namespace Assets.Scripts.View.Data
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

        public bool TryGetField(Point position, Direction direction, out FieldView fieldView)
        {
            return _fieldMap.TryGetValue(new PointDir(position, direction), out fieldView);
        }

        public void Add(Point position, Direction direction, FieldView fieldView)
        {
            _fieldMap.Add(new PointDir(position, direction), fieldView);
        }

        public void Clear()
        {
            _fieldMap.Clear();
        }
    }
}
