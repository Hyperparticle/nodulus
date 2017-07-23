using UnityEngine;

namespace View.Control
{
    public class CameraScript : MonoBehaviour
    {
        public static float Width => Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
        public static float Height => Camera.main.orthographicSize * 2.0f;

        public static float Fit(Vector2 dimensions, float padding = 0)
        {
            var wScale = (Width - padding) / dimensions.x;
            var hScale = (Height - padding) / dimensions.y;

            return Mathf.Min(wScale, hScale);
        }

        public static float Fit(Vector2 dimensions, float widthPadding, float heightPadding)
        {
            var wScale = (Width - widthPadding) / dimensions.x;
            var hScale = (Height - heightPadding) / dimensions.y;

            return Mathf.Min(wScale, hScale);
        }
    }
}
