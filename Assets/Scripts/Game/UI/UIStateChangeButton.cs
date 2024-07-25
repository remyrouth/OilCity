using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIStateChangeButton : MonoBehaviour
{
    [SerializeField] private GameState targetState;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { UIStateMachine.Instance.ChangeState(targetState); });
    }
}

