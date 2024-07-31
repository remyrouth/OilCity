using System;
using Game.Managers;
using TMPro;
using UnityEngine;

public class DialogueUI : Singleton<DialogueUI>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;
    private DialogueSO _currentDialogue;

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
    

    public void DisableDialogue()
    {
        panel.SetActive(false);
    }

    public void ClickDialogue()
    {
        OnDialogueClicked?.Invoke();
    }
}
