using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CivilianCityManager : Singleton<CivilianCityManager>, ITickReceiver
{
    [field: SerializeField] private BuildingScriptableObject civilianBuilding;
    private int _tickTimer = 0;
    public int TickNumberInterval => 10;
    
    public void OnTick()
    {
        _tickTimer++;
        if (_tickTimer > TickNumberInterval)
        {
            _tickTimer = 0;
            InvokeAction();
        }
    }

    public void InvokeAction()
    {
        TileObjectController building = null;
        while(building is null)
        {            
            TileObjectController[] tmp = FindObjectsByType<TileObjectController>(FindObjectsSortMode.None);
            int range = Random.Range(0, tmp.Length);
            if (tmp[range].TryGetComponent<TreeController>(out _)) continue;
            building = tmp[range];
        }
        bool doFreeTilesExist = false;
        int i = 2;
        while (doFreeTilesExist)
        {
            List<Vector2Int> freeTiles = GetTilesInRange(building, i);
            if (!freeTiles.Any())
            {
                i++;
                continue;
            }
            else
            {
                BoardManager.Instance.Create(freeTiles[Random.Range(0, freeTiles.Count)], civilianBuilding);
                doFreeTilesExist = true;
            }
            
        }
        
    }

    private List<Vector2Int> GetTilesInRange(TileObjectController building, int range)
    {
    Vector2Int upperRight = building.Anchor + building.size;
        List<Vector2Int> tiles = new();



        for (int x = building.Anchor.x - range; x <= upperRight.x + range; x++)
        {
            for (int y = building.Anchor.y - range; y <= upperRight.y + range; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                int xDistance = Mathf.Max(0, building.Anchor.x - currentPos.x, currentPos.x - upperRight.x);
                int yDistance = Mathf.Max(0, building.Anchor.y - currentPos.y, currentPos.y - upperRight.y);

                if (new Vector2Int(xDistance, yDistance).sqrMagnitude <= range * range)
                    tiles.Add(currentPos);
            }
        }

        return tiles;
    }
}
