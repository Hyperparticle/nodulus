using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class ArcView : MonoBehaviour
    {
        private PuzzleView _puzzleView;
        private ScaleScript _arcScale;
        private Colorizer _colorizer;

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

        public void Init(Arc arc, bool inStartIsland)
        {
            _puzzleView = PuzzleView.Get();
            _arcScale = GetComponent<ScaleScript>();
            _colorizer = GetComponent<Colorizer>();

            Arc = arc;

            //Highlight = false;

            _arcScale.SetArc(arc);

            if (!inStartIsland) { _colorizer.Darken(); }
        }

        void Start()
        {
            
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

    }
}
