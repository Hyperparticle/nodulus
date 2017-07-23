using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using View.Tween;

namespace View.Game
{
    public class LatticeView : MonoBehaviour {

        //public int horizontalLinesCount = 10;
        //public int verticalLinesCount = 5;

        //public float distanceBetweenLines = 2.0f;
        
        public GameObject GridLinePrefab;
        public GameObject GridPointPrefab;
        public GameObject RotorPrefab;

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

        private readonly List<GameObject> _gridObjects = new List<GameObject>();

        private float _nodeWidth;

        private void Awake()
        {
            _nodeWidth = RotorPrefab.transform.localScale.x;
//            _gridAlpha = GridColor.a;
        }

        public void Init(int horizontalLinesCount, int verticalLinesCount, float distanceBetweenLines)
        {
//            LeanTween.value(gameObject, _gridAlpha, 0f, FadeTime)
//                .setOnUpdate(a => {
//                    _grid.SetColor(new Color(GridColor.r, GridColor.g, GridColor.b, a));
//                    _dots.SetColor(new Color(GridColor.r, GridColor.g, GridColor.b, a));
//                })
//                .setEase(LeanTweenType.easeInOutSine)
//                .setOnComplete(() => {
//                    // Destroy the old grid, create a new one
//                    VectorLine.Destroy(ref _grid);
//                    VectorLine.Destroy(ref _dots);
//
//                    CreateNewLineGrid(horizontalLinesCount, verticalLinesCount, distanceBetweenLines);
//                    CreateNewDotGrid(horizontalLinesCount, verticalLinesCount, distanceBetweenLines);
//                });

            var k = 0;
            foreach (var gridLine in _gridObjects) {
                gridLine.GetComponent<GridTransit>().WaveOut(k++, Vector3.left);
                
                Destroy(gridLine, 1.5f);
            }
            
            _gridObjects.Clear();
            
            // Horizontal lines down Y axis
            for (var i = 0; i < horizontalLinesCount; i++) {
                var gridLine = Instantiate(GridLinePrefab);
                _gridObjects.Add(gridLine);
                
                gridLine.transform.SetParent(transform);
                gridLine.name = "GridLineHorizontal " + i;
                gridLine.transform.localRotation = Quaternion.identity;

                gridLine.transform.localPosition = new Vector3(
                    distanceBetweenLines * (verticalLinesCount - 1) / 2f, 
                    i * distanceBetweenLines,
                    ZOffset
                );
                
                var scale = transform.localScale;
                scale.Scale(new Vector3(distanceBetweenLines * (verticalLinesCount - 1) * _nodeWidth, 1, 1));
                gridLine.transform.localScale = scale;
                
                gridLine.GetComponent<GridTransit>().WaveIn(i, Vector3.right);
            }

            // Vertical lines across X axis
            for (var i = 0; i < verticalLinesCount; i++) {
                var gridLine = Instantiate(GridLinePrefab);
               _gridObjects.Add(gridLine);
                
                gridLine.transform.SetParent(transform);
                gridLine.name = "GridLineVertical " + i;
                gridLine.transform.localRotation = Quaternion.identity;

                gridLine.transform.localPosition = new Vector3(
                    i * distanceBetweenLines,
                    distanceBetweenLines * (horizontalLinesCount - 1) / 2f,
                    ZOffset
                );
                
                var scale = transform.localScale;
                scale.Scale(new Vector3(1, distanceBetweenLines * (horizontalLinesCount - 1) * _nodeWidth, 1));
                gridLine.transform.localScale = scale;
                
                gridLine.GetComponent<GridTransit>().WaveIn(i, Vector3.up);
            }

            for (var i = 0; i < horizontalLinesCount; i++) {
                for (var j = 0; j < verticalLinesCount; j++) {
                    var gridPoint = Instantiate(GridPointPrefab);
                    _gridObjects.Add(gridPoint);
                
                    gridPoint.transform.SetParent(transform);
                    gridPoint.name = "GridPoint (" + i + "," + j + ")";
                    gridPoint.transform.localRotation = Quaternion.identity;

                    gridPoint.transform.localPosition = new Vector3(
                        j * distanceBetweenLines, 
                        i * distanceBetweenLines,
                        ZOffset
                    );
                    
                    gridPoint.GetComponent<GridTransit>().WaveIn(i + j, Vector3.down);
                }
            }
            
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
