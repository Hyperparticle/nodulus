using System.Runtime.InteropServices;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class Colorizer : MonoBehaviour
    {
        private const float DarkBrightnessScale = 0.40f;

        public Color PrimaryColor;

        private Color _darkColor;
        private Color _invisibleColor;
        private Renderer _renderer;

        private bool _highlighted = true;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();

            // Precalculate colors here
            var hsb = new HsbColor(PrimaryColor);

            var darkHsb = new HsbColor(hsb.h, hsb.s, hsb.b*DarkBrightnessScale, hsb.a);
            _darkColor = darkHsb.ToColor();

            var invisibleHsb = new HsbColor(hsb.h, hsb.s, hsb.b, 0f);
            _invisibleColor = invisibleHsb.ToColor();
        }

        public void Highlight(bool immediate = false)
        {
            if (_highlighted) {
                return;
            }

            _highlighted = true;
            ColorThis(PrimaryColor, immediate);
        }

        public void Darken(bool immediate = false)
        {
            if (!_highlighted) {
                return;
            }

            _highlighted = false;
            ColorThis(_darkColor, immediate);
        }

        public void SetInvisible(bool immediate = false)
        {
            _highlighted = false;
            ColorThis(_invisibleColor, immediate);
        }

        public void ColorThis(Color color, bool immediate)
        {
            LeanTween.color(gameObject, color, immediate ? 0f : 0.5f)
                .setEase(LeanTweenType.easeInOutSine);
        }
    }
}
