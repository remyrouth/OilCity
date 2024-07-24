using UnityEngine;

public class OilDebugger : MonoBehaviour
{
    [SerializeField]
    private Gradient oilGradient;

    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < BoardManager.MAP_SIZE_X; x++)
        {
            for (int y = 0; y < BoardManager.MAP_SIZE_Y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                float oilValue = BoardManager.Instance.OilEvaluator.GetValueAtPosition(x, y);
                DrawRedSquare(tilePos, oilValue);
            }
        }
    }

    private void DrawRedSquare(Vector3Int tilePos, float oilValueInput)
    {
        Color gizmoPercentColor = oilGradient.Evaluate(oilValueInput);

        Vector3 worldPos = tilePos;
        Gizmos.color = gizmoPercentColor;
        Gizmos.DrawCube(worldPos + new Vector3(0.5f,0.5f,0),Vector3.one);
    }

}
