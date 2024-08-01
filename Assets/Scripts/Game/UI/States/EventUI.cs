using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventUI : UIState
{
    public override GameState type => GameState.EventUI;
    private TimeLineEvent _currentEvent;
    [SerializeField] private Image _image;


    private void Start()
    {
        ControlManager.Instance.leftClickActivationButtontrigger += MouseClick;
    }
    private void OnDestroy()
    {
        ControlManager.Instance.leftClickActivationButtontrigger -= MouseClick;
    }
    private void MouseClick()
    {
        if (UIStateMachine.Instance.CurrentStateType != type)
            return;
        UIStateMachine.Instance.ChangeState(GameState.GameUI);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _image.sprite = _currentEvent.newspaperSprite;
        TimeManager.Instance.TicksPerMinute = 0;
    }

    public override void OnExit()
    {
        base.OnExit();
        TimeManager.Instance.TicksPerMinute = 60;
    }
    public void TriggerEvent(TimeLineEvent nextEvent)
    {
        _currentEvent = nextEvent;
        UIStateMachine.Instance.ChangeState(type);
    }
}
