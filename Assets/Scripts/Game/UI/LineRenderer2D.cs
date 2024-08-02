using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class LineRenderer2D : Graphic
{
    public List<Vector2> points;
    public float lineThickness = 2f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points == null || points.Count < 2)
            return;

        float width = lineThickness / 2;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[i + 1];

            Vector2 direction = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x) * width;

            Vector2[] vs = new Vector2[] {
                start - perpendicular
                , start + perpendicular
                , end + perpendicular
                , end - perpendicular };
            UIVertex[] verts = new UIVertex[4];

            for (int j = 0; j < 4; j++)
            {
                verts[j] = UIVertex.simpleVert;
                verts[j].color = color;
                verts[j].position = vs[j];
            }

            vh.AddUIVertexQuad(verts);
        }
    }

    public void SetPoints(List<Vector2> points)
    {
        this.points = points;
        SetVerticesDirty();
    }
}
