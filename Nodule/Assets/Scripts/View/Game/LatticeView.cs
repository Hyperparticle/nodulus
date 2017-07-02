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

    private VectorLine _grid;

    private void Awake()
    {
        _grid = new VectorLine("Grid", new List<Vector3>(), 0f);
        _grid.Draw3DAuto();
    }

    public void Init(int horizontalLinesCount, int verticalLinesCount, float distanceBetweenLines)
    {
        LeanTween.value(gameObject, 1f, 0f, fadeTime)
            .setOnUpdate(a => _grid.SetColor(new Color(_grid.color.r, _grid.color.g, _grid.color.b, a)))
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() => {
                // Destroy the old grid, create a new one
                VectorLine.Destroy(ref _grid);
                CreateNewGrid(horizontalLinesCount, verticalLinesCount, distanceBetweenLines);
            });
    }

    private void CreateNewGrid(int horizontalLinesCount, int verticalLinesCount, float distanceBetweenLines)
    {
        var points = new List<Vector3>();

        // Lines across X axis
        for (int i = 0; i < horizontalLinesCount; i++)
        {
            points.Add(new Vector3(i * distanceBetweenLines, 0, zOffset));
            points.Add(new Vector3(i * distanceBetweenLines, (verticalLinesCount - 1) * distanceBetweenLines, zOffset));
        }

        // Lines down Y axis
        for (int i = 0; i < verticalLinesCount; i++)
        {
            points.Add(new Vector3(0, i * distanceBetweenLines, zOffset));
            points.Add(new Vector3((horizontalLinesCount - 1) * distanceBetweenLines, i * distanceBetweenLines, zOffset));
        }

        _grid = new VectorLine("Grid", points, lineWidth) { drawTransform = transform };
        _grid.SetColor(new Color(_grid.color.r, _grid.color.g, _grid.color.b, 0f));
        _grid.Draw3DAuto();

        LeanTween.value(gameObject, 0f, 1f, fadeTime)
            .setDelay(fadeInDelay)
            .setOnUpdate(a => _grid.SetColor(new Color(_grid.color.r, _grid.color.g, _grid.color.b, a)))
            .setEase(LeanTweenType.easeInOutSine);
    }
}
