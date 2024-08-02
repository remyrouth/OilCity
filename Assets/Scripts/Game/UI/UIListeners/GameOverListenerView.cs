
using TMPro;
using UnityEngine;

public class GameOverListenerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;

    private void Awake()
    {
        GameStateManager.Instance.OnGameEnded += UpdateLabel;
    }
    private void Start()
    {
        _label.text = "Game over...";
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameEnded -= UpdateLabel;
    }
    private void UpdateLabel()
    {
        _label.text = CheckLossReason();
    }
    private string CheckLossReason()
    {
        if (TimeLineEventManager.Instance.CheckForEndGame())
        {
            return "The polish oil industry is in shambles... Game over.";
        }
        if(MoneyManager.Instance.Money <= 0)
        {
            return "You're broke... Game over.";
        }
        if(WorkerSatisfactionManager.Instance.WorkerSatisfaction <= 0)
        {
            return "You failed to keep your workers satisfied... Game over.";
        }
        return "nwm nwm";
    }
}
