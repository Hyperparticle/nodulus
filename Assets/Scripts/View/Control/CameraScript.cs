using UnityEngine;

namespace View.Control
{
    /// <summary>
    /// Handles basic camera zooming.
    /// </summary>
    public class CameraScript : MonoBehaviour
    {
        private static int _zoomId;
        
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

        public static float CameraZoomToFit(Vector2 dimensions, Vector2 margin, Vector2 scaleRatio)
        {
            var cameraDimensions = CameraDimensions;
			
            // Calculate dimensions of the game board + a small margin to prevent cutoff around the edges,
            // then calculate a scaled zoom value based on the ratio of the board dimensions to the camera dimensions
            // so that the board never gets cut off by the camera
            var scaledDimensions = dimensions + margin;
            var cameraZoomScale = new Vector2(
                scaledDimensions.x / scaleRatio.x / cameraDimensions.x,
                scaledDimensions.y / scaleRatio.y / cameraDimensions.y
            );
            
            var cameraZoom = Camera.main.orthographicSize * cameraZoomScale;
            var maxZoom = Mathf.Max(cameraZoom.x, cameraZoom.y);

            return maxZoom;
        }

        public static void FitToDimensions(Vector2 dimensions, Vector2 margin,
            LeanTweenType tweenType = LeanTweenType.easeInOutSine)
        {
            FitToDimensions(dimensions, margin, GameDef.Get.WaveInMoveDelayStart, tweenType);
        }
        
        public static void FitToDimensions(Vector2 dimensions, Vector2 margin, float time,
            LeanTweenType tweenType = LeanTweenType.easeInOutSine)
        {
            var zoom = CameraZoomToFit(dimensions, margin, Vector2.one);
            ZoomCamera(zoom, time, tweenType);
        }

        public static int ZoomCamera(float zoom, float time, LeanTweenType tweenType = LeanTweenType.easeInOutSine)
        {
            if (LeanTween.isTweening(_zoomId)) {
                LeanTween.cancel(_zoomId);
            }
            
            _zoomId = LeanTween.value(Camera.main.orthographicSize, zoom, time)
                .setEase(tweenType)
                .setDelay(GameDef.Get.LevelDelay)
                .setOnUpdate(scaled => Camera.main.orthographicSize = scaled)
                .id;

            return _zoomId;
        }
    }
}
