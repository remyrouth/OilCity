using System.Collections.Generic;
using Game.Tutorial;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private List<GameObject> tutorialSteps;
    private int _nextStepIndex;
    [SerializeField] private bool tutorialEnabled;

    public bool TutorialEnabled
    {
        get => tutorialEnabled;
        set => tutorialEnabled = value;
    }
    
    private bool _inTutorial;

    public bool InTutorial
    {
        get => _inTutorial;
        set => _inTutorial = value;
    }

    private void Start()
    {
        _nextStepIndex = 0;
        if(TutorialEnabled && !SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
            _inTutorial = true;
            GoToNextStep();
        }
    }

    public void GoToNextStep()
    {
        Instantiate(tutorialSteps[_nextStepIndex], transform).GetComponent<TutorialStep>();
        _nextStepIndex++;
    }
}
