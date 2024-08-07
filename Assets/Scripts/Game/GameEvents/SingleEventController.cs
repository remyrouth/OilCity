using UnityEngine;

public class SingleEventController : MonoBehaviour
{
    [SerializeField] private float triggerYear;
    
    public float TriggerYear => triggerYear;
    
    protected virtual void OnEnable()
    {
        ControlManager.Instance.leftClickActivationButtontrigger += MouseClick;
    }
    
    private void OnDisable()
    {
        ControlManager.Instance.leftClickActivationButtontrigger -= MouseClick;
    }
    
    private void MouseClick()
    {
        Destroy(gameObject);
    }
}
