using Core.Items;
using UnityEngine;
using View.Control;

namespace View.Items
{
    /// <summary>
    /// A FieldView represents the view for an field in the gameboard. It is responsible
    /// for visualizing fields that highlight the moves that are available.
    /// </summary>
    public class FieldView : MonoBehaviour
    {
        public Color FieldColor => GameDef.Get.FieldColor;

        private ScaleScript _fieldScale;
        private Colorizer _colorizer;

        public Field Field { get; private set; }

        public NodeView ParentNode { get; private set; }
        public NodeView ConnectedNode { get; private set; }

        public Vector2 HitRect => new Vector3(transform.localScale.x, 1) + transform.localPosition;

        private void Awake()
        {
            _fieldScale = GetComponent<ScaleScript>();
            _colorizer = GetComponent<Colorizer>();
        }

        public void Init(Field field, NodeView parent, NodeView connected)
        {
            Field = field;
            ParentNode = parent;
            ConnectedNode = connected;

            _fieldScale.SetField(field);

            _colorizer.PrimaryColor = FieldColor;
            _colorizer.Fade(0f);
        }

        public void Highlight(bool enable)
        {
            if (enable) {
                _colorizer.Appear();
            } else {
                _colorizer.Fade();
            }
        }
    }
}
