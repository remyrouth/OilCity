using System.Collections;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour, IPlacer
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    protected BuildingScriptableObject m_so;

    protected bool WasMouseClicked
    {
        get
        {
            bool val = m_wasMouseClicked;
            m_wasMouseClicked = false;
            return val;
        }
        private set => m_wasMouseClicked = value;
    }
    private bool m_wasMouseClicked = false;

    public void InitSO(BuildingScriptableObject so)
    {
        m_so = so;
    }

    public virtual void UpdatePreview()
    {
        var mousePos = TileSelector.Instance.MouseToGrid();
        if (!this) return;
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        var can_be_built = IsValidPlacement(m_so);

        m_spriteRenderer.color = new Color(1, can_be_built ? 1 : 0, can_be_built ? 1 : 0, can_be_built ? 0.75f : 0.25f);
    }

    public virtual bool IsValidPlacement(BuildingScriptableObject so)
    {
        var mousePos = TileSelector.Instance.MouseToGrid();
        return !BoardManager.Instance.AreTilesOccupiedForBuilding(mousePos, so) && m_so.placementCost <= MoneyManager.Instance.Money;
    }

    public virtual IEnumerator IEDoBuildProcess()
    {
        // keep updating the preview until the player clicks on a valid spot
        while (!WasMouseClicked || !IsValidPlacement(m_so))
        {
            UpdatePreview();
            yield return null;
        }

        // create the instance of the thing and set its position
        Vector2Int pos = TileSelector.Instance.MouseToGrid();
        if (BoardManager.Instance.Create(pos, m_so))
            MoneyManager.Instance.ReduceMoney(m_so.placementCost);
    }

    public virtual void Cleanup()
    {
        Destroy(gameObject);
    }
    public void PressMouse() => WasMouseClicked = IsValidPlacement(m_so);
}
