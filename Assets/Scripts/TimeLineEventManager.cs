using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineEventManager : MonoBehaviour, ITickReceiver
{
    [SerializeField]
    private Vector2 yearRange;
    [SerializeField]
    private float currentYear;
    private float totalYears;
    [SerializeField]
    private float ticksPerYear = 1f;
    [SerializeField]
    private float currentTick = 4f;
    [SerializeField]
    private Image eventImageObject;
    [SerializeField]
    private Slider timelineSlider;
    [SerializeField]
    private GameObject newsPaperPrefabImage;

    [SerializeField]
    private List<TimeLineEvent> eventsOnTimeLine = new List<TimeLineEvent>();
    [SerializeField]
    private int currentEventListIndex = 0;

    private int m_ticksElapsed;
    private int m_totalTicks;

    private void Start()
    {
        totalYears = yearRange.y - yearRange.x;

        // Register this manager to receive ticks
        TimeManager.Instance.RegisterReceiver(this);
        currentYear = yearRange.x;
        SortEventsByPercentage();
        DisplayEvents();

        m_ticksElapsed = 0;
        m_totalTicks = (int)((yearRange.y - currentYear) * ticksPerYear);

        if (ticksPerYear <= 0f) {
            Debug.LogError("You must have a passage of time greater than 0f");
        }

        if (timelineSlider == null) {
            Debug.LogError("timelineSlider not attached");
        }
    }

    private void DisplayEvents() {
        foreach (TimeLineEvent newsEvent in eventsOnTimeLine) {
            DisplayIndividualEvent(newsEvent);
        }

    }

    private void DisplayIndividualEvent(TimeLineEvent newsEvent) {
        Vector3[] worldCorners = new Vector3[4];
        timelineSlider.GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        Vector3 start = worldCorners[0]; // Bottom-left corner
        Vector3 end = worldCorners[2]; // Top-right corner (assuming horizontal slider)

        float targetPercent = newsEvent.GamePercentage;
        float sliderWidth = end.x - start.x;
        float newX = start.x + (sliderWidth * targetPercent);
        float newY = timelineSlider.transform.position.y;
        float newZ = timelineSlider.transform.position.z;

        Vector3 matchedPercentagePosition = new Vector3(newX, newY, newZ);

        GameObject newspaperObject = Instantiate(newsPaperPrefabImage,
        matchedPercentagePosition,
        timelineSlider.gameObject.transform.rotation);

        newspaperObject.transform.SetParent(timelineSlider.transform, true);
    }


    public void EndTimeLineEvent() {
        eventImageObject.gameObject.SetActive(false);
    }

    private void ContinueTimeLine() {
        currentTick++;
        if (currentTick >= ticksPerYear) {
            if (ticksPerYear <= 0 && timelineSlider != null) {
                Debug.LogWarning("ticksPerYear or timelineSlider not set up correctly");
                return;
            }
            AlterSlider();
            MoneyManager.Instance.ReduceMoney(MoneyManager.Instance.Quota);
            currentTick = 0f;
            currentYear++;
            CheckNextEvent();
            AlterPollutionInstance();
        }
    }

    private void AlterPollutionInstance() {
        PollutionManager.Instance.SetPollution(GetTimePercentage());
    }

    private void AlterSlider() {
        timelineSlider.value = GetTimePercentage();
    }

    // Simply us checking if there is UI already present, if so, turn it off
    private void Update() {
        if (Input.GetMouseButtonDown(0) && eventImageObject.gameObject.activeSelf)
        {
            eventImageObject.sprite = null;
            eventImageObject.gameObject.SetActive(false);
        }
    }

    // This is the method that checks for the next newspaper event,
    // and has the UI pop up
    private void CheckNextEvent() {
        if (CheckForEndGame()) {
            GameStateManager.Instance.EndGame();
            return;
        }


        if (currentEventListIndex <= eventsOnTimeLine.Count - 1) {
            TimeLineEvent nextEvent = eventsOnTimeLine[currentEventListIndex];
            float nextEventPercent = nextEvent.GamePercentage;

            if (GetTimePercentage() >= nextEventPercent) {
                currentEventListIndex++;
                TriggerNextEvent(nextEvent);
            }
        }

    }

    private void TriggerNextEvent(TimeLineEvent nextEvent) {
        eventImageObject.sprite = nextEvent.newspaperSprite;
        eventImageObject.gameObject.SetActive(true);

    }

    private bool CheckForEndGame() {
        bool isNewsPaperUp = eventImageObject.gameObject.activeInHierarchy;
        if (!isNewsPaperUp && GetTimePercentage() >= 1f) {
            return true;
        }


        return false;
    }


    public void OnTick()
    {
        ContinueTimeLine();
        KeroseneManager.Instance.SetFalloffPercentage(m_ticksElapsed / (float)m_totalTicks);

        m_ticksElapsed++;
    }

    private void SortEventsByPercentage() {
        eventsOnTimeLine.Sort((a, b) => a.GamePercentage.CompareTo(b.GamePercentage));
    }

    private void OnDisable()
    {
        // Deregister this manager to stop receiving ticks
        TimeManager.Instance.DeregisterReceiver(this);
        eventsOnTimeLine.Clear();
    }

    private float GetTimePercentage() {
        // Debug.Log("totalYears: " +  totalYears);
        float percent = (totalYears - (yearRange.y - currentYear)) / totalYears;
        // Debug.Log("Percent: " +  percent);
        return percent;
    }
}
