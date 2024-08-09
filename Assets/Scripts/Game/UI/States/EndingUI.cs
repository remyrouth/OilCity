using UnityEngine;
using System.Collections; // Add this for IEnumerator
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class EndingUI : UIState
{
    [SerializeField] private SingleGraphView[] _graphs;
    public override GameState type => GameState.EndingUI;
    [SerializeField] private Transform _graphPivot;
    [SerializeField] private Image _background;
    [SerializeField] private SingleSoundPlayer SFXLossPlayer;



    private Vector3 startPosition;

    public override void OnEnter()
    {
        base.OnEnter();
        foreach (var graph in _graphs)
            graph.PopulateGraph();
        TimeManager.Instance.TicksPerMinute = 0;

        //StartCoroutine(GameRecorder.Instance.DoRollback());

        startPosition = transform.position;
        _graphPivot.position = new Vector3(startPosition.x, startPosition.y + 800f, startPosition.z);

        _graphPivot.DOMove(startPosition, 2);
        _background.DOColor(new Color(0, 0, 0, 0.75f), 1);
        SoundManager.Instance.PauseContinuousSounds();
        SFXLossPlayer.ActivateWithForeignTrigger();
    }


}
