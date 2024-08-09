using System.Collections.Generic;
using UnityEngine;

public abstract class OilMapController : MonoBehaviour, IAmountGiver<float>
{
    protected Dictionary<Vector2Int, float> alreadyMined = new();
    protected HashSet<Vector2Int> searched = new();
    private const float NOT_SEARCHED_PENALTY_MULTIPLIER = 0.25f;
    public void SetPositionAsSearched(Vector2Int pos)
    {
        if (!searched.Contains(pos))
            searched.Add(pos);
    }
    public void IncreaseAmountMinedAtPosition(int x, int y, float amount)
    {
        if (!alreadyMined.ContainsKey(new Vector2Int(x, y)))
            alreadyMined.Add(new Vector2Int(x, y), 0);
        alreadyMined[new Vector2Int(x, y)] += amount;
    }
    protected abstract float GetBaseValue(int x, int y);
    /// <summary>
    /// Returns the amount of oil that can be found at tile
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public float GetValueAtPosition(int x, int y)
    {
        float value = GetBaseValue(x, y)*0.5f;
        if (alreadyMined.ContainsKey(new Vector2Int(x, y)))
            value -= alreadyMined[new Vector2Int(x, y)];
        if (!searched.Contains(new Vector2Int(x, y)))
            value *= NOT_SEARCHED_PENALTY_MULTIPLIER;
        return Mathf.Clamp01(value);
    }
}
