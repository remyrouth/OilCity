using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OilDebugger : MonoBehaviour
{
    [SerializeField]
    private Gradient oilGradient;
    private BoardManager boardManager;
    private OilMapController oilEvaluator;


    private Tilemap _oilTileMap;

    private float highestOilValueSofar;

    private void Start()
    {
        // boardManager = BoardManager.Instance;
        // oilEvaluator = boardManager.OilEvaluator;

        // ReportOilValues();
    }



    private void OnDrawGizmos()
    {
        if (_oilTileMap == null) {
            _oilTileMap = GetComponent<Tilemap>();
        }

        for (int x = 0; x < BoardManager.MAP_SIZE_X; x++)
        {
            for (int y = 0; y < BoardManager.MAP_SIZE_Y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                float oilValue = BoardManager.Instance.OilEvaluator.GetValueAtPosition(x, y);
                if (oilValue > 0) // Draw only if there's oil
                {
                    DrawRedSquare(tilePos, oilValue);
                    if (oilValue > highestOilValueSofar) {
                        highestOilValueSofar = oilValue;
                    }
                }
            }
        }
    }

    private void DrawRedSquare(Vector3Int tilePos, float oilValueInput)
    {
        float colorPercent = oilValueInput/highestOilValueSofar;
        Color gizmoPercentColor = oilGradient.Evaluate(colorPercent);

        Vector3 worldPos = _oilTileMap.CellToWorld(tilePos);
        Gizmos.color = gizmoPercentColor;
        Gizmos.DrawCube(worldPos + _oilTileMap.cellSize / 2, _oilTileMap.cellSize);
    }

}
