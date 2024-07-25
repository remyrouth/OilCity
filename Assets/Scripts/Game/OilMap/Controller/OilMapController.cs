using System.Collections.Generic;
using UnityEngine;

public abstract class OilMapController : MonoBehaviour, IAmountGiver<float>
{
    protected Dictionary<Vector2Int, float> alreadyMined = new();

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
        if (alreadyMined.ContainsKey(new Vector2Int(x, y)))
            return GetBaseValue(x, y) - alreadyMined[new Vector2Int(x, y)];
        return GetBaseValue(x, y);
    }
}
