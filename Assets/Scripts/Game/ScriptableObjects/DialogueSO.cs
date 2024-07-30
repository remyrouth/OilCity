using UnityEngine;

[CreateAssetMenu]
public class DialogueSO : ScriptableObject
{
    [SerializeField][TextArea] private string dialogueTextEnglish;
    public string DialogueTextEnglish => dialogueTextEnglish;
    
    [SerializeField][TextArea] private string dialogueTextPolish;
    public string DialogueTextPolish => dialogueTextPolish;
}
