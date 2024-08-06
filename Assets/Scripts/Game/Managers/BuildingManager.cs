using System.Collections;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager>
{
    private IPlacer _currentPlacer;
    private PipeSpriteScript m_pipeSprites;
    private BuildingScriptableObject _currentBuildingSO;

    private Coroutine _coroutine;

    private void Awake()
    {
        m_pipeSprites = GetComponent<PipeSpriteScript>();
    }

    public void OnMouseClick()=>_currentPlacer?.PressMouse();

    public void BeginBuilding(BuildingScriptableObject SO)
    {
        CancelBuilding();
        UIStateMachine.Instance.ChangeState(GameState.BuildingUI);
        _currentBuildingSO = SO;
        
        _currentPlacer = Instantiate(SO.previewPrefab).GetComponent<BuildingPlacer>();
        _currentPlacer.InitSO(SO);

        _coroutine = StartCoroutine(IEDoBuildingProcess());
    }

    public void CancelBuilding()
    {
        _currentPlacer?.Cleanup();
        _currentPlacer = null;

        _currentBuildingSO = null;
        _coroutine = null;
        
        UIStateMachine.Instance.ChangeState(GameState.GameUI);
    }

    private IEnumerator IEDoBuildingProcess()
    {
        yield return _currentPlacer.IEDoBuildProcess();

        CancelBuilding();
    }

    public PipeSpriteScript.PipeRotation GetPipeRotation(Vector2Int in_pos, Vector2Int pos, Vector2Int out_pos)
    {
        return m_pipeSprites.OrientPipes(in_pos, pos, out_pos);
    }
}