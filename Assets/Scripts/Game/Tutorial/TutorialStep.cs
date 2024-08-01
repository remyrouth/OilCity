using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialStep : MonoBehaviour
    {
        [SerializeField] private DialogueSO dialogueText;
    
        public void OnEnable()
        {
            DialogueUI.Instance.ChangeText(dialogueText);
            BuildingPanelUI.Instance.DisableAllButtons();
        }

        protected void FinishStep()
        {
            TutorialManager.Instance.GoToNextStep();
            Destroy(gameObject);
        }
    }

}
