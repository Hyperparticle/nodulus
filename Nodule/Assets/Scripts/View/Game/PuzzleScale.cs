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

        void Awake()
        {
            Get = this;
        }

        public void Init(Point startNode, Point boardSize)
        {
            Dimensions = new Vector2(boardSize.x, boardSize.y)*Scaling;

            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            transform.Translate(-Dimensions*BoardScaling/2);

            transform.localEulerAngles = BoardRotation;

            //BoardScaling = CameraScript.Fit(Dimensions, BoardPadding, BoardPadding + 2.0f);
            //transform.localScale = Vector3.one * BoardScaling;
            //transform.localPosition = -Dimensions * BoardScaling / 2 + (Vector2)transform.localPosition;
            //transform.localPosition = -(Vector3)startNode * Scaling;
        }

        public static PuzzleScale Get { get; private set; }
    }
}
