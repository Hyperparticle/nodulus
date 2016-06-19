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

        void Start()
        {
            // Precalculate colors here
            var hsb = new HsbColor(PrimaryColor);

            var darkHsb = new HsbColor(hsb.h, hsb.s, hsb.b * DarkBrightnessScale, hsb.a);
            _darkColor = darkHsb.ToColor();

            var invisibleHsb = new HsbColor(hsb.h, hsb.s, hsb.b, 0f);
            _invisibleColor = invisibleHsb.ToColor();

            _renderer = GetComponent<Renderer>();

            Highlight();
        }

        public void Highlight()
        {
            _renderer.material.color = PrimaryColor;
        }

        public void Darken()
        {
            _renderer.material.color = _darkColor;
        }

        public void SetInvisible()
        {
            _renderer.material.color = _invisibleColor;
        }

        public void SetSecondary()
        {
            _renderer.material.color = SecondaryColor;
        }
    }
}
