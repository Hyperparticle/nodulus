using UnityEngine;

namespace View.Control
{
    public class CameraScript : MonoBehaviour
    {
        public static float Width => Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
        public static float Height => Camera.main.orthographicSize * 2.0f;

        public static Vector2 CameraDimensions {
            get
            {
                var cam = Camera.main;
                var p1 = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
                var p2 = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
                var p3 = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

                var width = (p2 - p1).magnitude;
                var height = (p3 - p2).magnitude;

                return new Vector2(width, height);
            }
        }

        public static void FitToDimensions(float marginScale, Vector2 dimensions)
        {
            var cameraDimensions = CameraDimensions;
			
            // Calculate dimensions of the game board + a small margin to prevent cutoff around the edges,
            // then calculate a scaled zoom value based on the ratio of the board dimensions to the camera dimensions
            // so that the board never gets cut off by the camera
            var margin = marginScale * new Vector2(2f, 3f);
            var scaledDimensions = dimensions + margin;
            var cameraZoomScale = new Vector2(
                scaledDimensions.x / cameraDimensions.x, 
                scaledDimensions.y / cameraDimensions.y
            );
            var cameraZoom = Camera.main.orthographicSize * cameraZoomScale;
            var maxZoom = Mathf.Max(cameraZoom.x, cameraZoom.y);

            LeanTween.value(Camera.main.orthographicSize, maxZoom, GameDef.Get.WaveInMoveDelayStart)
                .setEase(LeanTweenType.easeInOutSine)
                .setDelay(GameDef.Get.LevelDelay)
                .setOnUpdate(scaled => Camera.main.orthographicSize = scaled);
        }
    }
}
