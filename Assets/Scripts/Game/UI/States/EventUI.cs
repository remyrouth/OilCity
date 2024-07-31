using UnityEngine;
using UnityEngine.UI;

public class EventUI : UIState
{
    public override GameState type => GameState.EventUI;
    private TimeLineEvent _currentEvent;
    [SerializeField] private Image _image;
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
    public override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
            UIStateMachine.Instance.ChangeState(GameState.GameUI);
    }
    public void TriggerEvent(TimeLineEvent nextEvent)
    {
        _currentEvent = nextEvent;
        UIStateMachine.Instance.ChangeState(type);
    }
}
