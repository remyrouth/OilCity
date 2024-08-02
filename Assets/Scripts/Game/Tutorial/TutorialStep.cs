using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialStep : MonoBehaviour
    {
        [SerializeField] private DialogueSO dialogueText;
        [SerializeField] private Sprite portraitImage;
    
        public void OnEnable()
        {
            DialogueUI.Instance.ChangeText(dialogueText);
            BuildingPanelUI.Instance.DisableAllButtons();
            DialogueUI.Instance.ChangePortraitImage(portraitImage);
        }

        protected void FinishStep()
        {
            TutorialManager.Instance.GoToNextStep();
            Destroy(gameObject);
        }
    }

}
