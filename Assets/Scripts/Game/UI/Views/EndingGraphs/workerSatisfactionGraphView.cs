using System.Linq;

public class workerSatisfactionGraphView : SingleGraphView
{
    public override void PopulateGraph()
        => PopulateGraph(GameDataRecorder.Instance._records.Select(e => e.WorkerSatisfaction).ToList());
}
