using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        var can_be_built = IsValidPlacement(m_so);

        m_spriteRenderer.color = new Color(1, can_be_built ? 1 : 0, can_be_built ? 1 : 0, can_be_built ? 1 : 0.75f);
    }

    public virtual bool IsValidPlacement(BuildingScriptableObject so)
    {
        var mousePos = TileSelector.Instance.MouseToGrid();
        return !BoardManager.Instance.AreTilesOccupiedForBuilding(mousePos, so);
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
<<<<<<< Updated upstream
        BoardManager.Instance.Create(pos, m_so);
=======
        var value = m_so.CreateInstance();
        value.transform.position = new Vector3(pos.x, pos.y, 0);
>>>>>>> Stashed changes

        for (int i = 0; i < m_so.size.y; i++)
            for (int j = 0; j < m_so.size.x; j++)
                BoardManager.Instance.tileDictionary.Add(pos + new Vector2Int(j, i), value);
    }

    public virtual void Cleanup()
    {
        Destroy(gameObject);
    }

<<<<<<< Updated upstream
    public void PressMouse() => m_wasMouseClicked = IsValidPlacement(m_so);
=======
    public void PressMouse() => WasMouseClicked = true;
>>>>>>> Stashed changes
}
