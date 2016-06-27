using System.Runtime.InteropServices;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class Colorizer : MonoBehaviour
    {
        private const float DarkBrightnessScale = 0.40f;

        public Color PrimaryColor
        {
            get { return _primaryColor; }
            set {
                _primaryColor = value;
                CalculateColors();
            }
        }

        private Color _primaryColor;
        private Color _invisibleColor;

        private HsbColor _primaryHsb;
        private HsbColor _darkHsb;

        private Material _material;

        private bool _highlighted = true;
        private bool _visible = true;

        void Awake()
        {
            _material = GetComponent<Renderer>().material;
            CalculateColors();
        }

        private void CalculateColors()
        {
            // Precalculate colors here
            var hsb = _primaryHsb = new HsbColor(PrimaryColor);

            _darkHsb = new HsbColor(hsb.h, hsb.s, hsb.b*DarkBrightnessScale, hsb.a);

            var invisibleHsb = new HsbColor(hsb.h, hsb.s, hsb.b, 0f);
            _invisibleColor = invisibleHsb.ToColor();
        }

        public void Highlight(bool immediate = false)
        {
            if (_highlighted && !immediate) {
                return;
            }

            _highlighted = true;
            var time = immediate ? 0f : 0.5f;

            LeanTween.value(gameObject, _darkHsb.b, _primaryHsb.b, time)
                .setOnUpdate(b => { _material.color = Brightness(_primaryColor, b); })
                .setEase(LeanTweenType.easeInOutSine);
        }

        public void Darken(bool immediate = false)
        {
            if (!_highlighted && !immediate) {
                return;
            }

            _highlighted = false;
            var time = immediate ? 0f : 0.5f;

            LeanTween.value(gameObject, _primaryHsb.b, _darkHsb.b, time)
                .setOnUpdate(b => { _material.color = Brightness(_primaryColor, b); })
                .setEase(LeanTweenType.easeInOutSine); ;
        }

        public void SetVisible(bool immediate = false)
        {
            if (_visible && !immediate) {
                return;
            }

            _visible = true;
            var time = immediate ? 0f : 0.5f;

            LeanTween.value(gameObject, _invisibleColor.a, _primaryColor.a, time)
                .setOnUpdate(a => { _material.color = Alpha(_primaryColor, a); })
                .setEase(LeanTweenType.easeInOutSine);
        }

        public void SetInvisible(bool immediate = false)
        {
            if (!_visible && !immediate) {
                return;
            }

            _visible = false;
            var time = immediate ? 0f : 0.5f;

            LeanTween.value(gameObject, _primaryColor.a, _invisibleColor.a, time)
                .setOnUpdate(a => { _material.color = Alpha(_primaryColor, a); })
                .setEase(LeanTweenType.easeInOutSine);
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
