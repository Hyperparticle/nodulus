using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class LatticeView : MonoBehaviour {

    //public int horizontalLinesCount = 10;
    //public int verticalLinesCount = 5;

    //public float distanceBetweenLines = 2.0f;

    public float lineWidth = 5.0f;
    public float zOffset = 0.5f; // Depth offset
    public float fadeTime = 1f;
    public float fadeInDelay = 1f;
    public float dotsWidth = 10.0f;

    public Color gridColor = Color.white;

    public Texture dotsTexture;

    private VectorLine _grid;
    private VectorLine _dots;

    private float _gridAlpha;

    private void Awake()
    {
        _gridAlpha = gridColor.a;

        _grid = new VectorLine("Grid", new List<Vector3>(), 0f) { color = gridColor };
        _grid.Draw3DAuto();

        _dots = new VectorLine("Dots", new List<Vector3>(), 0f) { color = gridColor };
        _dots.Draw3DAuto();
    }

    public void Init(int horizontalLinesCount, int verticalLinesCount, float distanceBetweenLines)
    {
        LeanTween.value(gameObject, _gridAlpha, 0f, fadeTime)
            .setOnUpdate(a => {
                _grid.SetColor(new Color(gridColor.r, gridColor.g, gridColor.b, a));
                _dots.SetColor(new Color(gridColor.r, gridColor.g, gridColor.b, a));
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
        for (int i = 0; i < horizontalLinesCount; i++) {
            linePoints.Add(new Vector3(i * distanceBetweenLines, 0, zOffset));
            linePoints.Add(new Vector3(i * distanceBetweenLines, (verticalLinesCount - 1) * distanceBetweenLines, zOffset));
        }

        // Lines down Y axis
        for (int i = 0; i < verticalLinesCount; i++) {
            linePoints.Add(new Vector3(0, i * distanceBetweenLines, zOffset));
            linePoints.Add(new Vector3((horizontalLinesCount - 1) * distanceBetweenLines, i * distanceBetweenLines, zOffset));
        }

        _grid = new VectorLine("Grid", linePoints, lineWidth) { drawTransform = transform.parent.parent, color = gridColor };
        _grid.SetColor(new Color(gridColor.r, gridColor.g, gridColor.b, 0f));
        _grid.Draw3DAuto();

        _grid.rectTransform.SetPositionAndRotation(transform.position, transform.rotation);

        LeanTween.value(gameObject, 0f, _gridAlpha, fadeTime)
            .setDelay(fadeInDelay)
            .setOnUpdate(a => _grid.SetColor(new Color(gridColor.r, gridColor.g, gridColor.b, a)))
            .setEase(LeanTweenType.easeInOutSine);
    }

    private void CreateNewDotGrid(int horizontalLinesCount, int verticalLinesCount, float distanceBetweenLines)
    {
        var dotPoints = new List<Vector3>();
        
        for (int i = 0; i < horizontalLinesCount; i++) {
            for (int j = 0; j < verticalLinesCount; j++) {
                dotPoints.Add(new Vector3(i * distanceBetweenLines, j * distanceBetweenLines, zOffset));
            }
        }
        
        _dots = new VectorLine("Dots", dotPoints, dotsWidth, LineType.Points) {
            texture = dotsTexture,
            drawTransform = transform.parent.parent,
            color = gridColor
        };
        _dots.SetColor(new Color(gridColor.r, gridColor.g, gridColor.b, 0f));
        _dots.Draw3DAuto();

        // Set the position and rotation manually because the individual dots rotation cannot be changed
        _dots.rectTransform.SetPositionAndRotation(transform.position, transform.rotation);

        LeanTween.value(gameObject, 0f, _gridAlpha, fadeTime)
            .setDelay(fadeInDelay)
            .setOnUpdate(a => _dots.SetColor(new Color(gridColor.r, gridColor.g, gridColor.b, a)))
            .setEase(LeanTweenType.easeInOutSine);
    }
}
