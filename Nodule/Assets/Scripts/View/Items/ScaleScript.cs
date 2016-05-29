using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class ScaleScript : MonoBehaviour
    {
        private PuzzleScale _puzzleScale;
            
        public void SetNode(Node node)
        {
            _puzzleScale = PuzzleScale.Get();

            transform.localPosition = (Vector3) node.Position * _puzzleScale.Scaling;
            transform.localScale = Vector3.one * _puzzleScale.NodeScaling;
            transform.localRotation = Quaternion.identity;
        }

        public void SetArc(Arc arc)
        {
            _puzzleScale = PuzzleScale.Get();

            var arcPos = arc.Direction.Vector() * arc.Length / 2;
            var lengthScale = new Vector3(arc.Length*_puzzleScale.Scaling, 1, 1);
            lengthScale -= Vector3.right * _puzzleScale.NodeScaling;

            transform.localPosition = arcPos * _puzzleScale.Scaling;
            transform.localScale = Vector3.Scale(transform.localScale, lengthScale);
            transform.localRotation = arc.Direction.Rotation();
        }

        public void SetField(Field field)
        {
            _puzzleScale = PuzzleScale.Get();

            var fieldPos = field.Direction.Vector() * field.Length / 2;
            var lengthScale = new Vector3(field.Length * _puzzleScale.Scaling, 1, 1)
                - Vector3.right * _puzzleScale.NodeScaling;

            transform.localPosition = fieldPos * _puzzleScale.Scaling;
            transform.localScale = lengthScale;
            transform.localRotation = field.Direction.Rotation();
        }
    }
}
