using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventSetup : MonoBehaviour, ILanguageChangeable
{
    [SerializeField] private Image eventImage;
    [SerializeField] private TextMeshProUGUI eventTitle;
    [SerializeField] private TextMeshProUGUI eventParagraph;

    private LanguageItem _titleLanguageItem;
    private LanguageItem _paragraphLanguageItem;

    private void OnEnable()
    {
        TimeLineEventManager.Instance.OnEventTrigger += UpdateLanguageItemAndSprite;
    }
    private void OnDisable()
    {
        TimeLineEventManager.Instance.OnEventTrigger -= UpdateLanguageItemAndSprite;
    }

    public void UpdateText()
    {
        if (_titleLanguageItem is not null)
        {
            eventTitle.text = _titleLanguageItem.ToString();
        }
        if (_paragraphLanguageItem is not null)
        {
            eventParagraph.text = _paragraphLanguageItem.ToString();
        }
    }
    
    public void UpdateLanguageItemAndSprite(TimeLineEvent timeLineEvent)
    {
        eventImage.sprite = timeLineEvent.newspaperSprite;
        _titleLanguageItem = timeLineEvent.title;
        _paragraphLanguageItem = timeLineEvent.paragraph;
        UpdateText();
    }
}
