using System.Collections.Generic;
using UnityEngine;

public class TimeLineEventManager : Singleton<TimeLineEventManager>, ITickReceiver
{
    [SerializeField]
    private Vector2 yearRange;
    [SerializeField] private float currentYear;
    
    [SerializeField] private float ticksPerYear = 1f;
    [SerializeField] private float currentTick = 4f;

    [SerializeField] private List<TimeLineEvent> eventsOnTimeLine;
    [SerializeField] private int currentEventListIndex;

    [SerializeField] private RectTransform timelineSlider;
    [SerializeField] private GameObject newsPaperPrefabImage;
    
    private int _ticksElapsed;
    private int _totalTicks;
    private float _totalYears;
    
    public delegate void EventTriggered(TimeLineEvent nextEvent);
    public event EventTriggered OnEventTrigger;

    private void Start()
    {
        _totalYears = yearRange.y - yearRange.x;

        // Register this manager to receive ticks
        TimeManager.Instance.RegisterReceiver(this);
        currentYear = yearRange.x;
        SortEventsByPercentage();
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
        foreach (var newsEvent in eventsOnTimeLine)
        {
            DisplayIndividualEvent(newsEvent);
        }
    }
    
    private void DisplayIndividualEvent(TimeLineEvent newsEvent) {
        Vector3[] worldCorners = new Vector3[4];
        timelineSlider.GetWorldCorners(worldCorners);
        Vector3 start = worldCorners[0]; // Bottom-left corner
        Vector3 end = worldCorners[2]; // Top-right corner (assuming horizontal slider)

        float targetPercent = newsEvent.GamePercentage;
        float sliderWidth = end.x - start.x;
        float newX = start.x + (sliderWidth * targetPercent);
        float newY = timelineSlider.transform.position.y;
        float newZ = timelineSlider.transform.position.z;

        var matchedPercentagePosition = new Vector3(newX, newY, newZ);

        var newspaperObject = Instantiate(newsPaperPrefabImage, matchedPercentagePosition, timelineSlider.gameObject.transform.rotation);
        newspaperObject.transform.SetParent(timelineSlider.transform, true);
        newspaperObject.GetComponent<RectTransform>().sizeDelta = Vector2.one*64;
        newspaperObject.GetComponent<RectTransform>().localScale = Vector2.one;
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
        
        if (currentEventListIndex <= eventsOnTimeLine.Count - 1)
        {
            var nextEvent = eventsOnTimeLine[currentEventListIndex];
            var nextEventPercent = nextEvent.GamePercentage;

            if (GetTimePercentage() >= nextEventPercent)
            {
                currentEventListIndex++;
                TriggerNextEvent(nextEvent);
            }
        }
    }

    private void TriggerNextEvent(TimeLineEvent nextEvent)
    {
        OnEventTrigger?.Invoke(nextEvent);
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
        KeroseneManager.Instance.SetFalloffPercentage(_ticksElapsed / (float)_totalTicks);
        _ticksElapsed++;
    }

    private void SortEventsByPercentage()
    {
        eventsOnTimeLine.Sort((a, b) => a.GamePercentage.CompareTo(b.GamePercentage));
    }

    private void OnDisable()
    {
        // Deregister this manager to stop receiving ticks
        TimeManager.Instance.DeregisterReceiver(this);
        eventsOnTimeLine.Clear();
    }

    public float GetTimePercentage()
    {
        var percent = (_totalYears - (yearRange.y - currentYear)) / _totalYears;
        return percent;
    }
}
