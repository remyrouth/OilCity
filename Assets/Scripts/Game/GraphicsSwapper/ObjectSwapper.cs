using UnityEngine;

public class ObjectSwapper : MonoBehaviour, IGraphicsChangeable
{
    [SerializeField] private GameObject newer, older;
    private void Start()
    {
        ChangeGraphics(GraphicsSwapperManager.SetNewer);
    }
    public void ChangeGraphics(bool pickNewer)
    {
        newer.SetActive(pickNewer);
        older.SetActive(!pickNewer);
    }
}
