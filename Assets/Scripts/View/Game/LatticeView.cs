using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

namespace View.Game
{
    public class LatticeView : MonoBehaviour {

        //public int horizontalLinesCount = 10;
        //public int verticalLinesCount = 5;

        //public float distanceBetweenLines = 2.0f;

        public float LineWidth = 5.0f;
        public float ZOffset = 0.5f; // Depth offset
        public float FadeTime = 1f;
        public float FadeInDelay = 1f;
        public float DotsWidth = 10.0f;

        public Color GridColor = Color.white;

        public Texture DotsTexture;

        private VectorLine _grid;
        private VectorLine _dots;

        private float _gridAlpha;

        private void Awake()
        {
            _gridAlpha = GridColor.a;

            _grid = new VectorLine("Grid", new List<Vector3>(), 0f) { color = GridColor };
            _grid.Draw3DAuto();

            _dots = new VectorLine("Dots", new List<Vector3>(), 0f) { color = GridColor };
            _dots.Draw3DAuto();
        }

        public void Init(int horizontalLinesCount, int verticalLinesCount, float distanceBetweenLines)
        {
            LeanTween.value(gameObject, _gridAlpha, 0f, FadeTime)
                .setOnUpdate(a => {
                    _grid.SetColor(new Color(GridColor.r, GridColor.g, GridColor.b, a));
                    _dots.SetColor(new Color(GridColor.r, GridColor.g, GridColor.b, a));
                })
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(() => {
                    // Destroy the old grid, create a new one
                    VectorLine.Destroy(ref _grid);
                    VectorLine.Destroy(ref _dots);

                    CreateNewLineGrid(horizontalLinesCount, verticalLinesCount, distanceBetweenLines);
                    CreateNewDotGrid(horizontalLinesCount, verticalLinesCount, distanceBetweenLines);
                });
        }

        private void CreateNewLineGrid(int horizontalLinesCount, int verticalLinesCount, float distanceBetweenLines)
        {
            var linePoints = new List<Vector3>();
        
            // Lines across X axis
            for (var i = 0; i < horizontalLinesCount; i++) {
                linePoints.Add(new Vector3(i * distanceBetweenLines, 0, ZOffset));
                linePoints.Add(new Vector3(i * distanceBetweenLines, (verticalLinesCount - 1) * distanceBetweenLines, ZOffset));
            }

            // Lines down Y axis
            for (var i = 0; i < verticalLinesCount; i++) {
                linePoints.Add(new Vector3(0, i * distanceBetweenLines, ZOffset));
                linePoints.Add(new Vector3((horizontalLinesCount - 1) * distanceBetweenLines, i * distanceBetweenLines, ZOffset));
            }

            _grid = new VectorLine("Grid", linePoints, LineWidth) {
                drawTransform = transform.parent.parent,
                color = GridColor
            };
            _grid.SetColor(new Color(GridColor.r, GridColor.g, GridColor.b, 0f));
            _grid.Draw3DAuto();

            _grid.rectTransform.SetPositionAndRotation(transform.position, transform.rotation);
            
            LeanTween.value(gameObject, 0f, _gridAlpha, FadeTime)
                .setDelay(FadeInDelay)
                .setOnUpdate(a => _grid.SetColor(new Color(GridColor.r, GridColor.g, GridColor.b, a)))
                .setEase(LeanTweenType.easeInOutSine);
        }

        private void CreateNewDotGrid(int horizontalLinesCount, int verticalLinesCount, float distanceBetweenLines)
        {
            var dotPoints = new List<Vector3>();
        
            for (var i = 0; i < horizontalLinesCount; i++) {
                for (var j = 0; j < verticalLinesCount; j++) {
                    dotPoints.Add(new Vector3(i * distanceBetweenLines, j * distanceBetweenLines, ZOffset));
                }
            }
        
            _dots = new VectorLine("Dots", dotPoints, DotsWidth, LineType.Points) {
                texture = DotsTexture,
                drawTransform = transform.parent.parent,
                color = GridColor
            };
            _dots.SetColor(new Color(GridColor.r, GridColor.g, GridColor.b, 0f));
            _dots.Draw3DAuto();

            // Set the position and rotation manually because the individual dots rotation cannot be changed
            _dots.rectTransform.SetPositionAndRotation(transform.position, transform.rotation);

            LeanTween.value(gameObject, 0f, _gridAlpha, FadeTime)
                .setDelay(FadeInDelay)
                .setOnUpdate(a => _dots.SetColor(new Color(GridColor.r, GridColor.g, GridColor.b, a)))
                .setEase(LeanTweenType.easeInOutSine);
        }
    }
}
