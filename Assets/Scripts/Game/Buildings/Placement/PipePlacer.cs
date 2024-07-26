using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// TODO undoing/canceling pipe placement?
public class PipePlacer : BuildingPlacer
{
    [SerializeField] private GameObject m_singlePipePreviewPrefab;
    [SerializeField] private float m_pipePreviewZOffset = -0.5f;

    private Vector2Int m_start;
    private Vector2Int m_end;
    private PipeFlowDirection m_startDir;
    private PipeFlowDirection m_endDir;
    private bool m_wasStartPlaced = false;

    private SpriteRenderer m_singlePipePreview;
    private LineRenderer m_pathfindingPreview;
    private Vector3[] m_pointArray = new Vector3[0];

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
        var mousePos = TileSelector.Instance.MouseToGrid();
        m_singlePipePreview.transform.position = new Vector3(mousePos.x + HARDCODED_OFFSET, mousePos.y + HARDCODED_OFFSET, 0);
        var can_be_built = IsValidPlacement(m_so);

        m_singlePipePreview.color = new Color(1, 1, 1, can_be_built ? 1 : 0.75f);

        if (m_wasStartPlaced)
        {
            var current_end = TileSelector.Instance.MouseToGrid();

            if (m_start.Equals(current_end) || !can_be_built) return; // skip if placing end on start

            // use linerenderer rather than creating lots of gameobjects

            // start the A* pathfinding (A* over Djikstras bc i had the code on hand lmao)
            var list = Pathfind(m_start, current_end);
            Vector3[] array = new Vector3[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                array[i] = new Vector3(list[i].x + HARDCODED_OFFSET, list[i].y + HARDCODED_OFFSET, m_pipePreviewZOffset);
            }

            m_pointArray = array;

            m_pathfindingPreview.positionCount = array.Length;
            m_pathfindingPreview.SetPositions(array);
        }
    }

    #region Pathfinding
    private List<Vector2Int> Pathfind(Vector2Int start, Vector2Int end)
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
            if (current != start && BoardManager.Instance.IsTileOccupied(current)) continue;

            Vector2Int[] neighbors = new Vector2Int[] { current + Vector2Int.up, current + Vector2Int.down, current + Vector2Int.left, current + Vector2Int.right };

            foreach (Vector2Int npos in neighbors)
            {
                if ((npos.x < 0 || npos.x >= BoardManager.MAP_SIZE_X)
                    || (npos.y < 0 || npos.y >= BoardManager.MAP_SIZE_Y))
                {
                    continue;
                }

                int new_cost = cost_so_far[current] + (BoardManager.Instance.IsTileOccupied(current) ? 999 : 1);

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

                var pos = pipe.GetPositions();

                // if we're placing the start position, check if it's connecting to the end of the pipe
                // otherwise, check the opposite
                if (!m_wasStartPlaced)
                {
                    return mousePos.Equals(pos.end);
                }
                else
                {
                    return mousePos.Equals(pos.start);
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

        m_previewFabInstances[0] = Instantiate(m_singlePipePreviewPrefab.gameObject);
        m_singlePipePreview = m_previewFabInstances[0].GetComponentInChildren<SpriteRenderer>();

        while (!WasMouseClicked || !IsValidPlacement(m_so))
        {
            UpdatePreview();

            yield return null;
        }

        m_start = TileSelector.Instance.MouseToGrid(); // record the start position of the pipe

        m_wasStartPlaced = true; // register that we've recorded it
        m_previewFabInstances[1] = Instantiate(m_singlePipePreviewPrefab.gameObject);
        m_singlePipePreview = m_previewFabInstances[1].GetComponentInChildren<SpriteRenderer>();

        // while the player doesn't click or they place in invalid spot, keep waiting and updating
        while (!WasMouseClicked || !IsValidPlacement(m_so))
        {
            UpdatePreview();

            yield return null;
        }

        m_end = TileSelector.Instance.MouseToGrid(); // record the end position of the pipe
        m_singlePipePreview = null; // dereference the preview; we don't need it anymore

        // issue is that every individual pipe prefab has a controller; bad.
        // instead, it should just be the visual, and there should be an empty gameobject that is the "pipe system"

        var tile_object = m_so.CreateInstance(m_start); //  this should be a PipeSO, and therefore none of the initialization is done.

        for (int index = 0; index < m_pointArray.Length; index++)
        {
            bool start_pipe = index == 0;
            bool end_pipe = index == m_pointArray.Length - 1;

            var v2i = Utilities.Vector3ToVector2Int(m_pointArray[index]);

            // if at start or end of array, calculate the relevant pipe flow direction
            if (start_pipe || end_pipe)
            {
                // there might be an error if the pipe is length 1. this will only happen if a single pipe
                // is placed with the same start and end and on a blank tile.
                if (m_pointArray.Length > 1)
                {
                    if (start_pipe)
                    {
                        Utilities.GetCardinalEstimatePipeflowDirection(
                            Utilities.Vector3ToVector2Int(m_pointArray[1]), 
                            Utilities.Vector3ToVector2Int(m_pointArray[0]), 
                            out m_startDir);
                    }
                    else if (end_pipe)
                    {
                        int end_index = m_pointArray.Length - 1;

                        Utilities.GetCardinalEstimatePipeflowDirection(
                            Utilities.Vector3ToVector2Int(m_pointArray[end_index]),
                            Utilities.Vector3ToVector2Int(m_pointArray[end_index - 1]),
                            out m_endDir);
                    }
                }
                else if (m_pointArray.Length <= 0)
                {
                    // if we have a singleton pipe in the middle of nowhere, I honestly don't know what its defaults should be.
                    // so they're just gonna be North and South.
                    m_startDir = PipeFlowDirection.North;
                    m_endDir = PipeFlowDirection.South;
                }
            }

            // this only really matters for the start and end pipes, but if the tile is occupied, dont draw the pipe there.
            if (!BoardManager.Instance.IsTileOccupied(v2i))
            {
                // create the pipe object at the point without the offset and without the initial offset of the parent transform (which is at the m_start position)
                var pipe = Instantiate(m_so.prefab, m_pointArray[index] - (Vector3.up + Vector3.right) * HARDCODED_OFFSET - Utilities.Vector2IntToVector3(m_start), Quaternion.identity);
                pipe.transform.parent = tile_object.transform;
                // TODO this should orient the pipe and change its sprite to match the flow start and end for the segment.
            }
        }

        Debug.Log(string.Format("Start Dir {0}, End Dir {1}", m_startDir, m_endDir));

        // setup the pipe
        if (!tile_object.TryGetComponent<PipeController>(out var component))
        {
            Debug.LogError("Pipe prefab doesn't have a pipe controller!");
        }

        component.InitializePipe(m_start, m_end, m_startDir, m_endDir);
        component.Initialize(m_so, m_start);
        component.transform.position = Utilities.Vector2IntToVector3(m_start);

        for (int index = 0; index < m_pointArray.Length; ++index)
        {
            var v2i = Utilities.Vector3ToVector2Int(m_pointArray[index]);

            // if we're the start or end pipe and our position is occupied, don't register us as a controller tile
            if ((index == 0 || index == m_pointArray.Length - 1) && BoardManager.Instance.IsTileOccupied(v2i)) continue;

            BoardManager.Instance.tileDictionary[v2i] = component;
        }
    }

    public override void Cleanup()
    {
        Destroy(m_previewFabInstances[0]);
        Destroy(m_previewFabInstances[1]);
        Destroy(gameObject);
    }
}
