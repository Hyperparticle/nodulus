using Assets.Scripts.Utility;
using Assets.Scripts.View.Items;

namespace Assets.Scripts.Core.Data
{
    public class NodeViewMap
    {
        private readonly MultiMap<Point, Direction, NodeView> _nodeViewMap = new MultiMap<Point, Direction, NodeView>();


    }
}
