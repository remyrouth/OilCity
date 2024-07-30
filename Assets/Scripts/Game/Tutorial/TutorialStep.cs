using UnityEngine;

public class TutorialStep : MonoBehaviour
{
    [SerializeField] private DialogueSO dialogueText;
    
    public void OnEnable()
    {
        DialogueUI.Instance.ChangeText(dialogueText);
    }

    public void FinishStep()
    {
        TutorialManager.Instance.GoToNextStep();
        Destroy(gameObject);
    }
}
