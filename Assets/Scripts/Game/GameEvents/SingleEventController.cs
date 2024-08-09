using UnityEngine;

public class SingleEventController : MonoBehaviour
{
    [SerializeField] private float triggerYear;
    
    public float TriggerYear => triggerYear;

    public void OnButtonClick()
    {
        UIStateMachine.Instance.ChangeState(GameState.GameUI); 
        Destroy(gameObject);
    }
}
