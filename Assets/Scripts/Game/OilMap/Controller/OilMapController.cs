using System.Collections.Generic;
using UnityEngine;

public abstract class OilMapController : MonoBehaviour, IAmountGiver<float>
{
    protected Dictionary<Vector2Int, float> alreadyMined = new();

    public void IncreaseAmountMinedAtPosition(int x, int y, float amount)
    {
        alreadyMined[new Vector2Int(x, y)] += amount;
    }
    protected abstract float GetBaseValue(int x, int y);
    public float GetValueAtPosition(int x, int y)
    {
        return GetBaseValue(x, y) - alreadyMined[new Vector2Int(x,y)];
    }
}
