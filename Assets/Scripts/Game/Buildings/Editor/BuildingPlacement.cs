using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OilCity {
    public class BuildingPlacementTool : MonoBehaviour {

        [MenuItem("BuildingPlacement/RefreshBuildings")]
        static void RefreshBuildings() {
            //var buildings = FindObjectsOfType<BasicBuilding>();
            //foreach (var element in buildings) {
            //    element.OnBuild();
            //}
        }

        [MenuItem("BuildingPlacement/PlaceBuilding")]
        static void PlaceBuilding() {
            Vector2Int position = new Vector2Int(5, 5);
            //BuildingScriptableObject buildingSO = BuildingScriptableObject();
        }

    }
}