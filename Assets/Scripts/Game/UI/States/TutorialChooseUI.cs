using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialChooseUI : UIState
{
    public override GameState type => GameState.ChoosingTutorialUI;
    [SerializeField] private Button yesButton, noButton;
    private void Awake()
    {
        yesButton.onClick.AddListener(() => { 
            // SoundManager.Instance.PauseContinuousSounds();
            TutorialManager.Instance.TutorialEnabled = true; GoToGame(); 
        });

        noButton.onClick.AddListener(() => { 
            // SoundManager.Instance.PlayContinuousSounds();
            SoundManager.Instance.BeginMusicTrackFromTutorial();
            TutorialManager.Instance.TutorialEnabled = false; GoToGame(); 
        });

        normalTickRate = TimeManager.Instance.TicksPerMinute;
    }

    private void Start() {
        SoundManager.Instance.PauseContinuousSounds();
    }
    private int normalTickRate;
    public override void OnEnter()
    {
        base.OnEnter();
        TimeManager.Instance.TicksPerMinute = 0;
        BuildingPanelUI.Instance.DisableAllButtons();

    }
    public override void OnExit()
    {
        base.OnExit();
        TimeManager.Instance.TicksPerMinute = normalTickRate;
        TutorialManager.Instance.StartTutorial();
    }
    private void GoToGame()
    {
        UIStateMachine.Instance.ChangeState(GameState.GameUI);
    }
}
