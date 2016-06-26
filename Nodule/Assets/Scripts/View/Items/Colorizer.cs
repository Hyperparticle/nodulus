using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class Colorizer : MonoBehaviour
    {
        private const float DarkBrightnessScale = 0.40f;

        public Color PrimaryColor;
        public Color SecondaryColor;

        private Color _darkColor;
        private Color _invisibleColor;

        private Renderer _renderer;

        private bool _highlighted;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();

            // Precalculate colors here
            var hsb = new HsbColor(PrimaryColor);

            var darkHsb = new HsbColor(hsb.h, hsb.s, hsb.b * DarkBrightnessScale, hsb.a);
            _darkColor = darkHsb.ToColor();

            var invisibleHsb = new HsbColor(hsb.h, hsb.s, hsb.b, 0f);
            _invisibleColor = invisibleHsb.ToColor();

            Highlight(true);
        }

        public void Highlight(bool immediate = false)
        {
            if (_highlighted) {
                return;
            }

            _highlighted = true;
            LeanTween.color(gameObject, PrimaryColor, immediate ? 0f : 0.5f);
        }

        public void Darken(bool immediate = false)
        {
            if (!_highlighted) {
                return;
            }

            _highlighted = false;
            LeanTween.color(gameObject, _darkColor, immediate ? 0f : 0.5f);
        }

        public void SetInvisible(bool immediate = false)
        {
            _highlighted = false;
            LeanTween.color(gameObject, _invisibleColor, immediate ? 0f : 0.5f);
        }

        public void SetSecondary()
        {
            _renderer.material.color = SecondaryColor;
        }
    }
}
