using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using Game.Events;
using UnityEngine;
using System;

public class PipePlacer : BuildingPlacer
{

    [SerializeField] private GameObject m_singlePipePreviewPrefab;
    [SerializeField] private float m_pipePreviewZOffset = -0.5f;

    [Space(10)]

    [SerializeField] private Gradient m_lineGradient;

    private Vector2Int m_start;
    private Vector2Int m_end;
    private PipeFlowDirection m_startDir = PipeFlowDirection.Invalid;
    private PipeFlowDirection m_endDir = PipeFlowDirection.Invalid;
    private bool m_wasStartPlaced = false;
    private (bool c_i, bool c_o) m_ioConfig;
    private (FlowType input, FlowType output) m_ftConfig;
    private bool m_needsFlip;
    private IFlowable m_first;

    private SpriteRenderer m_singlePipePreview;
    private LineRenderer m_pathfindingPreview;
    private List<Vector2Int> m_pointList;

    private const float HARDCODED_OFFSET = 0.5f;

    private bool m_shouldUpdatePathfinding;
    private Vector2Int m_previousPosition;

    public static Predicate<(Vector2Int, bool)> IsValidPlaceOverride = null;

    /// <summary>
    /// A two-element array where the first index is the pipe-start preview prefab instance reference, and the second index
    /// is the pipe-end preview prefab instance reference.
    /// </summary>
    private GameObject[] m_previewFabInstances;

    private void Awake()
    {
        m_previewFabInstances = new GameObject[2];
        m_pathfindingPreview = GetComponent<LineRenderer>();
        m_pathfindingPreview.colorGradient = m_lineGradient;

        m_pointList = new List<Vector2Int>();

        m_ioConfig = (true, true);
        m_ftConfig = (FlowType.Any, FlowType.Any);
    }

    #region callbacks
    private void OnEnable()
    {
        BuildingEvents.OnCivilianBuildingSpawn += ForcePathfindingUpdate;
    }

    private void OnDisable()
    {
        BuildingEvents.OnCivilianBuildingSpawn -= ForcePathfindingUpdate;
    }
    #endregion

    private void ForcePathfindingUpdate() => m_shouldUpdatePathfinding = true;

    public override void UpdatePreview()
    {
        if (m_singlePipePreview == null) return;

        var mousePos = TileSelector.Instance.MouseToGrid();
        m_singlePipePreview.transform.position = new Vector3(mousePos.x + HARDCODED_OFFSET, mousePos.y + HARDCODED_OFFSET, 0);
        var can_be_built = IsValidPlacement(m_so);

        m_singlePipePreview.color = new Color(1, 1, 1, can_be_built ? 1 : 0.75f);

        if (m_wasStartPlaced)
        {
            // if we weren't forced to update pathfinding, perform our manual checks to see if we should pathfind this frame:
            // - is there a path to find? (i.e. start isnt end)
            // - is the current position in a valid placement?
            // - did we move our mouse to a new position?
            if (!m_shouldUpdatePathfinding) m_shouldUpdatePathfinding = m_start.Equals(mousePos) || !can_be_built || m_previousPosition.Equals(mousePos);

            m_previousPosition = mousePos;

            if (!m_shouldUpdatePathfinding) return;

            // use linerenderer rather than creating lots of gameobjects

            // terser code for the following:
            //    var src_to_ignore = BoardManager.Instance.IsTileOccupied(m_start) ? BoardManager.Instance.tileDictionary[m_start] : null;
            //    var dest_to_ignore = IsValidPlacement(m_so) && BoardManager.Instance.IsTileOccupied(current_end) ? BoardManager.Instance.tileDictionary[current_end] : null;
            BoardManager.Instance.TryGetTypeAt<TileObjectController>(m_start, out var src_to_ignore);
            BoardManager.Instance.TryGetTypeAt<TileObjectController>(mousePos, out var dest_to_ignore);

            // start the A* pathfinding (A* over Djikstras bc i had the code on hand lmao)
            m_pointList = Pathfind(m_start, mousePos, src_to_ignore, dest_to_ignore);
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

            bool is_current_occupied = BoardManager.Instance.IsTileOccupied(current);
            bool is_current_src_occupied = is_current_occupied && BoardManager.Instance.tileDictionary[current].Equals(src_to_ignore);
            bool is_current_dest_occupied = is_current_occupied && BoardManager.Instance.tileDictionary[current].Equals(dest_to_ignore);

            if (current == end) break;

            Vector2Int[] neighbors = new Vector2Int[] { current + Vector2Int.right, current + Vector2Int.up, current + Vector2Int.left, current + Vector2Int.down };

            foreach (Vector2Int npos in neighbors)
            {
                if (BoardManager.Instance.IsPositionOutsideBoard(npos)) continue;

                // traversal logic:
                // - if this tile is occupied (and not ignored), it is uber expensive
                // - if this tile is a src/dest and the neighbor is the opposite, it is uber expensive
                // - if this tile is a src, it is slightly expensive
                // - if this tile is a dest, it is slightly expensive

                // these manual checks are faster than the TryGetAt method, due to no reliance on TryGetComponent
                bool is_neighbor_occupied = BoardManager.Instance.IsTileOccupied(npos);
                bool is_neighbor_src = is_neighbor_occupied && BoardManager.Instance.tileDictionary[npos].Equals(src_to_ignore);
                bool is_neighbor_dest = is_neighbor_occupied && BoardManager.Instance.tileDictionary[npos].Equals(dest_to_ignore);

                int cost_mod = 1;


                if (is_neighbor_dest || is_neighbor_src) cost_mod = 5;
                if ((is_current_occupied && !is_current_src_occupied && !is_current_dest_occupied)
                    || (is_current_src_occupied && is_neighbor_dest)
                    || (is_current_dest_occupied && is_neighbor_src)) cost_mod = 999;


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

        // costly path invalid
        if (cost_so_far.ContainsKey(end) && cost_so_far[end] > 999)
        {
            return new List<Vector2Int>();
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
        if (IsValidPlaceOverride != null)
            return IsValidPlaceOverride.Invoke((mousePos, m_wasStartPlaced));
        if (BoardManager.Instance.AreTilesOccupiedForBuilding(mousePos, so))
        {
            if (BoardManager.Instance.TryGetTypeAt<IFlowable>(mousePos, out var flowable))
            {
                if (!m_wasStartPlaced) return true;
                if (flowable.Equals(m_first)) return false;

                // logic:
                // if the start pipe has not been placed yet, then we're in the process of placing the starting pipe.
                // that means that whatever building was clicked on will be drawn FROM, into the pipe. Therefore, we check
                // to see if the flow config for the building we clicked can output flow into a pipe.
                //
                // same thing goes for the end pipe. since the building at the end of the pipe receives the flow from said 
                // pipe, we need to check to see if the building can actually take that flow.

                return CheckOptionsForValidConnection(mousePos);
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

    private void UpdateCurrentSinglePreview(bool is_tentative_placement_valid)
    {
        if (m_singlePipePreview == null) return;

        // if we're the first placed spot, we don't care how long the pointlist length is
        bool rhs = !m_wasStartPlaced || m_pointList.Count != 0;

        m_singlePipePreview.color = is_tentative_placement_valid && rhs ? Color.green : Color.red;
    }

    private IEnumerator IEWaitForValidPositionClicked()
    {
        while (true)
        {
            UpdatePreview();

            bool is_placement_valid = IsValidPlacement(m_so);

            UpdateCurrentSinglePreview(is_placement_valid);

            if (WasMouseClicked && is_placement_valid) break;

            yield return null;
        }
    }

    public override IEnumerator IEDoBuildProcess()
    {
        m_wasStartPlaced = false;
        m_startDir = m_endDir = PipeFlowDirection.Invalid;

        m_previewFabInstances[0] = Instantiate(m_singlePipePreviewPrefab);
        m_singlePipePreview = m_previewFabInstances[0].GetComponentInChildren<SpriteRenderer>();

        yield return StartCoroutine(IEWaitForValidPositionClicked());

        // to be reassigned
        m_start = TileSelector.Instance.MouseToGrid(); // record the start position of the pipe

        AddConfigs(m_start);

        bool did_occupy_start = false;
        // if space is open, occupy it with the preview tilecontroller so that no building can slide into the "open" spot
        if (!BoardManager.Instance.IsTileOccupied(m_start))
        {
            BoardManager.Instance.tileDictionary[m_start] = m_previewFabInstances[0].GetComponent<TileObjectController>();
            did_occupy_start = true;
        }

        m_wasStartPlaced = true; // register that we've recorded it
        m_previewFabInstances[1] = Instantiate(m_singlePipePreviewPrefab);
        m_singlePipePreview = m_previewFabInstances[1].GetComponentInChildren<SpriteRenderer>();

        // while the player doesn't click or they place in invalid spot, keep waiting and updating
        yield return StartCoroutine(IEWaitForValidPositionClicked());

        // to be reassigned
        m_end = TileSelector.Instance.MouseToGrid(); // record the end position of the pipe
        m_singlePipePreview = null; // dereference the preview; we don't need it anymore

        // if we somehow set the start to the end, exit without placing a pipe
        if (m_start.Equals(m_end) || m_pointList.Count < 1) yield break;

        // if we registered a "fake" controller to occupy the start, remove it before placement
        if (did_occupy_start)
        {
            // not great to modify the map outside of the class, but there's no neater method present in the manager to do so.
            BoardManager.Instance.tileDictionary.Remove(m_start);
        }

        int pipes_laid = PlacePipes();

        // if no pipes are placed (i.e. all pathfound tiles are obstructed), break
        if (pipes_laid == 0)
        {
            IfPipeConnect();

            yield break;
        }

        // issue is that every individual pipe prefab has a controller; bad.
        // instead, it should just be the visual, and there should be an empty gameobject that is the "pipe system"

        var tile_object = m_so.CreateInstance(m_start); //  this should be a PipeSO, and therefore none of the initialization is done.
        if (!tile_object.TryGetComponent<PipeController>(out var component))
        {
            Debug.LogError("Pipe prefab doesn't have a pipe controller!");
        }

        // place controllers in the valid positions
        bool in_bounds = false;
        for (int i = 0; i < m_pointList.Count; ++i)
        {
            if (!in_bounds && m_pointList[i].Equals(m_start)) in_bounds = true;

            if (in_bounds)
            {
                BoardManager.Instance.tileDictionary[m_pointList[i]] = component;
            }

            if (in_bounds && m_pointList[i].Equals(m_end)) in_bounds = false;
        }

        // setup the pipe
        component.InitializePipe(m_start, m_end, m_startDir, m_endDir, m_pointList, m_needsFlip);

        component.Initialize(m_so, Vector2Int.zero); // 2nd arg unused
        component.transform.position = Utilities.Vector2IntToVector3(m_start);
        PipeEvents.PlacePipe();
    }

    private void AddConfigs(Vector2Int at_pos)
    {
        if (!BoardManager.Instance.TryGetTypeAt<IFlowable>(at_pos, out var flowable)) return;

        m_first = flowable;

        m_ioConfig = flowable.GetInOutConfig();
        m_ftConfig = flowable.GetFlowConfig();
    }

    private bool CheckOptionsForValidConnection(Vector2Int at_pos)
    {
        if (!BoardManager.Instance.TryGetTypeAt<IFlowable>(at_pos, out var flowable)) return false;

        var io = flowable.GetInOutConfig();
        var ft = flowable.GetFlowConfig();
        Debug.Log(m_ioConfig + " " + m_ftConfig); // true false | kerosene none
        Debug.Log(io + " " + ft);  // false true | none oil
        if (io.can_input)
        {
            bool has_dir = m_ioConfig.c_o;
            bool has_flow = m_ftConfig.output == ft.in_type || m_ftConfig.output == FlowType.Any;

            if (has_dir && has_flow)
            {
                m_needsFlip = false;

                return true;
            }
        }

        if (io.can_output)
        {
            bool has_dir = m_ioConfig.c_i;
            bool has_flow = m_ftConfig.input == ft.out_type || m_ftConfig.output == FlowType.Any;

            if (has_dir && has_flow)
            {
                m_needsFlip = true;

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// if a no-pipe system was "placed" with a pipe connection possible (i.e. pipe adjacent to building, wanted to establish connection),
    /// checks and establishes a connection.
    /// </summary>
    private void IfPipeConnect()
    {
        if (m_pointList.Count == 2 && Utilities.GetCardinalEstimatePipeflowDirection(m_end, m_start, out var _))
        {
            bool has_start_pipe = BoardManager.Instance.TryGetTypeAt<PipeController>(m_start, out var s_controller);
            bool has_end_pipe = BoardManager.Instance.TryGetTypeAt<PipeController>(m_end, out var e_controller);

            if (has_start_pipe && has_end_pipe)
            {
                if (s_controller.Equals(e_controller) // prevents self-connections
                    || (s_controller.GetParent() != null && s_controller.GetParent().Equals(e_controller)) // prevents repeated reparenting connections
                    || s_controller.GetPositions().start.Equals(m_start) // prevents wrong-way connections (start of pipe cannot connect to the end of another pipe)
                    || e_controller.GetPositions().end.Equals(m_end)) // prevents wrong-way connections (see above, most likely redundant)
                {
                    Debug.LogWarning("Invalid pipe connection! See comments for reasoning. Aborting...");
                    return;
                }

                s_controller.UpdateFlowAndVisual(m_start, m_end, true);
                e_controller.UpdateFlowAndVisual(m_end, m_start, false);

                s_controller.SetParent(e_controller);
                e_controller.AddChild(s_controller);

                TimeManager.Instance.LiteDeregister(s_controller);
            }
            else if (has_start_pipe && BoardManager.Instance.TryGetTypeAt<IFlowable>(m_end, out var e_flowable))
            {
                s_controller.UpdateFlowAndVisual(m_start, m_end, true);

                s_controller.SetParent(e_flowable);
                e_flowable.AddChild(s_controller);

                TimeManager.Instance.LiteDeregister(s_controller);
            }
            else if (BoardManager.Instance.TryGetTypeAt<IFlowable>(m_start, out var s_flowable) && has_end_pipe)
            {
                e_controller.UpdateFlowAndVisual(m_end, m_start, false);

                e_controller.AddChild(s_flowable);
                s_flowable.SetParent(e_controller);

                TimeManager.Instance.LiteDeregister(s_flowable);
            }
        }
        else
        {
            Debug.LogWarning("Shorthand pipe connection failed.");
        }
    }

    private int PlacePipes()
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

        SoundManager.Instance.SelectPipeSFXTrigger();

        return pipes_laid;
    }

    public override void Cleanup()
    {
        var tile_controller = m_previewFabInstances[0].GetComponent<TileObjectController>();
        if (BoardManager.Instance.TryGetTypeAt<TileObjectController>(m_start, out var tc) && tc.Equals(tile_controller))
        {
            BoardManager.Instance.tileDictionary.Remove(m_start);
        }

        Destroy(m_previewFabInstances[0]);
        Destroy(m_previewFabInstances[1]);
        Destroy(gameObject);
    }
}
