using UnityEngine;

public class AOEBuildingPlacer : BuildingPlacer
{
    private BuildingPreviewRangeShower _rangeShower;
    private Vector3 _previousPos = -Vector3.one;
    public override void UpdatePreview()
    {
        base.UpdatePreview();
        if (!this) return;
        if (_previousPos == transform.position)
            return;
        _previousPos = transform.position;
        if (_rangeShower == null)
            _rangeShower = GetComponent<BuildingPreviewRangeShower>();
        _rangeShower.HideRadius();
        _rangeShower.ShowRadius(m_so as AOEBuildingScriptableObject);
    }
}
