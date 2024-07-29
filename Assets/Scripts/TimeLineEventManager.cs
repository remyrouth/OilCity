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
    private float currentyear;
    [SerializeField]
    private float ticksPerYear = 1f;
    [SerializeField]
    private float currentTick = 4f;
    [SerializeField]
    private Image eventImageObject;
    [SerializeField]
    private List<TimeLineEvent> eventsOnTimeLine = new List<TimeLineEvent>();
    [SerializeField]
    private int currentEventListIndex = 0;

    private int m_ticksElapsed;
    private int m_totalTicks;

    private void Start()
    {
        // Register this manager to receive ticks
        TimeManager.Instance.RegisterReceiver(gameObject);
        currentyear = yearRange.x;
        SortEventsByPercentage();

        m_ticksElapsed = 0;
        m_totalTicks = (int)((yearRange.y - currentyear) * ticksPerYear);

        if (ticksPerYear <= 0f) {
            Debug.LogError("You must have a passage of time greater than 0f");
        }
    }


    public void EndTimeLineEvent() {
        eventImageObject.gameObject.SetActive(false);
    }

    private void ContinueimeLine() {
        currentTick++;
        if (currentTick >= ticksPerYear) {
            currentTick = 0f;
            currentyear++;
            CheckNextEvent();
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && eventImageObject.gameObject.activeSelf)
        {
            eventImageObject.sprite = null;
            eventImageObject.gameObject.SetActive(false);
        }
    }

    private void CheckNextEvent() {

        if (currentEventListIndex <= eventsOnTimeLine.Count - 1) {
            float currentPercent = currentyear/yearRange.y;
            TimeLineEvent nextEvent = eventsOnTimeLine[currentEventListIndex];
            float nextEventPercent = nextEvent.GamePercentage;

            if (currentPercent >= nextEventPercent) {
                currentEventListIndex++;
                TriggerNextEvent(nextEvent);
            }
        }

    }

    private void TriggerNextEvent(TimeLineEvent nextEvent) {
        eventImageObject.sprite = nextEvent.newspaperSprite;
        eventImageObject.gameObject.SetActive(true);

    }


    public void OnTick()
    {
        ContinueimeLine();
        KeroseneManager.Instance.SetFalloffPercentage(m_ticksElapsed / (float)m_totalTicks);

        m_ticksElapsed++;
    }

    private void SortEventsByPercentage() {
        eventsOnTimeLine.Sort((a, b) => a.GamePercentage.CompareTo(b.GamePercentage));
    }

    private void OnDisable()
    {
        // Deregister this manager to stop receiving ticks
        TimeManager.Instance.DeregisterReceiver(gameObject);
        eventsOnTimeLine.Clear();
    }
}
