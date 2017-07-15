using UnityEngine;
using Utility;
using View.Control;

namespace View.Items
{
    public class Colorizer : MonoBehaviour
    {
        // Defaults
        public float DarkBrightnessScale { get { return GameDef.Get.DarkBrightnessScale; } }
        public float TransitionTime { get { return GameDef.Get.ColorTransitionTime; } }
        public LeanTweenType Ease { get { return GameDef.Get.ColorEase; } }

        public Color Primary;

        public Color PrimaryColor
        {
            get { return _primaryColor; }
            set {
                _primaryColor = value;
                _primaryHsb = new HsbColor(_primaryColor);
                _darkHsb = new HsbColor(_primaryHsb.H, _primaryHsb.S, _primaryHsb.B*DarkBrightnessScale, _primaryHsb.A);
                CurrentColor = _primaryColor;
            }
        }

        private Color CurrentColor
        {
            get { return _material.color; }
            set { _material.color = value; }
        }

        private HsbColor CurrentHsb
        {
            get { return new HsbColor(_material.color); }
            set { _material.color = value.ToColor(); }
        }

        private Color _primaryColor;
        private Color _previousColor;

        private HsbColor _primaryHsb;
        private HsbColor _darkHsb;

        private Material _material;

        private void Awake()
        {
            _material = GetComponent<Renderer>().material;
        }

        public void Highlight()
        {
            Highlight(TransitionTime, 0f, Ease);
        }

        public void Highlight(float time)
        {
            Highlight(time, 0f, Ease);
        }

        public void Highlight(float time, float delay, LeanTweenType ease)
        {
            _previousColor = CurrentColor;

            if (time < float.Epsilon) {
                CurrentColor = Brightness(CurrentColor, _primaryHsb.B);
                return;
            }

            LeanTween.value(gameObject, CurrentHsb.B, _primaryHsb.B, time)
                .setOnUpdate(b => CurrentColor = Brightness(CurrentColor, b))
                .setDelay(delay)
                .setEase(ease);
        }

        public void Darken()
        {
            Darken(TransitionTime, 0f, Ease);
        }

        public void Darken(float time)
        {
            Darken(time, 0f, Ease);
        }

        public void Darken(float time, float delay, LeanTweenType ease)
        {
            _previousColor = CurrentColor;

            if (time < float.Epsilon) {
                CurrentColor = Brightness(CurrentColor, _darkHsb.B);
                return;
            }

            LeanTween.value(gameObject, CurrentHsb.B, _darkHsb.B, time)
                .setOnUpdate(b => CurrentColor = Brightness(CurrentColor, b))
                .setDelay(delay)
                .setEase(ease);
        }

        public void Appear()
        {
            Appear(TransitionTime, 0f, Ease);
        }

        public void Appear(float time)
        {
            Appear(time, 0f, Ease);
        }

        public void Appear(float time, float delay, LeanTweenType ease)
        {
            _previousColor = CurrentColor;

            if (time < float.Epsilon) {
                CurrentColor = Alpha(CurrentColor, _primaryColor.a);
                return;
            }

            LeanTween.alpha(gameObject, _primaryColor.a, time)
                .setDelay(delay)
                .setEase(ease);
        }

        public void Fade()
        {
            Fade(TransitionTime, 0f, Ease);
        }

        public void Fade(float time)
        {
            Fade(time, 0f, Ease);
        }

        public void Fade(float time, float delay, LeanTweenType ease)
        {
            _previousColor = CurrentColor;

            if (time < float.Epsilon) {
                CurrentColor = Alpha(CurrentColor, 0f);
                return;
            }

            LeanTween.alpha(gameObject, 0f, time)
                .setDelay(delay)
                .setEase(ease);
        }

        public void Previous()
        {
            Previous(TransitionTime, 0f, Ease);
        }

        public void Previous(float time)
        {
            Previous(time, 0f, Ease);
        }

        public void Previous(float time, float delay, LeanTweenType ease)
        {
            LeanTween.color(gameObject, _previousColor, time)
                .setDelay(delay)
                .setEase(ease);
        }

        private static Color Alpha(Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        private static Color Brightness(Color color, float b)
        {
            return new HsbColor(color) {B = b}.ToColor();
        }
    }
}


//private void Color(Colorize opt, )
//{
//    _previousColor = _material.color;

//    LeanTween.value(gameObject, _darkHsb.b, _primaryHsb.b, time)
//         .setDelay(delay)
//         .setOnUpdate(b => _material.color = Brightness(_primaryColor, b))
//         .setEase(ease);
//}

//private IDictionary<Colorize, Action<float, float, LeanTweenType, float, float>> _colorActions = 
//    new Dictionary<Colorize, Action<float, float, LeanTweenType, float, float>> {
//    {
//        Colorize.Highlight,
//        (time, delay, ease, from, to) => LeanTween.value(gameObject, from, to, time)
//            .setDelay(delay)
//            .setOnUpdate(b => _material.color = Brightness(_primaryColor, b))
//            .setEase(ease)
//    }
//};
///// <summary>
///// Colorizing options
///// </summary>
//public enum Colorize
//{
//    Highlight, Darken, Appear, Fade
//}

//public static class ColorizeExt
//{
//    public static void Color(this Colorize opt)
//    {

//    } 
//}
