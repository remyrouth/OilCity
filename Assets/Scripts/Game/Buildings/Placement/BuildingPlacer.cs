using System.Collections;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour, IPlacer
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    protected BuildingScriptableObject m_so;

    private bool m_wasMouseClicked = false;

    public void InitSO(BuildingScriptableObject so)
    {
        m_so = so;
    }

    public virtual void UpdatePreview()
    {
        var mousePos = TileSelector.Instance.MouseToGrid();
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        var can_be_built = IsValidPlacement(m_so);

        m_spriteRenderer.color = new Color(1, 1, 1, can_be_built ? 1 : 0.75f);
    }

    public virtual bool IsValidPlacement(BuildingScriptableObject so)
    {
        var mousePos = TileSelector.Instance.MouseToGrid();
        return BoardManager.Instance.AreTilesOccupiedForBuilding(mousePos, so);
    }

    public virtual IEnumerator IEDoBuildProcess() 
    {
        // keep updating the preview until the player clicks on a valid spot
        while (!m_wasMouseClicked || !IsValidPlacement(m_so))
        {
            UpdatePreview();

            yield return null;

            m_wasMouseClicked = false;
        }


        // create the instance of the thing and set its position
        Vector2Int pos = TileSelector.Instance.MouseToGrid();
        m_so.CreateInstance().transform.position = new Vector3(pos.x, pos.y, 0);
    }

    public virtual void Cleanup()
    {
        Destroy(gameObject);
    }

    public void PressMouse() => m_wasMouseClicked = true;
}
