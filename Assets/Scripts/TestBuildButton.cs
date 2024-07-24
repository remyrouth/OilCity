using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBuildButton : MonoBehaviour
{
    [SerializeField]
    private BuildingScriptableObject bso;
    public void build() => bso.BeginBuilding();
}
