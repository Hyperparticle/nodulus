using System;
using Core.Data;
using UnityEngine;
using View.Control;

namespace View.Game
{
    public class PuzzleScale : MonoBehaviour
    {
        public float Scaling => GameDef.Get.Scaling;

        public float NodeScaling => GameDef.Get.NodeScaling;
        public float EdgeScaling => GameDef.Get.EdgeScaling;
        public float BoardScaling => GameDef.Get.BoardScaling;
        public float BoardPadding => GameDef.Get.BoardPadding;
        public Vector3 BoardRotation => GameDef.Get.BoardRotation;

        public Vector2 Dimensions { get; private set; }

        public Vector2 Offset { get; private set; }

        public Vector3 InitialPosition { get; private set; }
        
        public event Action PuzzleInit;

        private void Awake()
        {
            InitialPosition = transform.localPosition;
        }
        
        public void Init(Point startNode, Point boardSize, Vector3 initialPosition)
        {
            Dimensions = new Vector2(boardSize.X, boardSize.Y)*Scaling;

            transform.localEulerAngles = BoardRotation;

            // Move the board to the center of the screen
            Offset = -Dimensions / 2f;
            var movePos = InitialPosition + (Vector3) Offset + initialPosition;
            movePos.Scale(transform.localScale);
            LeanTween.moveLocal(gameObject, movePos, 1f)
                .setEase(LeanTweenType.easeInOutSine);
            
            PuzzleInit?.Invoke();
        }

        public Vector2 Scale(Vector2 boardPos)
        {
            return boardPos*Scaling;
        }
    }
}
