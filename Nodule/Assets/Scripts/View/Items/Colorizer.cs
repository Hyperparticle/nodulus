using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class Colorizer : MonoBehaviour
    {
        // Defaults
        private const float DarkBrightnessScale = 0.40f;
        private const float TransitionTime = 0.5f;
        private const LeanTweenType Ease = LeanTweenType.easeInOutSine;

        public Color Primary;

        public Color PrimaryColor
        {
            get { return _primaryColor; }
            set {
                _primaryColor = value;
                _primaryHsb = new HsbColor(_primaryColor);
                _darkHsb = new HsbColor(_primaryHsb.h, _primaryHsb.s, _primaryHsb.b*DarkBrightnessScale, _primaryHsb.a);
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

        void Awake()
        {
            _material = GetComponent<Renderer>().material;
        }

        public void Highlight(float time = TransitionTime, float delay = 0f, LeanTweenType ease = Ease)
        {
            _previousColor = CurrentColor;

            if (time < float.Epsilon) {
                CurrentColor = Brightness(CurrentColor, _primaryHsb.b);
                return;
            }

            LeanTween.value(gameObject, CurrentHsb.b, _primaryHsb.b, time)
                .setOnUpdate(b => CurrentColor = Brightness(CurrentColor, b))
                .setDelay(delay)
                .setEase(ease);
        }

        public void Darken(float time = TransitionTime, float delay = 0f, LeanTweenType ease = Ease)
        {
            _previousColor = CurrentColor;

            if (time < float.Epsilon) {
                CurrentColor = Brightness(CurrentColor, _darkHsb.b);
                return;
            }

            LeanTween.value(gameObject, CurrentHsb.b, _darkHsb.b, time)
                .setOnUpdate(b => CurrentColor = Brightness(CurrentColor, b))
                .setDelay(delay)
                .setEase(ease);
        }

        public void Appear(float time = TransitionTime, float delay = 0f, LeanTweenType ease = Ease)
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

        public void Fade(float time = TransitionTime, float delay = 0f, LeanTweenType ease = Ease)
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

        public void Previous(float time = TransitionTime, float delay = 0f, LeanTweenType ease = Ease)
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
            return new HsbColor(color) {b = b}.ToColor();
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
