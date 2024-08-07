using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialStep : MonoBehaviour
    {
        [SerializeField] private DialogueSO dialogueText;
        [SerializeField] private Sprite portraitImage;
    
        public virtual void Initialize()
        {
            DialogueUI.Instance.ChangeText(dialogueText);
            BuildingPanelUI.Instance.DisableAllButtons();
            DialogueUI.Instance.ChangePortraitImage(portraitImage);
        }
        public virtual void Deinitialize() { }

        protected void FinishStep()
        {
            TutorialManager.Instance.GoToNextStep();
            Destroy(gameObject);
        }
    }

}
