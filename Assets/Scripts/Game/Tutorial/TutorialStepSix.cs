using Game.Events;

public class TutorialStepSix : TutorialStep
{
    private new void OnEnable()
    {
        PipeEvents.OnPipePlaced += FinishStep;
        base.OnEnable();
    }
    
    private void OnDisable()
    {
        PipeEvents.OnPipePlaced -= FinishStep;
    }
    
    private new void FinishStep()
    {
        base.FinishStep();
    }
}
