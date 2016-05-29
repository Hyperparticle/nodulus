using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class FieldView : MonoBehaviour
    {
        private PuzzleView _puzzleView;
        private ScaleScript _fieldScale;
        private Colorizer _colorizer;

        public Field Field { get; private set; }

        public NodeView ParentNode { get; private set; }
        public NodeView ConnectedNode { get; private set; }

        //private Renderer _renderer;

        //private Vector3 _initScale;

        public void Init(Field field, NodeView parent, NodeView connected)
        {
            _puzzleView = PuzzleView.Get();
            _fieldScale = GetComponent<ScaleScript>();
            _colorizer = GetComponent<Colorizer>();

            Field = field;
            ParentNode = parent;
            ConnectedNode = connected;

            _fieldScale.SetField(field);
            _colorizer.SetInvisible();
        }

        void Start()
        {
            //_initScale = transform.localScale;

            //_renderer = GetComponent<Renderer>();

            //_renderer.material.color = _puzzleView.FieldColor;
        }

        void OnMouseDown()
        {
            _puzzleView.Push(this);
        }

        public void Highlight(bool enable)
        {
            //if (enable == _enable) return;
            //_enable = enable;

            ////_material.SetFloat(ControllerScript.GlowPowerName, enable ? GlowPower : 0);

            //var start = _puzzleView.FieldHighlightColor;
            //var end = _puzzleView.FieldColor;
            //Pair.Swap(ref start, ref end, enable);

            //_colorLerp.Begin(start, esd);
        }
    }
}
