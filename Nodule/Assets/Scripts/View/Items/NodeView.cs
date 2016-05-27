using System.Linq;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class NodeView : MonoBehaviour
    {
        public Transform Rotor;

        private PuzzleView _puzzleView;
        private Material _renderMaterial;

        private const float TransitionTime = 0.33f;
        private readonly LerpQ _rotationLerp = new LerpQ(TransitionTime, Lerp.SmootherStep);
        private readonly LerpC _colorLerp = new LerpC(TransitionTime, Lerp.SmootherStep);

        public Node Node { get; private set; }
        public bool Transitioning { get { return _rotationLerp.Transitioning || _colorLerp.Transitioning; } }

        private Vector3 _initScale;

        private bool _highlighted;
        public bool Highlight
        {
            get { return _highlighted; }

            set
            {
                _highlighted = value;
                if (Node.Final) return;

                var color = _highlighted ? _puzzleView.NodeHighlightColor : _puzzleView.NodeColor;
                _colorLerp.Begin(_renderMaterial.color, color);
            }
        }

        public void Init(Node node)
        {
            _puzzleView = GetComponentInParent<PuzzleView>();
            _renderMaterial = Rotor.GetComponent<Renderer>().material;

            Node = node;

            Highlight = false;
        }

        void Start()
        {
            _initScale = transform.localScale;
        
            SetTransform();
        
            if (!Node.Final) return;
            _renderMaterial.color = _puzzleView.NodeFinalColor;
        }

        void Update()
        {
            if (_colorLerp.Transitioning)
                _renderMaterial.color = _colorLerp.Update();

            if (_rotationLerp.Transitioning)
                Rotor.localRotation = _rotationLerp.Update();
        }

        private void SetTransform()
        {
            transform.localPosition = (Vector3)Node.Position * _puzzleView.Scaling;
            transform.localScale = _initScale * _puzzleView.NodeScaling;
            transform.localRotation = Quaternion.identity;
        }

        public void Invert(ArcView edge, Direction direction)
        {
            if (Transitioning) return;
            SetRotor(edge);

            // Find the axis to rotate about (dependent on edge direction and rotor rotation),
            // and rotate a relative 90 degrees about that axis

            var rotation = NextRotation(direction, true);
            _rotationLerp.Begin(Rotor.localRotation, rotation);
        }

        public void Revert(ArcView edge, FieldView field, bool opposite)
        {
            if (Transitioning) return;
            SetRotor(edge);

            // Set the edge's parent and position of the new rotor
            // Push the edge in the correct direction (in front of the node)

            var inverse = Quaternion.Inverse(Rotor.localRotation);

            var dir = inverse*Vector3.back;
            var pos = (dir * field.Field.Length / 2) * _puzzleView.Scaling;
            edge.MoveTo(pos);

            // Find the axis to rotate about (dependent on edge direction and rotor rotation),
            // and rotate a relative -90 degrees about that axis

            var close = (pos - edge.transform.localPosition).magnitude < 0.1f;

            var fieldDirection = field.Field.Direction;
            fieldDirection = opposite ? fieldDirection.Opposite() : fieldDirection;
            var rotation = NextRotation(fieldDirection, false);
            _rotationLerp.Begin(Rotor.localRotation, rotation, close ? 0f : ArcView.TransitionTime);
        }

        private Quaternion NextRotation(Direction fieldDirection, bool forward)
        {
            var degrees = forward ? 90 : -90;

            var inverse = Quaternion.Inverse(Rotor.localRotation);
            var axis = inverse * Quaternion.Euler(0, 0, 90) * fieldDirection.Vector();
            var rotation = Rotor.localRotation * Quaternion.AngleAxis(degrees, axis);
            return rotation;
        }

        private void SetRotor(ArcView edge)
        {
            foreach (var other in transform.GetComponentsInChildren<ArcView>())
            {
                other.transform.parent = transform;
            }

            var direction = edge.Edge.Direction.Rotated(1);
            foreach (var perp in Node.Connections
                .Where(connection => connection.Direction == direction || connection.Direction == direction.Opposite()))
            {
                perp.Edge.EdgeScript.transform.parent = Rotor;
            }

            edge.transform.parent = Rotor;
        }
    }
}
