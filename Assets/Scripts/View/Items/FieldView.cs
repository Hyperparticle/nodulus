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
        private Renderer _renderer;
//        private Light _light;

        private Vector3 _initScale;
        private Color _initEmissionColor;
        
        private int _scalePulseId;
        private int _emissionPulseId;
//        private int _lightPulseId;

        public Field Field { get; private set; }

        public NodeView ParentNode { get; private set; }
        public NodeView ConnectedNode { get; private set; }

        public Vector2 HitRect => new Vector3(transform.localScale.x, 1) + transform.localPosition;

        private void Awake()
        {
            _fieldScale = GetComponent<ScaleScript>();
            _colorizer = GetComponent<Colorizer>();
            _renderer = GetComponent<Renderer>();
//            _light = GetComponent<Light>();
        }

        public void Init(Field field, NodeView parent, NodeView connected)
        {
            Field = field;
            ParentNode = parent;
            ConnectedNode = connected;

            _fieldScale.SetField(field);

            _colorizer.PrimaryColor = FieldColor;
            _colorizer.Fade(0f);
            
            _initScale = transform.localScale;
            _initEmissionColor = _renderer.material.color;
        }

        public void Highlight(bool enable)
        {
            // TODO: make configurable
            const float time = 0.6f;
            
            if (enable) {
                _colorizer.PulseAppear(time);
                PulseScale(time);
            } else {
                LeanTween.cancel(gameObject);
                LeanTween.cancel(_emissionPulseId);
//                LeanTween.cancel(_lightPulseId);

//                LeanTween.value(_light.intensity, 0f, time / 2f)
//                    .setEase(LeanTweenType.easeInOutSine)
//                    .setOnUpdate(value => _light.intensity = value);
                
                _colorizer.Fade(() => {
                    transform.localScale = _initScale;
                });
            }
        }
        
        private void PulseScale(float time)
        {
            if (LeanTween.isTweening(_scalePulseId)) {
                return;
            }
            
            // TODO: make configurable
            const float scale = 0.05f;
            
            _scalePulseId = LeanTween.scale(gameObject, _initScale + Vector3.one * scale, time)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(-1)
                .id;

            // TODO: magic numbers
            var color = _initEmissionColor;
            _emissionPulseId = LeanTween.value(0.7f, 1.1f, time)
                .setOnUpdate((float value) => {
                    if (_renderer == null) { return; }
                    _renderer.material?.SetColor("_EmissionColor", color * value);
                })
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(-1)
                .id;

//            _lightPulseId = LeanTween.value(0f, 2f, time) // TODO: magic numbers
//                .setOnUpdate(value => {
//                    if (_light == null) { return; }
//                    _light.intensity = value;
//                })
//                .setEase(LeanTweenType.easeInOutSine)
//                .setLoopPingPong(-1)
//                .id;
        }

        private void PulseIntensity(float time)
        {
            
        }
    }
}
