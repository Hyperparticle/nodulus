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

            var cam = Camera.main;
            var p1 = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            var p2 = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
            var p3 = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

            var width = (p2 - p1).magnitude;
            var height = (p3 - p2).magnitude;

            CameraDimensions = new Vector2(width, height);

            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Init(Point startNode, Point boardSize)
        {
            Dimensions = new Vector2(boardSize.x, boardSize.y)*Scaling;
            GetClamp();

            transform.localEulerAngles = BoardRotation;

            //transform.Translate(-Dimensions * BoardScaling / 2);

            var cameraOffset = CameraDimensions.x/2f * Vector2.left; // Move board the left screen edge
            var startNodeOffset = -(Vector2)startNode * Scaling;   // Move node to bottom left screen corner
            var edgeOffset = NodeScaling * Vector2.right;          // Move node slightly right to prevent cutoff

            var offset = cameraOffset + startNodeOffset + edgeOffset;

            Offset = Clamp(offset);

            LeanTween.moveLocal(gameObject, Offset, 1f)
                .setEase(LeanTweenType.easeInOutSine);

            //transform.Translate(offset);

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

            var clamp1 = CameraDimensions / 2f - Dimensions - margin;
            var clamp2 = -CameraDimensions / 2f + margin;

            MinClamp = new Vector2(Mathf.Min(clamp1.x, clamp2.x), Mathf.Min(clamp1.y, clamp2.y));
            MaxClamp = new Vector2(Mathf.Max(clamp1.x, clamp2.x), Mathf.Max(clamp1.y, clamp2.y));
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
