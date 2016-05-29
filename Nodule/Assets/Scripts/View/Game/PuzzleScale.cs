using Assets.Scripts.Core.Data;
using UnityEngine;

namespace Assets.Scripts.View.Game
{
    public class PuzzleScale: MonoBehaviour
    {
        public float Scaling = 2.5f;
        public float NodeScaling = 1.0f;
        public float EdgeScaling = 1.0f;
        public float BoardScaling = 1.0f;
        public float BoardPadding = 1.0f;

        private Vector2 _dimensions;

        public void Init(Point startNode, Point boardSize)
        {
            _dimensions = new Vector2(boardSize.x, boardSize.y)*Scaling;

            transform.localScale = Vector3.one;

            //BoardScaling = CameraScript.Fit(_dimensions, BoardPadding, BoardPadding + 2.0f);
            //transform.localScale = Vector3.one * BoardScaling;
            transform.localPosition = -_dimensions * BoardScaling / 2 + (Vector2)transform.localPosition;

            //transform.localPosition = -(Vector3)startNode * Scaling;
        }

        private static PuzzleScale _puzzleScale;
        public static PuzzleScale Get()
        {
            return _puzzleScale ??
                   (_puzzleScale = GameObject.FindGameObjectWithTag("PuzzleGame").GetComponent<PuzzleScale>());
        }
    }
}
