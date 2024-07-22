using UnityEngine;

public abstract class TreeMap : MonoBehaviour, IAmountGiver<bool>
{
    public abstract bool GetValueAtPosition(int x, int y);
}
