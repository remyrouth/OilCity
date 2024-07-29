using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
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
    private PipeSpriteScript m_pipeOrientation;
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
        m_pipeOrientation = GetComponent<PipeSpriteScript>();
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

            var src_to_ignore = BoardManager.Instance.IsTileOccupied(m_start) ? BoardManager.Instance.tileDictionary[m_start] : null;
            var dest_to_ignore = IsValidPlacement(m_so) && BoardManager.Instance.IsTileOccupied(current_end) ? BoardManager.Instance.tileDictionary[current_end] : null;

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

            if (current == end) break;
            if (current != start 
                && BoardManager.Instance.IsTileOccupied(current)
                && !BoardManager.Instance.tileDictionary[current].Equals(src_to_ignore)
                && !BoardManager.Instance.tileDictionary[current].Equals(dest_to_ignore)) continue;

            Vector2Int[] neighbors = new Vector2Int[] { current + Vector2Int.right, current + Vector2Int.up, current + Vector2Int.left, current + Vector2Int.down };

            foreach (Vector2Int npos in neighbors)
            {
                if ((npos.x < 0 || npos.x >= BoardManager.MAP_SIZE_X)
                    || (npos.y < 0 || npos.y >= BoardManager.MAP_SIZE_Y))
                {
                    continue;
                }

                int new_cost = cost_so_far[current] + (
                    BoardManager.Instance.IsTileOccupied(current) 
                    && !BoardManager.Instance.tileDictionary[current].Equals(src_to_ignore)
                    && !BoardManager.Instance.tileDictionary[current].Equals(dest_to_ignore) ? 999 : 1);

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
            // if we're currently hovering over a pipe, we'll want to make sure the connection place is valid
            if (BoardManager.Instance.tileDictionary[mousePos].TryGetComponent<PipeController>(out var pipe))
            {
                // pipes cannot connect at non-start/end-points

                var (start, end) = pipe.GetPositions();

                // if we're placing the start position, check if it's connecting to the end of the pipe
                // otherwise, check the opposite
                if (!m_wasStartPlaced)
                {
                    return mousePos.Equals(end);
                }
                else
                {
                    return mousePos.Equals(start);
                }
            }

            if (BoardManager.Instance.tileDictionary[mousePos].TryGetComponent<IFlowable>(out var flowable))
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

        // if we somehow set the start to the end, exit without placing a pipe
        if (m_start.Equals(m_end) || m_pointList.Count < 1) yield break;

        // issue is that every individual pipe prefab has a controller; bad.
        // instead, it should just be the visual, and there should be an empty gameobject that is the "pipe system"

        var tile_object = m_so.CreateInstance(m_start); //  this should be a PipeSO, and therefore none of the initialization is done.
        if (!tile_object.TryGetComponent<PipeController>(out var component))
        {
            Debug.LogError("Pipe prefab doesn't have a pipe controller!");
        }

        bool has_placed_start = false;
        bool has_placed_end = false;
        int start_ind = -1;
        Vector2Int prior_pipe_pos = new(-1, -1);
        for (int index = 0; index < m_pointList.Count; index++)
        {
            bool is_open_space = !BoardManager.Instance.IsTileOccupied(m_pointList[index]);

            if (!has_placed_start)
            {
                if (!is_open_space)
                {
                    prior_pipe_pos = m_pointList[index];
                    continue;
                }

                if (index != 0)
                {
                    Utilities.GetCardinalEstimatePipeflowDirection(m_pointList[index], prior_pipe_pos, out m_startDir);
                    // Debug.Log(m_startDir + " 1");

                    m_start = m_pointList[index];
                    start_ind = index;
                } 
                else
                {
                    Utilities.GetCardinalEstimatePipeflowDirection(m_pointList[1], m_pointList[0], out m_startDir);
                    // Debug.Log(m_startDir + " 2");

                    m_start = m_pointList[0];
                    start_ind = 0;
                }

                has_placed_start = true;
            }
            else if (!has_placed_end)
            {
                // if we reach this point, we've placed a starting pipe and are now looking to place end pipes
                // if we've reached a tile no longer open, that means we're at the end of the system
                if (!is_open_space)
                {
                    Utilities.GetCardinalEstimatePipeflowDirection(m_pointList[index], prior_pipe_pos, out m_endDir);
                    // Debug.Log(m_endDir + " 1");

                    m_end = m_pointList[index - 1];
                    break;
                }
                else if (index == m_pointList.Count - 1)
                {
                    Utilities.GetCardinalEstimatePipeflowDirection(m_pointList[index], m_pointList[index - 1], out m_endDir);
                    // Debug.Log(m_endDir + " 2");

                    m_end = m_pointList[index];

                    has_placed_end = true;
                }
            }

            BoardManager.Instance.SetPipeTileInSupermap(m_pointList[index], m_pipeOrientation.OrientPipes(
                index > 0 ? m_pointList[index - 1] : new Vector2Int(-1, -1),
                m_pointList[index],
                index < m_pointList.Count - 1 ? m_pointList[index + 1] : new Vector2Int(-1, -1)));

            BoardManager.Instance.tileDictionary[m_pointList[index]] = component;
            prior_pipe_pos = m_pointList[index];
        }

        for (int i = 0; i < tile_object.transform.childCount; i++)
        {
            tile_object.transform.GetChild(i).position = Utilities.Vector2IntToVector3(m_pointList[i + start_ind]) - Utilities.Vector2IntToVector3(m_start);
        }

        // setup the pipe
        component.InitializePipe(m_start, m_end, m_startDir, m_endDir, m_pointList);
        component.Initialize(m_so, Vector2Int.zero); // 2nd arg unused
        component.transform.position = Utilities.Vector2IntToVector3(m_start);
    }

    public override void Cleanup()
    {
        Destroy(m_previewFabInstances[0]);
        Destroy(m_previewFabInstances[1]);
        Destroy(gameObject);
    }
}
