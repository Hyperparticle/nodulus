using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class FieldView : MonoBehaviour
    {
        private const float GlowPower = 0.1f;

        private PuzzleView _puzzleView;

        private const float TransitionTime = 0.33f;
        private readonly LerpQ _rotationLerp = new LerpQ(TransitionTime, Lerp.SmootherStep);
        private readonly LerpC _colorLerp = new LerpC(TransitionTime, Lerp.SmootherStep);

        public Field Field { get; private set; }
        public NodeView ParentNode { get; private set; }
        public NodeView ConnectedNode { get; private set; }
        public bool Transitioning { get { return _rotationLerp.Transitioning || _colorLerp.Transitioning; } }

        private Renderer _renderer;
        private bool _enable;

        private Vector3 _initScale;

        public void Init(Field field, NodeView parent, NodeView connected)
        {
            Field = field;
            ParentNode = parent;
            ConnectedNode = connected;
        
            transform.localRotation = Field.Direction.Rotation();
        }

        void Start()
        {
            _puzzleView = ParentNode.GetComponentInParent<PuzzleView>();
            _initScale = transform.localScale;

            _renderer = GetComponent<Renderer>();
        
            transform.localScale = BoardScale;
            transform.localPosition = BoardPosition;

            _renderer.material.color = _puzzleView.FieldColor;
        }

        void Update()
        {
            if (_colorLerp.Transitioning)
                _renderer.material.color = _colorLerp.Update();

            if (_rotationLerp.Transitioning) 
                transform.localRotation = _rotationLerp.Update();
        }

        void OnMouseDown()
        {
            _puzzleView.Revert(this);
        }

        private Vector3 BoardPosition
        {
            get
            {
                var pos = Field.Direction.Vector() * Field.Length / 2;
                return pos * _puzzleView.Scaling;
            }
        }

        private Vector3 BoardScale
        {
            get
            {
                var lengthScale = new Vector3(Field.Length * _puzzleView.Scaling, 1, 1);
                lengthScale -= Vector3.right * _puzzleView.NodeScaling;
                return Vector3.Scale(_initScale, lengthScale);
            }
        }

        public void Highlight(bool enable)
        {
            if (enable == _enable) return;
            _enable = enable;

            //_material.SetFloat(ControllerScript.GlowPowerName, enable ? GlowPower : 0);

            var start = _puzzleView.FieldHighlightColor;
            var end = _puzzleView.FieldColor;
            Pair.Swap(ref start, ref end, enable);

            _colorLerp.Begin(start, end);
        }
    }
}
