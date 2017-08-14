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
        private float _gridPointWidth;

        private const float GridLineScale = 0.15f;
        private const float GridPointScale = 0.2f;

        private ItemPool _itemPool;
        
        private void Awake()
        {
            _nodeWidth = RotorPrefab.transform.localScale.x;
            _gridPointWidth = GridPointPrefab.transform.localScale.x;

            _itemPool = GetComponentInParent<ItemPool>();
        }

        public void DestroyGrid()
        {
            DestroyGridObjects(_gridLinesHorizontal, Vector3.left);
            DestroyGridObjects(_gridLinesVertical, Vector3.down);
            DestroyGridObjects(_gridPoints, Vector3.up);
        }

        public void Init(int horizontalLinesCount, int verticalLinesCount, float distanceBetweenLines, 
            float animationSpeed = 1f, float delayScale = 1f)
        {
            if (verticalLinesCount > 1) {
                // Horizontal lines down Y axis
                for (var i = 0; i < horizontalLinesCount; i++) {
                    var gridLine = _itemPool.GetGridLine();
                    _gridLinesHorizontal.Add(gridLine);

                    var material = gridLine.GetComponent<Renderer>().material;
                    material.color = Colorizer.Alpha(material.color, 0f);

                    gridLine.transform.SetParent(transform);
                    gridLine.name = "GridLineHorizontal " + i;
                    gridLine.transform.localRotation = Quaternion.identity;

                    gridLine.transform.localPosition = new Vector3(
                        distanceBetweenLines * (verticalLinesCount - 1) / 2f,
                        i * distanceBetweenLines,
                        ZOffset
                    );

                    var scale = transform.localScale;
                    scale.Scale(new Vector3(
                        distanceBetweenLines * (verticalLinesCount - 1) * _nodeWidth - _gridPointWidth, 
                        GridLineScale, 
                        GridLineScale
                    ));
                    gridLine.transform.localScale = scale;

                    Appear(gridLine, animationSpeed, delayScale);
                    gridLine.GetComponent<GridTransit>().WaveIn(
                        i, horizontalLinesCount, Vector3.right, LeanTweenType.easeOutSine, animationSpeed, delayScale
                    );
                }
            }

            if (horizontalLinesCount > 1) {
                // Vertical lines across X axis
                for (var i = 0; i < verticalLinesCount; i++) {
                    var gridLine = _itemPool.GetGridLine();
                    _gridLinesVertical.Add(gridLine);
                    
                    var material = gridLine.GetComponent<Renderer>().material;
                    material.color = Colorizer.Alpha(material.color, 0f);

                    gridLine.transform.SetParent(transform);
                    gridLine.name = "GridLineVertical " + i;
                    gridLine.transform.localRotation = Quaternion.identity;

                    gridLine.transform.localPosition = new Vector3(
                        i * distanceBetweenLines,
                        distanceBetweenLines * (horizontalLinesCount - 1) / 2f,
                        ZOffset
                    );

                    var scale = transform.localScale;
                    scale.Scale(new Vector3(
                        GridLineScale, 
                        distanceBetweenLines * (horizontalLinesCount - 1) * _nodeWidth - _gridPointWidth, 
                        GridLineScale
                    ));
                    gridLine.transform.localScale = scale;
                
                    Appear(gridLine, animationSpeed, delayScale);
                    gridLine.GetComponent<GridTransit>().WaveIn(
                        i, verticalLinesCount, Vector3.up, LeanTweenType.easeOutSine, animationSpeed, delayScale
                    );
                }
            }

            // Points at each cross section
            _gridPoints = new GameObject[horizontalLinesCount, verticalLinesCount];
            for (var i = 0; i < horizontalLinesCount; i++) {
                for (var j = 0; j < verticalLinesCount; j++) {
                    var gridPoint = Instantiate(GridPointPrefab);
                    _gridPoints[i, j] = gridPoint;
                    
                    var material = gridPoint.GetComponent<Renderer>().material;
                    material.color = Colorizer.Alpha(material.color, 0f);

                    gridPoint.transform.SetParent(transform);
                    gridPoint.name = "GridPoint (" + i + "," + j + ")";
                    gridPoint.transform.localRotation = Quaternion.identity;

                    gridPoint.transform.localPosition = new Vector3(
                        j * distanceBetweenLines,
                        i * distanceBetweenLines,
                        ZOffset
                    );
                    
                    gridPoint.transform.localScale = Vector3.one * GridPointScale;
                    
                    Appear(gridPoint, animationSpeed, delayScale);
                    gridPoint.GetComponent<GridTransit>().WaveIn(
                        i + j, horizontalLinesCount * verticalLinesCount,  Vector3.down, LeanTweenType.easeOutBack, animationSpeed, delayScale
                    );
                }
            }
        }

        private void Appear(GameObject gameObj, float animationSpeed = 1f, float delayScale = 1f)
        {
            var colorizer = gameObj.GetComponent<Colorizer>();
            delayScale = delayScale < 0.5f ? 0.5f : delayScale;
            colorizer.Appear(FadeTime, FadeDelay, colorizer.Ease, animationSpeed, delayScale);
        }
        
        private void DestroyGridObjects(ICollection<GameObject> gridObjects, Vector3 dir, 
            float animationSpeed = 1f, float delayScale = 1f)
        {
            var k = 0;
            foreach (var gridObject in gridObjects) {
                gridObject.GetComponent<GridTransit>().WaveOut(
                    k++, gridObjects.Count, dir, LeanTweenType.easeInSine, animationSpeed, delayScale
                );
                gridObject.GetComponent<Colorizer>().Fade(FadeTime);
                _itemPool.ReleaseGridLines(gridObject, 1.5f);
            }

            gridObjects.Clear();
        }

        private void DestroyGridObjects(GameObject[,] gridObjects, Vector3 dir, 
            float animationSpeed = 1f, float delayScale = 1f)
        {
            for (var i = 0; i < gridObjects.GetLength(0); i++) {
                for (var j = 0; j < gridObjects.GetLength(1); j++) {
                    var gridObject = gridObjects[i, j];
                    gridObject.GetComponent<GridTransit>().WaveOut(
                        i + j, gridObjects.GetLength(0) * gridObjects.GetLength(1), dir, LeanTweenType.easeInSine, animationSpeed, delayScale
                    );
                    gridObject.GetComponent<Colorizer>().Fade(FadeTime);
                    Destroy(gridObject, 1.5f);
                }
            }
        }
    }
}
