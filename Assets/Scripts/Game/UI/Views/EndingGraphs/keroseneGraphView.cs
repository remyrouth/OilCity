using System.Linq;

public class keroseneGraphView : SingleGraphView
{
    public override void PopulateGraph()
        => PopulateGraph(GameDataRecorder.Instance._records.Select(e => e.KeroseneSoldAmount).ToList());
}
