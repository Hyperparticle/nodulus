using System;
using UnityEngine;
using Utility;
using View.Control;

namespace View.Items
{
    public class Colorizer : MonoBehaviour
    {
        // Defaults
        public float DarkBrightnessScale => GameDef.Get.DarkBrightnessScale;

        public float TransitionTime => GameDef.Get.ColorTransitionTime;
        public LeanTweenType Ease => GameDef.Get.ColorEase;

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

        public void Fade(Action onComplete = null)
        {
            Fade(TransitionTime, 0f, Ease, onComplete);
        }

        public void Fade(float time, Action onComplete = null)
        {
            Fade(time, 0f, Ease, onComplete);
        }

        public void Fade(float time, float delay, LeanTweenType ease, Action onComplete = null)
        {
            onComplete = onComplete ?? (() => {});
            _previousColor = CurrentColor;

            if (time < float.Epsilon) {
                CurrentColor = Alpha(CurrentColor, 0f);
                return;
            }
            
            LeanTween.alpha(gameObject, 0f, time)
                .setDelay(delay)
                .setEase(ease)
                .setOnComplete(onComplete);
        }

        public void PulseAppear(float time)
        {
            _previousColor = CurrentColor;

            LeanTween.alpha(gameObject, _primaryColor.a + 0.1f, time)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(() => {
                    LeanTween.alpha(gameObject, _primaryColor.a, time)
                        .setEase(LeanTweenType.easeInOutSine)
                        .setLoopPingPong(-1);
                });
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

        public static Color Alpha(Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        public static Color Brightness(Color color, float b)
        {
            return new HsbColor(color) {B = b}.ToColor();
        }
    }
}
