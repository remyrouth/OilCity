using Game.Events;
using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialPipeStep : TutorialStep
    {
        [SerializeField] private int buttonToUnlockIndex;
        [SerializeField] private BuildingScriptableObject startBuilding, endBuilding;
        public override void Initialize()
        {
            PipeEvents.OnPipePlaced += FinishStep;
            base.Initialize();
            BuildingPanelUI.Instance.ToggleButtonInteractableWithHighlight(buttonToUnlockIndex);
            PipePlacer.IsValidPlaceOverride = OverriddenPipePredicate;
        }

        public override void Deinitialize()
        {
            PipePlacer.IsValidPlaceOverride = null;
            PipeEvents.OnPipePlaced -= FinishStep;
        }

        private new void FinishStep()
        {
            BuildingPanelUI.Instance.ToggleHighlight(buttonToUnlockIndex);
            base.FinishStep();
        }
        private bool OverriddenPipePredicate((Vector2Int, bool) values)
        {
            if (!BoardManager.Instance.tileDictionary.TryGetValue(values.Item1, out var toc))
                return false;
            return toc.Config?.Equals(values.Item2 ? endBuilding : startBuilding) ?? false;
        }
    }

}
