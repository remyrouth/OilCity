using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using static UnityEditor.PlayerSettings;
using DG.Tweening;

public sealed class PipeController : BuildingController<BuildingScriptableObject>, IFlowable
{
    // only the child is actually invoked; the parent is more so just for establishing the flow tree
    private IFlowable m_child; // where you get the flow from (the start)
    private IFlowable m_parent; // where the flow goes to (the end)

    private PipeFlowDirection m_startDirection; // the orientation of the start pipe
    private PipeFlowDirection m_endDirection; // the orientation of the end pipe

    private Vector2Int m_startPipePos; // position of the start pipe
    private Vector2Int m_endPipePos; // position of the end pipe

    private List<Vector2Int> m_pipes;

    private GameObject m_connection;
    private GameObject m_noConnection;

#if UNITY_EDITOR
    private Mesh m_debugMesh;

    public void SetDebugMesh(Mesh debugMesh) => m_debugMesh = debugMesh;
#endif

    public void SetConnectionIndicators(GameObject connection, GameObject noConnection)
    {
        m_connection = connection;
        m_noConnection = noConnection;
    }

    /// <summary>
    /// Init method for just pipes. Provides necessary values for functionality.
    /// </summary>
    /// <param name="start_pos"></param>
    /// <param name="end_pos"></param>
    /// <param name="start_pipe_dir"></param>
    /// <param name="end_pipe_dir"></param>
    public void InitializePipe(Vector2Int start_pos, Vector2Int end_pos, PipeFlowDirection start_pipe_dir, PipeFlowDirection end_pipe_dir, List<Vector2Int> pipes)
    {
        // notarize all the values passed in
        m_startPipePos = start_pos;
        m_endPipePos = end_pos;

        m_startDirection = start_pipe_dir;
        m_endDirection = end_pipe_dir;

        int start_i = -1, end_i = -1;
        for (int i = 0; i < pipes.Count; i++)
        {
            if (pipes[i].Equals(start_pos)) start_i = i;
            if (pipes[i].Equals(end_pos)) end_i = i;
        }

        if (start_i == -1 || end_i == -1) throw new System.ArgumentException("start or end positions were not found in pipe point list!");

        m_pipes = pipes.GetRange(start_i, end_i - start_i + 1);
    }

    public void SetTileActions(List<TileAction> actions)
    {
        this.TileActions = actions;
    }
    public void SetActionPivot()
    {
        _actionsPivot = Vector2.one / 2;
    }
    private PipeSpillageEffect _oilSpillout, _keroseneSpillout;
    public void SetParticleSystems(GameObject _oil, GameObject _kerosene)
    {
        _oilSpillout = Instantiate(_oil, transform).GetComponent<PipeSpillageEffect>();
        _keroseneSpillout = Instantiate(_kerosene, transform).GetComponent<PipeSpillageEffect>();
    }

    protected override void CreateInitialConnections(Vector2Int _)
    {
        var child_pos = m_startPipePos + Utilities.GetPipeFlowDirOffset(Utilities.FlipFlow(m_startDirection));
        var parent_pos = m_endPipePos + Utilities.GetPipeFlowDirOffset(m_endDirection);

        var (connect_to_child, connect_to_parent) = ValidatePipesAndConnect(child_pos, parent_pos);

        if (connect_to_child && BoardManager.Instance.TryGetTypeAt<IFlowable>(child_pos, out var obj) && obj.GetInOutConfig().can_output)
        {
            if (obj.GetParent() == null)
            {
                obj.SetParent(this);
                AddChild(obj);
            }
            else
            {
                Debug.LogWarning("Pipe already has a connection! Ignoring...");
                PingSpot(m_noConnection, Utilities.Vector2IntToVector3(m_startPipePos + Utilities.GetPipeFlowDirOffset(Utilities.FlipFlow(m_startDirection))));
            }
        }
        else
        {
            PingSpot(m_noConnection, Utilities.Vector2IntToVector3(m_startPipePos + Utilities.GetPipeFlowDirOffset(Utilities.FlipFlow(m_startDirection))));
        }

        if (connect_to_parent && BoardManager.Instance.TryGetTypeAt<IFlowable>(parent_pos, out var pobj) && pobj.GetInOutConfig().can_input)
        {
            pobj.AddChild(this);
            SetParent(pobj);
        }
        else
        {
            PingSpot(m_noConnection, Utilities.Vector2IntToVector3(m_endPipePos + Utilities.GetPipeFlowDirOffset(m_endDirection)));
        }
    }

    /// <summary>
    /// A helper method for testing to see if the pipe connection would be valid. In all cases except for when
    /// connecting two pipes at not endpoints, this returns true/true.
    /// </summary>
    /// <param name="child_end"></param>
    /// <param name="parent_end"></param>
    /// <returns></returns>
    private (bool connect_to_child, bool connect_to_parent) ValidatePipesAndConnect(Vector2Int child_end, Vector2Int parent_end)
    {
        bool is_child_valid = true;
        if (BoardManager.Instance.TryGetTypeAt<PipeController>(child_end, out var c_pipe) && c_pipe != this)
        {
            var (_, end) = c_pipe.GetPositions();

            is_child_valid = end.Equals(child_end) && c_pipe.GetOpenStatus().open_end; // to prevent stealing a pipe from one that already has a connection

            if (is_child_valid) c_pipe.UpdateFlowAndVisual(end, m_startPipePos, true); // we're connecting to their end, so our start is the endpoint and their end is the pipe. Therefore, flip the flow dir.
        }
        else if (c_pipe == this) is_child_valid = false;

        bool is_parent_valid = true;
        if (BoardManager.Instance.TryGetTypeAt<PipeController>(parent_end, out var p_pipe) && p_pipe != this)
        {
            var (start, _) = p_pipe.GetPositions();

            is_parent_valid = start.Equals(parent_end) && p_pipe.GetOpenStatus().open_start; // to prevent connecting to a pipe that already has a connection

            if (is_parent_valid) p_pipe.UpdateFlowAndVisual(start, m_endPipePos, false); // don't flip the flowdir bc we are flowing into their start from our endpoint.
        }
        else if (p_pipe == this) is_parent_valid = false;

        return (is_child_valid, is_parent_valid);
    }

    /// <summary>
    /// Called by another pipe controller when this pipe controller needs to update one of its endpoints to
    /// flow into the calling pipe controller.
    /// </summary>
    public void UpdateFlowAndVisual(Vector2Int endpoint, Vector2Int pipe, bool flip_flow)
    {
        if (!Utilities.GetCardinalEstimatePipeflowDirection(endpoint, pipe, out var flow_direction))
        {
            Debug.LogError(string.Format("Pipeflow not adjacent! {0} {1}", endpoint, pipe));
            return;
        }

        // endpoint and pipe may refer to the opposite things if we're flowing into the pipe from the endpoint. Therefore, if needed, flip the estimated flow dir.
        if (flip_flow) flow_direction = Utilities.FlipFlow(flow_direction);

        var in_pos = Vector2Int.zero;
        var out_pos = Vector2Int.zero;
        var my_status = GetOpenStatus();

        // change flowdir for endpoint
        if (endpoint.Equals(m_startPipePos) && my_status.open_start)
        {
            in_pos = pipe;
            out_pos = m_pipes.Count > 1 ? m_pipes[1] : endpoint + Utilities.GetPipeFlowDirOffset(m_endDirection);
            m_startDirection = flow_direction;
        }
        else if (endpoint.Equals(m_endPipePos) && my_status.open_end)
        {
            in_pos = m_pipes.Count > 1 ? m_pipes[^2] : endpoint + Utilities.GetPipeFlowDirOffset(Utilities.FlipFlow(m_startDirection));
            out_pos = pipe;
            m_endDirection = flow_direction;
        }

        // change visual for endpoint
        BoardManager.Instance.ClearSupermapTile(endpoint); // wipe the tile before placing so the transform doesnt get borked
        BoardManager.Instance.SetPipeTileInSupermap(
            endpoint, 
            BuildingManager.Instance.GetPipeRotation(in_pos, endpoint, out_pos));
    }

    private void PingSpot(GameObject ping, Vector3 pos)
    {
        var obj = Instantiate(ping, pos + Vector3.forward * 2f + Vector3.right * 0.5f + Vector3.up, Quaternion.identity);
        obj.transform.localScale = Vector3.zero;
       // obj.transform.DOScale(Vector3.one, 0.25f);
        Destroy(obj, 2f);

        var sequence = DOTween.Sequence();
        sequence.Append(obj.transform.DOScale(Vector3.one, 0.25f));
        sequence.AppendInterval(1.5f);
        sequence.Append(obj.transform.DOScale(Vector3.zero, 0.25f));
    }

    /// <summary>
    /// Pipes can connect to other pipes.
    /// </summary>
    /// <returns></returns>
    public (bool can_input, bool can_output) GetInOutConfig() => (true, true);

    #region tree stuff
    /// <summary>
    /// If the pipe has no child source or the current child isn't the input child, reassign.
    /// It isn't possible to "add multiple" children to a pipe because there's only one connection.
    /// </summary>
    /// <param name="child"></param>
    public void AddChild(IFlowable child)
    {
        if (m_child == null || !m_child.Equals(child))
        {
            m_child = child;
            PingSpot(m_connection, Utilities.Vector2IntToVector3(m_startPipePos + Utilities.GetPipeFlowDirOffset(Utilities.FlipFlow(m_startDirection))));
        }

    }

    /// <summary>
    /// If the given child is equal to the current child, dereference them.
    /// </summary>
    /// <param name="child"></param>
    public void DisownChild(IFlowable child)
    {
        if (m_child != null && m_child.Equals(child))
        {
            m_child = null;
            // PingSpot(m_noConnection, Utilities.Vector2IntToVector3(m_startPipePos + Utilities.GetPipeFlowDirOffset(Utilities.FlipFlow(m_startDirection))));
        }
    }

    /// <summary>
    /// Returns a singleton list of the child this pipe sources input from. The list can be modified with no
    /// affect on the pipe itself. Use Add/DisownChild if you need to do that.
    /// 
    /// Returns an empty list if the child is null.
    /// </summary>
    /// <returns></returns>
    public List<IFlowable> GetChildren()
    {
        if (m_child == null) return new List<IFlowable>();

        var singleton_list = new List<IFlowable>
        {
            m_child
        };

        return singleton_list;
    }

    /// <summary>
    /// Returns the parent of the pipe; i.e. the destination of the flow from the child.
    /// </summary>
    /// <returns></returns>
    public IFlowable GetParent()
    {
        return m_parent;
    }

    /// <summary>
    /// Sets the flow destination to the given parent.
    /// </summary>
    /// <param name="parent"></param>
    public void SetParent(IFlowable parent)
    {
        
        if (parent != null)
        {
            PingSpot(m_connection, Utilities.Vector2IntToVector3(m_endPipePos + Utilities.GetPipeFlowDirOffset(m_endDirection)));
        }
        else
        {
            //PingSpot(m_noConnection, Utilities.Vector2IntToVector3(m_endPipePos + Utilities.GetPipeFlowDirOffset(m_endDirection)));
        }
        

        m_parent = parent;
        _oilSpillout.Stop();
        _keroseneSpillout.Stop();
    }
    #endregion

    /// <summary>
    /// TODO implement more, but for now, just recurses down the tree and reports the amount gathered as overflow. This method is invoked
    /// directly if a pipe becomes a member of the tickable forest.
    /// </summary>
    public void OnTick()
    {
        var received = SendFlow();
        Debug.LogWarning(string.Format("{0} has overflowed {1}", gameObject.name, received));

        if (received.type == FlowType.Oil && received.amount > 0)
            _oilSpillout.Play(m_endPipePos, m_endDirection);
        else
            _oilSpillout.Stop();

        if (received.type == FlowType.Kerosene && received.amount > 0)
            _keroseneSpillout.Play(m_endPipePos, m_endDirection);
        else
            _keroseneSpillout.Stop();
    }

    /// <summary>
    /// For a pipe's sendflow method, they just report the flow of their source.
    /// </summary>
    /// <returns></returns>
    public (FlowType type, float amount) SendFlow()
    {
        if (m_child == null) return (FlowType.None, 0f);

        return m_child.SendFlow();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // clear all relevant pipe tiles from supermap
        foreach (var pos in m_pipes)
        {
            BoardManager.Instance.ClearSupermapTile(pos);
        }
    }

    /// <summary>
    /// Returns true if the object at the position is the input for this pipe system. i.e. if the tile flows out into the pipe system.
    /// </summary>
    /// <param name="tile_pos"></param>
    /// <returns></returns>
    public bool DoesPipeSystemReceiveInputFromTile(Vector2Int tile_pos)
    {
        if (Utilities.GetCardinalEstimatePipeflowDirection(tile_pos, m_startPipePos, out PipeFlowDirection est_flow_dir))
        {
            // flow is flipped here because the estimate flow direction method operates under the assumption that the pipe is always flowing
            // into the tile, not the other way around.
            return Utilities.FlipFlow(est_flow_dir) == m_startDirection;
        }
        else
        {
            // not within any of the cardinal directions, so auto-false.
            return false;
        }
    }

    /// <summary>
    /// Returns true if the object at the position is the output object for this pipe system. i.e. if the pipe system flows into the tile.
    /// </summary>
    /// <param name="tile_pos"></param>
    /// <returns></returns>
    public bool DoesPipeSystemOutputToTile(Vector2Int tile_pos)
    {
        if (Utilities.GetCardinalEstimatePipeflowDirection(tile_pos, m_endPipePos, out PipeFlowDirection est_flow_dir)) return est_flow_dir == m_endDirection;
        else
        {
            // not within any of the cardinal directions, so auto-false.
            return false;
        }
    }

    public (Vector2Int start, Vector2Int end) GetPositions() => (m_startPipePos, m_endPipePos);

    public (bool open_start, bool open_end) GetOpenStatus() => (m_child == null, m_parent == null);

    public (FlowType in_type, FlowType out_type) GetFlowConfig()
    {
        var through_flow = m_child == null ? FlowType.None : m_child.GetFlowConfig().out_type;

        return (through_flow, through_flow);
    }

    void OnDrawGizmos()
    {
        var offset = new Vector3(0.5f, 0.5f);

        var s_dir = Utilities.GetPipeFlowDirOffset(m_startDirection);
        var e_dir = Utilities.GetPipeFlowDirOffset(m_endDirection);

        var s_rot = Quaternion.Euler(-Vector2.SignedAngle(Vector2.up, s_dir) + 90, 90f, 90f);
        var e_rot = Quaternion.Euler(-Vector2.SignedAngle(Vector2.up, e_dir) + 90, 90f, 90f);

        var s_pos = Utilities.Vector2IntToVector3(m_startPipePos) + offset - Utilities.Vector2IntToVector3(Utilities.GetPipeFlowDirOffset(m_startDirection)) * .35f;
        var e_pos = Utilities.Vector2IntToVector3(m_endPipePos) + offset + Utilities.Vector2IntToVector3(Utilities.GetPipeFlowDirOffset(m_endDirection)) * .35f;

        if (m_startDirection != PipeFlowDirection.Invalid)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(m_debugMesh, 0, s_pos, s_rot, new Vector3(0.2f, 0.1f, .2f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(s_pos, Vector3.one * 0.2f);
        }

        if (m_endDirection != PipeFlowDirection.Invalid)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireMesh(m_debugMesh, 0, e_pos, e_rot, new Vector3(0.2f, 0.1f, .2f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(e_pos, Vector3.one * 0.2f);
        }
    }
}