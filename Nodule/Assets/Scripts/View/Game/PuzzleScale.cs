using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Control;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    public class PuzzleScale : MonoBehaviour
    {
        // Constants
        public float Scaling { get { return GameDef.Get.Scaling; } }
        public float NodeScaling { get { return GameDef.Get.NodeScaling; } }
        public float EdgeScaling { get { return GameDef.Get.EdgeScaling; } } 
        public float BoardScaling { get { return GameDef.Get.BoardScaling; } }
        public float BoardPadding { get { return GameDef.Get.BoardPadding; } }
        public Vector3 BoardRotation { get { return GameDef.Get.BoardRotation; } }

        public Vector2 Dimensions { get; private set; }

        public Vector2 Offset { get; private set; }

        public Vector2 MinClamp { get; private set; }
        public Vector2 MaxClamp { get; private set; }

        public Vector2 CameraDimensions { get; private set; }

        private Vector2 _currentPos = Vector2.zero;

        void Awake()
        {
            Get = this;

            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Init(Point startNode, Point boardSize)
        {
            CameraDimensions = GetCameraDimensions();
            Dimensions = new Vector2(boardSize.x, boardSize.y)*Scaling;
            GetClamp();

            transform.localEulerAngles = BoardRotation;

            // Offset to show board with start node on left edge of the screen
            //var cameraOffset = CameraDimensions.x/2f * Vector2.left; // Move board the left screen edge
            //var startNodeOffset = -(Vector2)startNode * Scaling;   // Move node to bottom left screen corner
            //var edgeOffset = NodeScaling * Vector2.right;          // Move node slightly right to prevent cutoff
            //var offset = cameraOffset + startNode + edgeOffset;
            //Offset = Clamp(offset);

            Offset = -Dimensions / 2f;

            LeanTween.moveLocal(gameObject, Offset, 1f)
                .setEase(LeanTweenType.easeInOutSine);

            var margin = NodeScaling * Vector2.one * 2f;
            var scaledDimensions = Dimensions + margin;
            var cameraZoomScale = new Vector2(scaledDimensions.x / CameraDimensions.x, scaledDimensions.y / CameraDimensions.y);
            var cameraZoom = Camera.main.orthographicSize * cameraZoomScale;
            var maxZoom = Mathf.Max(cameraZoom.x, cameraZoom.y);

            LeanTween.value(Camera.main.orthographicSize, maxZoom, 1f)
                .setEase(LeanTweenType.easeInOutSine)
                .setDelay(0.2f)
                .setOnUpdate(v => Camera.main.orthographicSize = v);

            //BoardScaling = CameraScript.Fit(Dimensions, BoardPadding, BoardPadding + 2.0f);
            //transform.localScale = Vector3.one * BoardScaling;
            //transform.localPosition = -Dimensions * BoardScaling / 2 + (Vector2)transform.localPosition;
            //transform.localPosition = -(Vector3)startNode * Scaling;
        }

        private void GetClamp()
        {
            var boardOffset = CameraDimensions;
            var margin = NodeScaling * Vector2.one; // Add margin to prevent node cutoff
            var overlap = Dimensions - CameraDimensions;

            var minClamp = CameraDimensions / 2f - Dimensions - margin;
            var maxClamp = -CameraDimensions / 2f + margin;

            if (minClamp.x > maxClamp.x) {
                minClamp.x = maxClamp.x; 
            }
            if (minClamp.y > maxClamp.y) {
                minClamp.y = maxClamp.y = 0;
            }

            MinClamp = minClamp;
            MaxClamp = maxClamp;
        }

        private Vector2 GetCameraDimensions()
        {
            var cam = Camera.main;
            var p1 = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            var p2 = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
            var p3 = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

            var width = (p2 - p1).magnitude;
            var height = (p3 - p2).magnitude;

            return new Vector2(width, height);
        }

        public Vector2 Scale(Vector2 boardPos)
        {
            return boardPos*Scaling;
        }

        public Vector2 Clamp(Vector2 pos)
        {
            return new Vector2(
                Mathf.Clamp(pos.x, MinClamp.x, MaxClamp.x),
                Mathf.Clamp(pos.y, MinClamp.y, MaxClamp.y)
            );
        }

        public static PuzzleScale Get { get; private set; }
    }
}
