using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PipeBuildingPreview : BuildingPreview
{
    [SerializeField] private SpriteRenderer m_singlePipePreviewPrefab;

    private Vector2Int m_start;
    private Vector2Int m_end;
    private bool m_wasStartPlaced = false;

    private SpriteRenderer m_singlePipePreview;
    private LineRenderer m_pathfindingPreview;

    public override void UpdatePreview()
    {
        var mousePos = TileSelector.Instance.MouseToGrid();
        m_singlePipePreview.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        var can_be_built = IsValidPlacement(null);

        m_singlePipePreview.color = new Color(1, 1, 1, can_be_built ? 1 : 0.75f);

        if (m_wasStartPlaced)
        {
            var current_end = TileSelector.Instance.MouseToGrid();

            // use linerenderer rather than creating lots of gameobjects

            var list = Pathfind(m_start, current_end);
            Vector3[] array = new Vector3[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                array[i] = new Vector3(list[i].x, list[i].y, 0);
            }

            m_pathfindingPreview.positionCount = array.Length;
            m_pathfindingPreview.SetPositions(array);
        }
    }

    private List<Vector2Int> Pathfind(Vector2Int start, Vector2Int end)
    {
        // TODO implement comparer for v2ints that prioritize the origin
        var frontier = new SimplePriorityQueue<Vector2Int, float>();

        var came_from = new Dictionary<Vector2Int, Vector2Int>();
        var cost_so_far = new Dictionary<Vector2Int, float>();

        frontier.Enqueue(start, 0f);
        came_from[start] = start;
        cost_so_far[start] = 0f;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == end) break;

            Vector2Int[] neighbors = new Vector2Int[] { current + Vector2Int.up, current + Vector2Int.down, current + Vector2Int.left, current + Vector2Int.right };

            foreach (Vector2Int npos in neighbors)
            {
                float new_cost = cost_so_far[current] + (BoardManager.Instance.IsTileOccupied(current) ? 99f : 1f);

                if (!cost_so_far.ContainsKey(npos) || new_cost < cost_so_far[npos])
                {
                    cost_so_far[npos] = new_cost;
                    float priority = new_cost + ManhattanDistance(npos, end);
                    frontier.Enqueue(npos, priority);
                    came_from[npos] = current;
                }
            }
        }

        // path rebuilding
        var path = new List<Vector2Int>();
        var step_cell = end;

        while (came_from.ContainsKey(step_cell))
        {
            if (step_cell == start) break;

            path.Add(step_cell);

            step_cell = came_from[step_cell];
        }

        return path;
    }

    public static int ManhattanDistance(Vector2Int lpos, Vector2Int rpos)
    {
        return (int)(Mathf.Abs(lpos.x - rpos.x) + Mathf.Abs(lpos.y - rpos.y));
    }

    public override bool IsValidPlacement(BuildingScriptableObject so)
    {
        var mousePos = TileSelector.Instance.MouseToGrid();
        return BoardManager.Instance.AreTilesOccupiedForBuilding(mousePos, so); // pipe SOs are 1x1
    }

    public override IEnumerator IEDoBuildProcess()
    {
        m_wasStartPlaced = false;
        m_singlePipePreview = Instantiate(m_singlePipePreviewPrefab.gameObject).GetComponent<SpriteRenderer>();

        while (!Input.GetMouseButtonDown(0) || !IsValidPlacement(m_so))
        {
            UpdatePreview();

            yield return null;
        }

        m_start = TileSelector.Instance.MouseToGrid();
        m_wasStartPlaced = true;
        m_singlePipePreview = Instantiate(m_singlePipePreviewPrefab.gameObject).GetComponent<SpriteRenderer>();

        while (!Input.GetMouseButtonDown(0) || !IsValidPlacement(m_so) || !(m_start.Equals(TileSelector.Instance.MouseToGrid())))
        {
            UpdatePreview();

            yield return null;
        }

        m_end = TileSelector.Instance.MouseToGrid();
        m_singlePipePreview = null;


        m_so.CreateInstance().transform.position = new Vector3(m_start.x, m_start.y, 0);
        // TODO make the creation of pipes spread through the map to the tiles it covers
    }
}
