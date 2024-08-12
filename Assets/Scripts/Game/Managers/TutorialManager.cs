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

    public void StartTutorial()
    {
        _nextStepIndex = 0;
        if(TutorialEnabled && !SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
            _inTutorial = true;
            GoToNextStep();
            DialogueUI.Instance.EnableDialogue();
        }
    }
    private TutorialStep _currentStep = null;
    // public void GoToNextStep()
    // {
    //     if (_currentStep!= null)
    //         _currentStep.Deinitialize();
    //     _currentStep = Instantiate(tutorialSteps[_nextStepIndex], transform).GetComponent<TutorialStep>();
    //     _currentStep.Initialize();
    //     _nextStepIndex++;
    // }

    public void GoToNextStep()
    {
        if (_currentStep != null)
            _currentStep.Deinitialize();

        if (_nextStepIndex < tutorialSteps.Count)
        {
            _currentStep = Instantiate(tutorialSteps[_nextStepIndex], transform).GetComponent<TutorialStep>();
            _currentStep.Initialize();
            _nextStepIndex++;
        }
        else
        {
            // Debug.Log("Tutorial completed.");
            _inTutorial = false;
            OnTutorialCompleted();
        }
    }

    private void OnTutorialCompleted()
    {
        // SoundManager.Instance.PauseContinuousSounds();
        SoundManager.Instance.BeginMusicTrackFromTutorial();
    }
}
