using Assets.Scripts.Model.Data;

namespace Assets.Scripts.Model.Moves
{
    public class Player
    {
        private readonly Island _island;
       
        public bool Win { get { return _island.IsFinal; } }

        public Player(Island start)
        {
            _island = start;
        }

        //public void MoveTo(Node node)
        //{
            
        //}

        //public bool IsProximal(Field field)
        //{
        //    return HasNode(field.ParentNode) || HasNode(field.ConnectedNode);
        //}

        //private bool HasNode(Node node)
        //{
        //    return _island.Contains(node);
        //}

        //public bool ParentInIsland(Field field)
        //{
        //    return _prevIsland.Contains(field.ParentNode);
        //}
    }
}
