using UnityEngine;

public class EventSetup : MonoBehaviour
{
    private int _currentEventIndex;

    private void OnEnable()
    {
        TimeLineEventManager.Instance.OnEventTrigger += UpdateLanguageItemAndSprite;
        _currentEventIndex = 0;
    }
    private void OnDisable()
    {
        if (TimeLineEventManager.Instance != null)
            TimeLineEventManager.Instance.OnEventTrigger -= UpdateLanguageItemAndSprite;
    }

    public void UpdateLanguageItemAndSprite()
    {
        Instantiate(TimeLineEventManager.Instance.Events[_currentEventIndex], transform).GetComponent<SingleEventController>();
        _currentEventIndex++;
    }
}
