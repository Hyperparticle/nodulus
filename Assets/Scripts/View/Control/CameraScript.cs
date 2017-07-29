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
                var p3 = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));

                var width = (p2 - p1).magnitude;
                var height = (p3 - p1).magnitude;

                return new Vector2(width, height);
            }
        }

        public static void FitToDimensions(Vector2 dimensions, Vector2 margin, 
            LeanTweenType tweenType = LeanTweenType.easeInOutSine)
        {
            var cameraDimensions = CameraDimensions;
			
            // Calculate dimensions of the game board + a small margin to prevent cutoff around the edges,
            // then calculate a scaled zoom value based on the ratio of the board dimensions to the camera dimensions
            // so that the board never gets cut off by the camera
            var scaledDimensions = dimensions + margin;
            var cameraZoomScale = new Vector2(
                scaledDimensions.x / cameraDimensions.x,
                scaledDimensions.y / cameraDimensions.y
            );
            
            var cameraZoom = Camera.main.orthographicSize * cameraZoomScale;
            var maxZoom = Mathf.Max(cameraZoom.x, cameraZoom.y);

            ZoomCamera(maxZoom, GameDef.Get.WaveInMoveDelayStart, tweenType);
        }

        public static void ZoomCamera(float zoom, float time, LeanTweenType tweenType = LeanTweenType.easeInOutSine)
        {
            LeanTween.value(Camera.main.orthographicSize, zoom, time)
                .setEase(tweenType)
                .setDelay(GameDef.Get.LevelDelay)
                .setOnUpdate(scaled => Camera.main.orthographicSize = scaled);
        }

        public static float CameraFitScale(Vector2 dimensions, Vector2 scaleRatio)
        {
            return CameraFitScale(dimensions, scaleRatio, Camera.main.orthographicSize);
        }
        
        public static float CameraFitScale(Vector2 dimensions, Vector2 scaleRatio, float orthographicSize)
        {
            var cameraDimensions = CameraDimensions * orthographicSize / Camera.main.orthographicSize;
			
            var scaledDimensions = dimensions;
            var zoomScale = new Vector2(
                cameraDimensions.x * scaleRatio.x / scaledDimensions.x,
                cameraDimensions.y * scaleRatio.y / scaledDimensions.y
            );
            
            var minZoom = Mathf.Min(zoomScale.x, zoomScale.y);

            return minZoom;
        }
    }
}
