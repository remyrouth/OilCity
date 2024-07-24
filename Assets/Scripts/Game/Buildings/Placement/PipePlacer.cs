using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// TODO undoing/canceling pipe placement?
public class PipePlacer : BuildingPlacer
{
    [SerializeField] private GameObject m_singlePipePreviewPrefab;

    private Vector2Int m_start;
    private Vector2Int m_end;
    private PipeFlowDirection m_startDir;
    private PipeFlowDirection m_endDir;
    private bool m_wasStartPlaced = false;

    private SpriteRenderer m_singlePipePreview;
    private LineRenderer m_pathfindingPreview;
    private Vector3[] m_pointArray;

    /// <summary>
    /// A two-element array where the first index is the pipe-start preview prefab instance reference, and the second index
    /// is the pipe-end preview prefab instance reference.
    /// </summary>
    private GameObject[] m_previewFabInstances;

    private void Awake()
    {
        m_previewFabInstances = new GameObject[2];
    }

    public override void UpdatePreview()
    {
        var mousePos = TileSelector.Instance.MouseToGrid();
        m_singlePipePreview.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        var can_be_built = IsValidPlacement(null);

        m_singlePipePreview.color = new Color(1, 1, 1, can_be_built ? 1 : 0.75f);

        if (m_wasStartPlaced)
        {
            var current_end = TileSelector.Instance.MouseToGrid();

            if (m_start.Equals(current_end)) return; // skip if placing end on start

            // use linerenderer rather than creating lots of gameobjects

            // start the A* pathfinding (A* over Djikstras bc i had the code on hand lmao)
            var list = Pathfind(m_start, current_end);
            Vector3[] array = new Vector3[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                array[i] = new Vector3(list[i].x, list[i].y, 0);
            }

            m_pointArray = array;

            m_pathfindingPreview.positionCount = array.Length;
            m_pathfindingPreview.SetPositions(array);
        }
    }

    #region Pathfinding
    private List<Vector2Int> Pathfind(Vector2Int start, Vector2Int end)
    {
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
            path.Add(step_cell);

            if (step_cell == start) break;

            step_cell = came_from[step_cell];
        }

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

        m_singlePipePreview = Instantiate(m_singlePipePreviewPrefab.gameObject).GetComponent<SpriteRenderer>();
        m_previewFabInstances[0] = m_singlePipePreview.gameObject;

        while (!Input.GetMouseButtonDown(0) || !IsValidPlacement(m_so))
        {
            UpdatePreview();

            yield return null;
        }

        m_start = TileSelector.Instance.MouseToGrid(); // record the start position of the pipe

        m_wasStartPlaced = true; // register that we've recorded it
        m_singlePipePreview = Instantiate(m_singlePipePreviewPrefab.gameObject).GetComponent<SpriteRenderer>(); // prepare the "end-preview" gameobject
        m_previewFabInstances[1] = m_singlePipePreview.gameObject; // register that we've prepared the 2nd preview gameobject

        // while the player doesn't click or they place in invalid spot, keep waiting and updating
        while (!Input.GetMouseButtonDown(0) || !IsValidPlacement(m_so))
        {
            UpdatePreview();

            yield return null;
        }

        m_end = TileSelector.Instance.MouseToGrid(); // record the end position of the pipe
        m_singlePipePreview = null; // dereference the preview; we don't need it anymore

        // issue is that every individual pipe prefab has a controller; bad.
        // instead, it should just be the visual, and there should be an empty gameobject that is the "pipe system"

        var tile_object = m_so.CreateInstance(); //  this should be a PipeSO, and therefore none of the initialization is done.

        for (int index = 0; index < m_pointArray.Length; index++)
        {
            bool start_pipe = index == 0;
            bool end_pipe = index == m_pointArray.Length - 1;

            // if at start or end of array, check to ensure that we wont place a pipe on top of anything
            // if we are, then we need to assign the pipe's flow direction.
            if (start_pipe || end_pipe)
            {
                var v2i = Utilities.Vector3ToVector2Int(m_pointArray[index]);

                // since our start point is on a tile that we validated to be able to use pipes, we don't
                // need to check that again; we only need to see if it was placed on a tile or an open space.
                //
                // there might still be an error if the pipe is length 1. this will only happen if a single pipe
                // is placed with the same start and end and on a blank tile.
                if (m_pointArray.Length > 1 && BoardManager.Instance.IsTileOccupied(v2i))
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

            var pipe = Instantiate(m_so.prefab, m_pointArray[index], Quaternion.identity);
            pipe.transform.parent = tile_object.transform;
            // TODO this should orient the pipe and change its sprite to match the flow start and end for the segment.
        }

        tile_object.transform.position = new Vector3(m_start.x, m_start.y, 0); // say that the start position of the system is at the first pipe

        if (!tile_object.TryGetComponent<PipeController>(out var component))
        {
            Debug.LogError("Pipe prefab doesn't have a pipe controller!");
        }

        component.InitializePipe(m_start, m_end, m_startDir, m_endDir);
        component.Initialize(m_so);
    }

    public override void Cleanup()
    {
        Destroy(m_previewFabInstances[0]);
        Destroy(m_previewFabInstances[1]);
        Destroy(gameObject);
    }
}
