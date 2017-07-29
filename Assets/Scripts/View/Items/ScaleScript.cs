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
            var lengthScale = new Vector3(arc.Length * _puzzleScale.Scaling, puzzleLocalScale.y, puzzleLocalScale.z)
                - Vector3.right * _puzzleScale.NodeScaling;

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
            var lengthScale = new Vector3(field.Length * _puzzleScale.Scaling, puzzleLocalScale.y, puzzleLocalScale.z)
                - Vector3.right * _puzzleScale.NodeScaling;

            transform.localPosition = fieldPos * _puzzleScale.Scaling;
            transform.localScale = Vector3.Scale(transform.localScale, lengthScale);
            transform.localRotation = field.Direction.Rotation();

            Length = field.Length / 2f * _puzzleScale.Scaling;
        }
    }
}
