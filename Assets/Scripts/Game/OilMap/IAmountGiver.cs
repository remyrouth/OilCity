using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmountGiver<T>
{
    T GetValueAtPosition(int x, int y);
}
