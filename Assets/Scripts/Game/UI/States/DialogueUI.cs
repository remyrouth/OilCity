using System;
using Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : Singleton<DialogueUI>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image portrait;
    [SerializeField] private Image clickIndicator;
    [SerializeField] private GameObject arrow;

    public GameObject Arrow
    {
        get => arrow;
        set => arrow = value;
    }

    private DialogueSO _currentDialogue;

    public DialogueSO CurrentDialogue => _currentDialogue;

    public event Action OnDialogueClicked;
    
    public void EnableDialogue()
    {
        panel.SetActive(true);
    }
    
    public void ChangeText(DialogueSO dialogueText)
    {
        _currentDialogue = dialogueText;
        ChangeTextLanguage();
    }
    
    public void ChangeTextLanguage()
    {
        text.text = SettingsManager.Instance.CurrentLanguage switch
        {
            Language.English => _currentDialogue.DialogueTextEnglish,
            Language.Polish => _currentDialogue.DialogueTextPolish,
            _ => _currentDialogue.DialogueTextEnglish
        };
    }

    public void ChangePortraitImage(Sprite image)
    {
        portrait.sprite = image;
    }
    
    public void DisableDialogue()
    {
        panel.SetActive(false);
    }
    
    public void ToggleIndicator()
    {
        clickIndicator.gameObject.SetActive(!clickIndicator.gameObject.activeSelf);
    }
    
    public void EnableArrow(string animationName)
    {
        arrow.GetComponent<CanvasGroup>().alpha = 1;
        arrow.GetComponentInChildren<Animator>(true).Play(animationName);
    }
    public void DisableArrow()
    {
        arrow.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void ClickDialogue()
    {
        OnDialogueClicked?.Invoke();
    }
}
