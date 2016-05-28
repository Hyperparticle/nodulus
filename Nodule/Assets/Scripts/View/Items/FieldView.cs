using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class FieldView : MonoBehaviour
    {
        private PuzzleView _puzzleView;

        public Field Field { get; private set; }

        public NodeView ParentNode { get; private set; }
        public NodeView ConnectedNode { get; private set; }

        //private Renderer _renderer;

        //private Vector3 _initScale;

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
            //_initScale = transform.localScale;

            //_renderer = GetComponent<Renderer>();
        
            //transform.localScale = BoardScale;
            transform.localPosition = BoardPosition;

            //_renderer.material.color = _puzzleView.FieldColor;
        }

        void OnMouseDown()
        {
            _puzzleView.Push(this);
        }

        private Vector3 BoardPosition
        {
            get
            {
                var pos = Field.Direction.Vector() * Field.Length / 2;
                return pos * _puzzleView.Scaling;
            }
        }

        //private Vector3 BoardScale
        //{
        //    get
        //    {
        //        var lengthScale = new Vector3(Field.Length * _puzzleView.Scaling, 1, 1);
        //        lengthScale -= Vector3.right * _puzzleView.NodeScaling;
        //        return Vector3.Scale(_initScale, lengthScale);
        //    }
        //}

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
