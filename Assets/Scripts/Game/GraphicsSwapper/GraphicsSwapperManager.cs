using System.Linq;
using UnityEngine;

public class GraphicsSwapperManager : MonoBehaviour
{
    public static bool SetNewer = true;
    private void Start()
    {
        ControlManager.Instance.leftClickActivationButtontrigger += OnClicked;
    }
    private int _counter;
    public void OnClicked()
    {
        if (TileSelector.Instance.MouseToGrid() != new Vector2Int(BoardManager.MAP_SIZE_X-1, BoardManager.MAP_SIZE_Y-1))
            return;
        _counter++;
        if (_counter == 10)
        {
            ChangeGraphics();
            _counter = 0;
        }
    }
    [ContextMenu("ChangeGraphics")]
    public void ChangeGraphics()
    {
        SetNewer = !SetNewer;
        var changables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IGraphicsChangeable>();
        foreach (var changable in changables)
            changable.ChangeGraphics(SetNewer);
    }
}
