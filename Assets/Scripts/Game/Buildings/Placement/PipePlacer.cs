using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PipePlacer : BuildingPlacer
{
    [SerializeField] private GameObject m_singlePipePreviewPrefab;
    [SerializeField] private float m_pipePreviewZOffset = -0.5f;

    private Vector2Int m_start;
    private Vector2Int m_end;
    private PipeFlowDirection m_startDir = PipeFlowDirection.Invalid;
    private PipeFlowDirection m_endDir = PipeFlowDirection.Invalid;
    private bool m_wasStartPlaced = false;

    private SpriteRenderer m_singlePipePreview;
    private LineRenderer m_pathfindingPreview;
    private List<Vector2Int> m_pointList;

    private const float HARDCODED_OFFSET = 0.5f;


    /// <summary>
    /// A two-element array where the first index is the pipe-start preview prefab instance reference, and the second index
    /// is the pipe-end preview prefab instance reference.
    /// </summary>
    private GameObject[] m_previewFabInstances;

    private void Awake()
    {
        m_previewFabInstances = new GameObject[2];
        m_pathfindingPreview = GetComponent<LineRenderer>();
    }

    public override void UpdatePreview()
    {
        if (m_singlePipePreview == null) return;

        var mousePos = TileSelector.Instance.MouseToGrid();
        m_singlePipePreview.transform.position = new Vector3(mousePos.x + HARDCODED_OFFSET, mousePos.y + HARDCODED_OFFSET, 0);
        var can_be_built = IsValidPlacement(m_so);

        m_singlePipePreview.color = new Color(1, 1, 1, can_be_built ? 1 : 0.75f);

        if (m_wasStartPlaced)
        {
            var current_end = TileSelector.Instance.MouseToGrid();

            if (m_start.Equals(current_end) || !can_be_built) return; // skip if placing end on start

            // use linerenderer rather than creating lots of gameobjects

            // terser code for the following:
            //    var src_to_ignore = BoardManager.Instance.IsTileOccupied(m_start) ? BoardManager.Instance.tileDictionary[m_start] : null;
            //    var dest_to_ignore = IsValidPlacement(m_so) && BoardManager.Instance.IsTileOccupied(current_end) ? BoardManager.Instance.tileDictionary[current_end] : null;
            BoardManager.Instance.TryGetTypeAt<TileObjectController>(m_start, out var src_to_ignore);
            BoardManager.Instance.TryGetTypeAt<TileObjectController>(current_end, out var dest_to_ignore);

            // start the A* pathfinding (A* over Djikstras bc i had the code on hand lmao)
            m_pointList = Pathfind(m_start, current_end, src_to_ignore, dest_to_ignore);
            var array = new Vector3[m_pointList.Count];

            for (int i = 0; i < m_pointList.Count; i++)
            {
                array[i] = new Vector3(m_pointList[i].x + HARDCODED_OFFSET, m_pointList[i].y + HARDCODED_OFFSET, m_pipePreviewZOffset);
            }

            m_pathfindingPreview.positionCount = m_pointList.Count;
            m_pathfindingPreview.SetPositions(array);
        }
    }

    #region Pathfinding
    private List<Vector2Int> Pathfind(Vector2Int start, Vector2Int end, TileObjectController src_to_ignore, TileObjectController dest_to_ignore)
    {
        var frontier = new SimplePriorityQueue<Vector2Int, int>();

        var came_from = new Dictionary<Vector2Int, Vector2Int>();
        var cost_so_far = new Dictionary<Vector2Int, int>();

        frontier.Enqueue(start, 0);
        came_from[start] = start;
        cost_so_far[start] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            bool is_occupied = BoardManager.Instance.IsTileOccupied(current);
            bool is_ignored = is_occupied && (BoardManager.Instance.tileDictionary[current].Equals(src_to_ignore) || BoardManager.Instance.tileDictionary[current].Equals(dest_to_ignore));

            if (current == end) break;
            //if (current != start && is_occupied && !is_ignored) continue;

            Vector2Int[] neighbors = new Vector2Int[] { current + Vector2Int.right, current + Vector2Int.up, current + Vector2Int.left, current + Vector2Int.down };

            foreach (Vector2Int npos in neighbors)
            {
                if (BoardManager.Instance.IsPositionOutsideBoard(npos)) continue;

                int cost_mod = 1;

                // "ignored" tiles are still traversable, but more expensive.
                if (is_ignored) cost_mod = 200;
                else if (is_occupied) cost_mod = 999; 

                int new_cost = cost_so_far[current] + cost_mod;

                if (!cost_so_far.ContainsKey(npos) || new_cost < cost_so_far[npos])
                {
                    cost_so_far[npos] = new_cost;
                    int priority = new_cost + ManhattanDistance(npos, end);
                    frontier.Enqueue(npos, priority);
                    came_from[npos] = current;
                }
            }
        }

        // path rebuilding
        var path = new List<Vector2Int>();
        var step_pos = end;

        while (came_from.ContainsKey(step_pos))
        {
            path.Add(step_pos);

            if (step_pos == start) break;

            step_pos = came_from[step_pos];
        }

        path.Reverse();
        return path;
    }

    public static int ManhattanDistance(Vector2Int lpos, Vector2Int rpos)
    {
        return Mathf.Abs(lpos.x - rpos.x) + Mathf.Abs(lpos.y - rpos.y);
    }

    #endregion

    /// <summary>
    /// Checks to see if the current placement for a mouse click is valid.
    /// Depends on the state of the placement (i.e. are we placing the start or the end of the pipe system), and if
    /// there's an object in the way or not. 
    /// 
    /// Pipes can be placed in free space; they just won't have connections. However, pipes can also be placed "attached to" 
    /// buildings initially. That means that the only case where a pipe won't connect is when the building isnt flowable
    /// or the building doesn't accept that sort of connection.
    /// </summary>
    /// <param name="so"></param>
    /// <returns></returns>
    public override bool IsValidPlacement(BuildingScriptableObject so)
    {
        var mousePos = TileSelector.Instance.MouseToGrid();

        if (BoardManager.Instance.AreTilesOccupiedForBuilding(mousePos, so))
        {
            if (BoardManager.Instance.TryGetTypeAt<IFlowable>(mousePos, out var flowable))
            {
                // logic:
                // if the start pipe has not been placed yet, then we're in the process of placing the starting pipe.
                // that means that whatever building was clicked on will be drawn FROM, into the pipe. Therefore, we check
                // to see if the flow config for the building we clicked can output flow into a pipe.
                //
                // same thing goes for the end pipe. since the building at the end of the pipe receives the flow from said 
                // pipe, we need to check to see if the building can actually take that flow.

                var flow_config = flowable.GetFlowConfig();
                return m_wasStartPlaced ? flow_config.can_input : flow_config.can_output;
            }

            // is occupied, but by a non-flow building
            return false;
        }
        else
        {
            // open space, therefore valid
            return true;
        }
    }

    public override IEnumerator IEDoBuildProcess()
    {
        m_wasStartPlaced = false;
        m_startDir = m_endDir = PipeFlowDirection.Invalid;

        m_previewFabInstances[0] = Instantiate(m_singlePipePreviewPrefab);
        m_singlePipePreview = m_previewFabInstances[0].GetComponentInChildren<SpriteRenderer>();

        while (!WasMouseClicked || !IsValidPlacement(m_so))
        {
            UpdatePreview();

            yield return null;
        }

        // to be reassigned
        m_start = TileSelector.Instance.MouseToGrid(); // record the start position of the pipe

        m_wasStartPlaced = true; // register that we've recorded it
        m_previewFabInstances[1] = Instantiate(m_singlePipePreviewPrefab);
        m_singlePipePreview = m_previewFabInstances[1].GetComponentInChildren<SpriteRenderer>();

        // while the player doesn't click or they place in invalid spot, keep waiting and updating
        while (!WasMouseClicked || !IsValidPlacement(m_so))
        {
            UpdatePreview();

            yield return null;
        }

        // to be reassigned
        m_end = TileSelector.Instance.MouseToGrid(); // record the end position of the pipe
        m_singlePipePreview = null; // dereference the preview; we don't need it anymore
        Debug.Log("point count " + m_pointList.Count);  
        // if we somehow set the start to the end, exit without placing a pipe
        if (m_start.Equals(m_end) || m_pointList.Count < 1) yield break;

        // issue is that every individual pipe prefab has a controller; bad.
        // instead, it should just be the visual, and there should be an empty gameobject that is the "pipe system"

        var tile_object = m_so.CreateInstance(m_start); //  this should be a PipeSO, and therefore none of the initialization is done.
        if (!tile_object.TryGetComponent<PipeController>(out var component))
        {
            Debug.LogError("Pipe prefab doesn't have a pipe controller!");
        }

        int pipes_laid = PlacePipes(component);

        // if no pipes are placed (i.e. all pathfound tiles are obstructed), break
        if (pipes_laid == 0)
        {
            IfPipeConnect();

            // Destroy(tile_object.gameObject); TODO re-uncomment this. It's supposed to be destroyed
            yield break;
        }

        // setup the pipe
        component.InitializePipe(m_start, m_end, m_startDir, m_endDir, m_pointList);
        component.Initialize(m_so, Vector2Int.zero); // 2nd arg unused
        component.transform.position = Utilities.Vector2IntToVector3(m_start);
    }

    /// <summary>
    /// if a no-pipe system was "placed" with a pipe connection possible (i.e. pipe adjacent to building, wanted to establish connection),
    /// checks and establishes a connection.
    /// </summary>
    private void IfPipeConnect()
    {
        if (m_pointList.Count == 2 && Utilities.GetCardinalEstimatePipeflowDirection(m_end, m_start, out var _))
        {
            if (BoardManager.Instance.TryGetTypeAt<PipeController>(m_start, out var s_controller) && BoardManager.Instance.TryGetTypeAt<IFlowable>(m_end, out var e_flowable))
            {
                s_controller.SetParent(e_flowable);
                e_flowable.AddChild(s_controller);

                s_controller.UpdateFlowAndVisual(m_start, m_end, false);
            }
            else if (BoardManager.Instance.TryGetTypeAt<IFlowable>(m_start, out var s_flowable) && BoardManager.Instance.TryGetTypeAt<PipeController>(m_end, out var e_controller))
            {
                e_controller.AddChild(s_flowable);
                s_flowable.SetParent(e_controller);

                e_controller.UpdateFlowAndVisual(m_end, m_start, true);
            }
        }
        else
        {
            Debug.LogWarning("Shorthand pipe connection failed.");
        }
    }

    private int PlacePipes(PipeController with_component)
    {
        bool has_placed_start = false;
        bool has_placed_end = false;
        Vector2Int prior_pipe_pos = new(-1, -1);
        int pipes_laid = 0;
        for (int index = 0; index < m_pointList.Count; index++)
        {
            bool is_open_space = !BoardManager.Instance.IsTileOccupied(m_pointList[index]);

            if (is_open_space)
            {
                // set the pipe in the supermap with its orientation
                BoardManager.Instance.SetPipeTileInSupermap(m_pointList[index], BuildingManager.Instance.GetPipeRotation(
                    index > 0 ? m_pointList[index - 1] : new Vector2Int(-1, -1),
                    m_pointList[index],
                    index < m_pointList.Count - 1 ? m_pointList[index + 1] : new Vector2Int(-1, -1)));

                // set the controller at the location
                BoardManager.Instance.tileDictionary[m_pointList[index]] = with_component;

                pipes_laid++;
            }

            if (!has_placed_start)
            {
                // if we havent encountered an open space yet, keep pushing the "start" of the pipe to the next index
                if (!is_open_space)
                {
                    prior_pipe_pos = m_pointList[index];
                    continue;
                }

                // if we've found an open space but it's not at the start index (0), then we must use the prior pipe pos to find out
                // what direction of flow this pipe segment would've had.
                if (index != 0)
                {
                    Utilities.GetCardinalEstimatePipeflowDirection(m_pointList[index], prior_pipe_pos, out m_startDir);

                    m_start = m_pointList[index];
                }
                else // otherwise we can just use the first and second indices.
                {
                    Utilities.GetCardinalEstimatePipeflowDirection(m_pointList[1], m_pointList[0], out m_startDir);

                    m_start = m_pointList[0];
                }

                has_placed_start = true;
            }

            if (!has_placed_end)
            {
                // if we've hit an occupied space or this index is the last, get the flow
                if (!is_open_space || index == m_pointList.Count - 1)
                {
                    Utilities.GetCardinalEstimatePipeflowDirection(m_pointList[index], prior_pipe_pos, out m_endDir);

                    m_end = m_pointList[is_open_space ? index : index - 1];

                    has_placed_end = true;
                }
            }

            // cache current pipe
            prior_pipe_pos = m_pointList[index];
        }

        return pipes_laid;
    }

    public override void Cleanup()
    {
        Destroy(m_previewFabInstances[0]);
        Destroy(m_previewFabInstances[1]);
        Destroy(gameObject);
    }
}
