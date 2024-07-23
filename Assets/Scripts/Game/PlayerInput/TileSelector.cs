using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseClick();
        }
    }

    private Vector2Int MouseToGrid()
    {
        Vector3 mouseSpotPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int mouseCellPosition = new Vector2Int(Mathf.CeilToInt(mouseSpotPosition.x), Mathf.CeilToInt(mouseSpotPosition.y));
        return mouseCellPosition;
    }
    private void OnMouseClick()
    {
        if (!BoardManager.Instance.IsTileOccupied(MouseToGrid())) return;
        if (!BoardManager.Instance.tileDictionary[MouseToGrid()].TryGetComponent(out TileObjectController building)) return;
        if (building.GetActions().Count <= 0) return;
        Debug.Log(building.GetActions());
    }
}
