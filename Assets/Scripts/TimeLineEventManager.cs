using System.Collections.Generic;
using UnityEngine;

public class TimeLineEventManager : Singleton<TimeLineEventManager>, ITickReceiver
{
    [SerializeField]
    private Vector2 yearRange;
    [SerializeField] private float currentYear;
    
    [SerializeField] private float ticksPerYear;
    [SerializeField] private float currentTick;
    
    [SerializeField] private List<GameObject> events;
    public List<GameObject> Events => events;
    
    [SerializeField] private int currentEventIndex;
    
    [SerializeField] private RectTransform timelineSlider;
    [SerializeField] private GameObject newsPaperPrefabImage;
    [SerializeField] private SingleSoundPlayer newsSFXPlayer;
    
    private int _ticksElapsed;
    private int _totalTicks;
    private float _totalYears;
    
    public delegate void EventTriggered();
    public event EventTriggered OnEventTrigger;

    private void Start()
    {
        _totalYears = yearRange.y - yearRange.x;
        TimeManager.Instance.RegisterReceiver(this);
        currentYear = yearRange.x;
        DisplayEvents();

        _ticksElapsed = 0;
        _totalTicks = (int)((yearRange.y - currentYear) * ticksPerYear);

        if (ticksPerYear <= 0f)
        {
            Debug.LogError("You must have a passage of time greater than 0f");
        }
    }

    private void DisplayEvents()
    {
        foreach (var gameEvent in events)
        {
            DisplayIndividualEvent(gameEvent.GetComponent<SingleEventController>());
        }
    }
    
    private void DisplayIndividualEvent(SingleEventController newsEvent) {
        Vector3[] worldCorners = new Vector3[4];
        timelineSlider.GetWorldCorners(worldCorners);
        Vector3 start = worldCorners[0]; // Bottom-left corner
        Vector3 end = worldCorners[2]; // Top-right corner (assuming horizontal slider)

        var targetPercent = (newsEvent.TriggerYear - yearRange.x) / _totalYears;
        Debug.Log(targetPercent);
        var sliderWidth = end.x - start.x;
        var newX = start.x + (sliderWidth * targetPercent);
        var newY = timelineSlider.transform.position.y;
        var newZ = timelineSlider.transform.position.z;
        
        var matchedPercentagePosition = new Vector3(newX, newY, newZ);

        var newspaperObject = Instantiate(newsPaperPrefabImage, matchedPercentagePosition, timelineSlider.gameObject.transform.rotation);
        newspaperObject.transform.SetParent(timelineSlider.transform, true);

        newsSFXPlayer.ActivateWithForeignTrigger();
    }
    
    private void ContinueTimeLine()
    {
        currentTick++;
        if (currentTick >= ticksPerYear)
        {
            if (ticksPerYear <= 0)
            {
                Debug.LogWarning("ticksPerYear not set up correctly");
                return;
            }
            currentTick = 0f;
            currentYear++;
            CheckNextEvent();
        }
    }
    
    // This is the method that checks for the next newspaper event,
    // and has the UI pop up
    private void CheckNextEvent()
    {
        if (CheckForEndGame())
        {
            GameStateManager.Instance.EndGame();
            return;
        }
        
        if (currentEventIndex <= events.Count - 1)
        {
            if (currentYear == events[currentEventIndex].GetComponent<SingleEventController>().TriggerYear)
            {
                currentEventIndex++;
                TriggerNextEvent();
            }
        }
    }

    private void TriggerNextEvent()
    {
        OnEventTrigger?.Invoke();
        UIStateMachine.Instance.ChangeState(GameState.EventUI);
    }

    public bool CheckForEndGame()
    {
        var isNewsPaperUp = UIStateMachine.Instance.CurrentStateType == GameState.EventUI;
        if (!isNewsPaperUp && GetTimePercentage() >= 1f)
        {
            return true;
        }
        return false;
    }
    
    public void OnTick()
    {
        ContinueTimeLine();
        _ticksElapsed++;
    }

    private void OnDisable()
    {
        // Deregister this manager to stop receiving ticks
        TimeManager.Instance.DeregisterReceiver(this);
    }

    public float GetTimePercentage()
    {
        var percent = (_totalYears - (yearRange.y - currentYear)) / _totalYears;
        return percent;
    }
}
