using Core.Data;
using Core.Items;
using UnityEngine;
using View.Game;

namespace View.Items
{
    public class ScaleScript : MonoBehaviour
    {
        public float Length { get; private set; }

        private PuzzleScale _puzzleScale;

        public void SetNode(Node node)
        {
            _puzzleScale = GetComponentInParent<PuzzleScale>();
            
            transform.localPosition = (Vector3) node.Position * _puzzleScale.Scaling;
            transform.localScale = Vector3.one * _puzzleScale.NodeScaling;
            transform.localRotation = Quaternion.identity;

            Length = 0f;
        }

        public void SetArc(Arc arc)
        {
            _puzzleScale = GetComponentInParent<PuzzleScale>();
            
            var arcPos = arc.Direction.Vector() * arc.Length / 2;

            var puzzleLocalScale = _puzzleScale.transform.localScale;
            var width = arc.Length * _puzzleScale.Scaling - _puzzleScale.NodeScaling;
            var lengthScale = Vector3.Scale(new Vector3(width, 1f, 1f), puzzleLocalScale);

            transform.localPosition = arcPos * _puzzleScale.Scaling;
            transform.localScale = Vector3.Scale(transform.localScale, lengthScale);
            transform.localRotation = arc.Direction.Rotation();

            Length = arc.Length / 2f * _puzzleScale.Scaling;
        }

        public void SetField(Field field)
        {
            _puzzleScale = GetComponentInParent<PuzzleScale>();
            
            var fieldPos = field.Direction.Vector() * field.Length / 2;
            
            var puzzleLocalScale = _puzzleScale.transform.localScale;
            var width = field.Length * _puzzleScale.Scaling - _puzzleScale.NodeScaling;
            var lengthScale = Vector3.Scale(new Vector3(width, 1f, 1f), puzzleLocalScale);

            transform.localPosition = fieldPos * _puzzleScale.Scaling;
            transform.localScale = Vector3.Scale(transform.localScale, lengthScale);
            transform.localRotation = field.Direction.Rotation();

            Length = field.Length / 2f * _puzzleScale.Scaling;
        }
    }
}
