using System;
using Core.Data;
using UnityEngine;
using View.Control;

namespace View.Game
{
    /// <summary>
    /// Scales the puzzle appropriately and provides properties about the board's dimensions.
    /// </summary>
    public class PuzzleScale : MonoBehaviour
    {
        public float Scaling => GameDef.Get.Scaling;

        public float NodeScaling => GameDef.Get.NodeScaling;
        public float EdgeScaling => GameDef.Get.EdgeScaling;
        public float BoardScaling => GameDef.Get.BoardScaling;
        public float BoardPadding => GameDef.Get.BoardPadding;
        public Vector3 BoardRotation => GameDef.Get.BoardRotation;
        
        public Vector2 Margin => NodeScaling * new Vector2(2f, 3f); // TODO: magic numbers

        public Vector2 Dimensions { get; private set; }

        public Vector2 Offset { get; private set; }

        public Vector3 InitialPosition { get; private set; } = -Vector3.one;
        
        public event Action PuzzleInit;

        private BoxCollider _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
        }


        public void Init(Point startNode, Point boardSize)
        {
            Dimensions = new Vector2(boardSize.x, boardSize.y) * Scaling;

            transform.localEulerAngles = BoardRotation;

            // Move the board to the center of the screen
            Offset = -Dimensions / 2f;
            Offset.Scale(transform.localScale);

            // TODO: make init idempotent
            if (InitialPosition == -Vector3.one) {
                InitialPosition = transform.localPosition;
                
                transform.localPosition = InitialPosition + (Vector3) Offset;
            }
            
            var movePos = InitialPosition + (Vector3) Offset;
            
            // TODO: make configurable
            LeanTween.moveLocal(gameObject, movePos, 1f)
                .setEase(LeanTweenType.easeInOutSine);
            
            PuzzleInit?.Invoke();
            
            ScaleCollider();
        }

        public Vector2 Scale(Vector2 boardPos)
        {
            return boardPos*Scaling;
        }

        private void ScaleCollider()
        {
            var rot = transform.rotation.eulerAngles;
            var rotOffset = new Vector2(Mathf.Sin(Mathf.Deg2Rad * rot.y), Mathf.Sin(Mathf.Deg2Rad * rot.x));
            
            _collider.size = Dimensions + Vector2.one * NodeScaling * 4f + rotOffset;
            _collider.center = - (Vector3) Offset + Vector3.back * NodeScaling / 2f;
            _collider.center += new Vector3(rotOffset.x, -rotOffset.y) / 2f;
        }
    }
}
