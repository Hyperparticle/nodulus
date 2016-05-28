using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class ArcView : MonoBehaviour
    {
        private PuzzleView _puzzleView;

        private Vector3 _initScale;
        private NodeView _invertedNode;

        //private readonly LerpV _moveLerp = new LerpV(TransitionTime, Lerp.SmootherStep);
        //private readonly LerpC _colorLerp = new LerpC(ColorTransitionTime, Lerp.SmootherStep);

        public Arc Arc { get; private set; }

        private bool _highlighted;
        public bool Highlight
        {
            get { return _highlighted; }

            set
            {
                _highlighted = value;

                //var color = _highlighted ? _puzzleView.ArcHighlightColor : _puzzleView.ArcColor;
                //_colorLerp.Begin(_renderMaterial.color, color);
            }
        }

        public void Init(Arc arc)
        {
            Arc = arc;
            transform.rotation = arc.Direction.Rotation();

            //_puzzleView = Arc.ParentNode.NodeScript.GetComponentInParent<PuzzleView>();

            Highlight = false;
        }

        void Start()
        {
            _initScale = transform.localScale;

            transform.localPosition = Vector3.zero;


            transform.localScale = BoardScale;
            transform.localPosition = BoardPosition;
        }

        void Update()
        {
            //if (_colorLerp.Transitioning)
            //    _renderMaterial.color = _colorLerp.Update();

            //if (_moveLerp.Transitioning)
            //    transform.localPosition = _moveLerp.Update();
        }

        public void Pull(Direction direction)
        {
            //_invertedNode = Arc.Root(direction).NodeScript;
            transform.parent = _invertedNode.Rotor;

            _invertedNode.Pull(this, direction);
        }

        public void Push(FieldView field, bool opposite)
        {
            var node = opposite ? field.ConnectedNode : field.ParentNode;

            node.Push(this, field, opposite);
        }

        public void MoveTo(Vector3 pos)
        {
        }

        private Vector3 BoardPosition
        {
            get
            {
                var pos = Arc.Direction.Vector() * Arc.Length / 2;
                return pos * _puzzleView.Scaling;
            }
        }

        private Vector3 BoardScale
        {
            get
            {
                var lengthScale = new Vector3(Arc.Length * _puzzleView.Scaling, 1, 1);
                lengthScale -= Vector3.right * _puzzleView.NodeScaling;
                return Vector3.Scale(_initScale, lengthScale);
            }
        }
    }
}
