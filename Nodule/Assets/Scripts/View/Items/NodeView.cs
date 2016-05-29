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
        private ScaleScript _nodeScale;
        private Colorizer _colorizer;

        public Node Node { get; private set; }


        //private bool _highlighted;
        //public bool Highlight
        //{
        //    get { return _highlighted; }

        //    set
        //    {
        //        _highlighted = value;
        //        if (Node.Final) return;

        //        //var color = _highlighted ? _puzzleView.NodeHighlightColor : _puzzleView.NodeColor;
        //    }
        //}

        public void Init(Node node, bool inStartIsland)
        {
            _puzzleView = PuzzleView.Get();
            _nodeScale = GetComponent<ScaleScript>();
            _colorizer = GetComponentInChildren<Colorizer>();

            Node = node;

            _nodeScale.SetNode(node);

            if (!inStartIsland) { _colorizer.Darken(); }
            if (node.Final) { _colorizer.SetSecondary(); }
        }


        public void Pull(ArcView arc, Direction direction)
        {
            SetRotor(arc);

            // Find the axis to rotate about (dependent on arc direction and rotor rotation),
            // and rotate a relative 90 degrees about that axis

            //var rotation = NextRotation(direction, true);
            //_rotationLerp.Begin(Rotor.localRotation, rotation);
        }

        public void Push(ArcView arc, FieldView field, bool opposite)
        {
            //SetRotor(arc);

            //// Set the arc's parent and position of the new rotor
            //// Push the arc in the correct direction (in front of the node)

            //var inverse = Quaternion.Inverse(Rotor.localRotation);

            //var dir = inverse*Vector3.back;
            //var pos = (dir * field.Field.Length / 2) * _puzzleView.Scaling;
            //arc.MoveTo(pos);

            //// Find the axis to rotate about (dependent on arc direction and rotor rotation),
            //// and rotate a relative -90 degrees about that axis

            //var close = (pos - arc.transform.localPosition).magnitude < 0.1f;

            //var fieldDirection = field.Field.Direction;
            //fieldDirection = opposite ? fieldDirection.Opposite() : fieldDirection;
            //var rotation = NextRotation(fieldDirection, false);
            //_rotationLerp.Begin(Rotor.localRotation, rotation, close ? 0f : ArcView.TransitionTime);
        }

        private Quaternion NextRotation(Direction fieldDirection, bool forward)
        {
            var degrees = forward ? 90 : -90;

            var inverse = Quaternion.Inverse(Rotor.localRotation);
            var axis = inverse * Quaternion.Euler(0, 0, 90) * fieldDirection.Vector();
            var rotation = Rotor.localRotation * Quaternion.AngleAxis(degrees, axis);
            return rotation;
        }

        private void SetRotor(ArcView arc)
        {
            //foreach (var other in transform.GetComponentsInChildren<ArcView>())
            //{
            //    other.transform.parent = transform;
            //}

            //var direction = arc.Arc.Direction.Rotated(1);
            //foreach (var perp in Node.Connections
            //    .Where(connection => connection.Direction == direction || connection.Direction == direction.Opposite()))
            //{
            //    perp.Arc.ArcScript.transform.parent = Rotor;
            //}

            //arc.transform.parent = Rotor;
        }
    }
}

