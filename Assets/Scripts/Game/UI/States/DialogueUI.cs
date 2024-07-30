using Game.Managers;
using TMPro;
using UnityEngine;

public class DialogueUI : Singleton<DialogueUI>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;
    
    public void EnableDialogue()
    {
        panel.SetActive(true);
    }
    
    public void ChangeText(DialogueSO dialogueText)
    {
        text.text = SettingsManager.Instance.CurrentLanguage switch
        {
            Language.English => dialogueText.DialogueTextEnglish,
            Language.Polish => dialogueText.DialogueTextPolish,
            _ => dialogueText.DialogueTextEnglish
        };
    }

    public void DisableDialogue()
    {
        panel.SetActive(false);
    }
}
