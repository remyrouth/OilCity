using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private List<GameObject> tutorialSteps;
    private int _nextStepIndex;
    [SerializeField] private bool enabled;

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    private void Awake()
    {
        _nextStepIndex = 0;
        if(Enabled && !SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
            //Todo disable building spawning
            GoToNextStep();
        }
    }

    public void GoToNextStep()
    {
        Instantiate(tutorialSteps[_nextStepIndex], transform).GetComponent<TutorialStep>();
        _nextStepIndex++;
    }
}
