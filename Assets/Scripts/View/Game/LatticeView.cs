using System.Collections.Generic;
using UnityEngine;
using View.Items;
using View.Tween;

namespace View.Game
{
    public class LatticeView : MonoBehaviour
    {
        public GameObject GridLinePrefab;
        public GameObject GridPointPrefab;
        public GameObject RotorPrefab;

        public float ZOffset = 0.5f; // Game board depth offset
        public float FadeTime = 2f;
        public float FadeDelay = 1f;

        private readonly List<GameObject> _gridLinesHorizontal = new List<GameObject>();
        private readonly List<GameObject> _gridLinesVertical = new List<GameObject>();
        private GameObject[,] _gridPoints = new GameObject[0, 0];

        private float _nodeWidth;

        private const float GridScale = 0.15f;
        
        private void Awake()
        {
            _nodeWidth = RotorPrefab.transform.localScale.x;
        }

        public void Init(int horizontalLinesCount, int verticalLinesCount, float distanceBetweenLines)
        {
            DestroyGridObjects(_gridLinesHorizontal, Vector3.left);
            DestroyGridObjects(_gridLinesVertical, Vector3.down);
            DestroyGridObjects(_gridPoints, Vector3.up);

            // Horizontal lines down Y axis
            for (var i = 0; i < horizontalLinesCount; i++) {
                var gridLine = Instantiate(GridLinePrefab);
                _gridLinesHorizontal.Add(gridLine);

                gridLine.transform.SetParent(transform);
                gridLine.name = "GridLineHorizontal " + i;
                gridLine.transform.localRotation = Quaternion.identity;

                gridLine.transform.localPosition = new Vector3(
                    distanceBetweenLines * (verticalLinesCount - 1) / 2f,
                    i * distanceBetweenLines,
                    ZOffset
                );

                var scale = transform.localScale;
                scale.Scale(new Vector3(distanceBetweenLines * (verticalLinesCount - 1) * _nodeWidth, GridScale, GridScale));
                gridLine.transform.localScale = scale;

                Appear(gridLine);
                gridLine.GetComponent<GridTransit>().WaveIn(i, Vector3.right, LeanTweenType.easeOutSine);
            }

            // Vertical lines across X axis
            for (var i = 0; i < verticalLinesCount; i++) {
                var gridLine = Instantiate(GridLinePrefab);
                _gridLinesVertical.Add(gridLine);

                gridLine.transform.SetParent(transform);
                gridLine.name = "GridLineVertical " + i;
                gridLine.transform.localRotation = Quaternion.identity;

                gridLine.transform.localPosition = new Vector3(
                    i * distanceBetweenLines,
                    distanceBetweenLines * (horizontalLinesCount - 1) / 2f,
                    ZOffset
                );

                var scale = transform.localScale;
                scale.Scale(new Vector3(GridScale, distanceBetweenLines * (horizontalLinesCount - 1) * _nodeWidth, GridScale));
                gridLine.transform.localScale = scale;
                
                Appear(gridLine);
                gridLine.GetComponent<GridTransit>().WaveIn(i, Vector3.up, LeanTweenType.easeOutSine);
            }

            // Points at each cross section
            _gridPoints = new GameObject[horizontalLinesCount, verticalLinesCount];
            for (var i = 0; i < horizontalLinesCount; i++) {
                for (var j = 0; j < verticalLinesCount; j++) {
                    var gridPoint = Instantiate(GridPointPrefab);
                    _gridPoints[i, j] = gridPoint;

                    gridPoint.transform.SetParent(transform);
                    gridPoint.name = "GridPoint (" + i + "," + j + ")";
                    gridPoint.transform.localRotation = Quaternion.identity;

                    gridPoint.transform.localPosition = new Vector3(
                        j * distanceBetweenLines,
                        i * distanceBetweenLines,
                        ZOffset
                    );
                    
                    Appear(gridPoint);
                    gridPoint.GetComponent<GridTransit>().WaveIn(i + j, Vector3.down, LeanTweenType.easeOutBack);
                }
            }
        }

        private void Appear(GameObject gameObj)
        {
            var colorizer = gameObj.GetComponent<Colorizer>();
            colorizer.PrimaryColor = gameObj.GetComponent<Renderer>().material.color;
            colorizer.Fade(0.01f, () => {
                colorizer.Appear(FadeTime, FadeDelay, colorizer.Ease);
            });
            
        }
        
        private void DestroyGridObjects(ICollection<GameObject> gridObjects, Vector3 dir)
        {
            var k = 0;
            foreach (var gridObject in gridObjects) {
                gridObject.GetComponent<GridTransit>().WaveOut(k++, dir, LeanTweenType.easeInSine);
                gridObject.GetComponent<Colorizer>().Fade(FadeTime);
                Destroy(gridObject, 1.5f);
            }

            gridObjects.Clear();
        }

        private void DestroyGridObjects(GameObject[,] gridObjects, Vector3 dir)
        {
            for (var i = 0; i < gridObjects.GetLength(0); i++) {
                for (var j = 0; j < gridObjects.GetLength(1); j++) {
                    var gridObject = gridObjects[i, j];
                    gridObject.GetComponent<GridTransit>().WaveOut(i + j, dir, LeanTweenType.easeInSine);
                    gridObject.GetComponent<Colorizer>().Fade(FadeTime);
                    Destroy(gridObject, 1.5f);
                }
            }
        }
    }
}
