using System.Collections;

public interface IPlacer
{
    void InitSO(BuildingScriptableObject so);

    void UpdatePreview();

    bool IsValidPlacement(BuildingScriptableObject so);

    IEnumerator IEDoBuildProcess();

    void Cleanup();
}
