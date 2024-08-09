using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EventButton : MonoBehaviour
{
    [SerializeField] private GameState targetState;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { 
            UIStateMachine.Instance.ChangeState(targetState); 
            
            if (targetState == GameState.GameUI) {
                SoundManager.Instance.PlayContinuousSounds();
            } else if (targetState == GameState.PauseUI) {
                SoundManager.Instance.PauseContinuousSounds();
            }
        });
    }
}
