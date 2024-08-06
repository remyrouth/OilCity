using UnityEngine;

public class EndingUI : UIState
{
    [SerializeField] private SingleGraphView[] _graphs;
    public override GameState type => GameState.EndingUI;

    public override void OnEnter()
    {
        base.OnEnter();
        foreach (var graph in _graphs)
            graph.PopulateGraph();
        TimeManager.Instance.TicksPerMinute = 0;
        //StartCoroutine(GameRecorder.Instance.DoRollback());
    }
}
