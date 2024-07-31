using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SingleGraphView : MonoBehaviour
{
    private const float _maxSizeY = 600;
    private const float _maxSizeX = 1055;
    protected const int MAX_POINTS = 20;
    protected LineRenderer2D _line;
    private void Awake()
    {
        _line = GetComponent<LineRenderer2D>();
    }

    public abstract void PopulateGraph();
    protected void PopulateGraph(List<float> values)
    {
        var max = values.Max();
        List<Vector2> points = new();
        for (int i = 0; i < values.Count; i++)
            points.Add(new Vector2(_maxSizeX/values.Count*i, values[i] * _maxSizeY / max));
        _line.SetPoints(points);
    }
}
