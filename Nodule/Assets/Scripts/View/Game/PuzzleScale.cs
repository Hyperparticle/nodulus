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

        private Vector2 _currentPos = Vector2.zero;
        private Vector2 _cameraDimensions;

        void Awake()
        {
            Get = this;

            var cam = Camera.main;
            var p1 = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            var p2 = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
            var p3 = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

            var width = (p2 - p1).magnitude;
            var height = (p3 - p2).magnitude;

            _cameraDimensions = new Vector2(width, height);

            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Init(Point startNode, Point boardSize)
        {
            Dimensions = new Vector2(boardSize.x, boardSize.y)*Scaling;

            transform.localEulerAngles = BoardRotation;

            //transform.Translate(-Dimensions * BoardScaling / 2);

            var cameraOffset = _cameraDimensions.x/2f * Vector2.left; // Move board the left screen edge
            var startNodeOffset = -(Vector2)startNode * Scaling;   // Move node to bottom left screen corner
            var edgeOffset = NodeScaling * Vector2.right;          // Move node slightly right to prevent cutoff

            var offset = cameraOffset + startNodeOffset + edgeOffset;

            LeanTween.moveLocal(gameObject, offset, 1f)
                .setEase(LeanTweenType.easeInOutSine);

            //transform.Translate(offset);

            //BoardScaling = CameraScript.Fit(Dimensions, BoardPadding, BoardPadding + 2.0f);
            //transform.localScale = Vector3.one * BoardScaling;
            //transform.localPosition = -Dimensions * BoardScaling / 2 + (Vector2)transform.localPosition;
            //transform.localPosition = -(Vector3)startNode * Scaling;
        }

        public Vector2 Scale(Vector2 boardPos)
        {
            return boardPos*Scaling;
        }

        public static PuzzleScale Get { get; private set; }
    }
}
