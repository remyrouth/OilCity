
using TMPro;
using UnityEngine;

public class GameOverListenerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] private LanguageItem _moneyLoss;
    [SerializeField] private LanguageItem _satifactionLoss;
    [SerializeField] private LanguageItem _timeLoss;

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
            return _timeLoss.ToString();
        }
        if(MoneyManager.Instance.Money <= 0)
        {
            return _moneyLoss.ToString();
        }
        if(WorkerSatisfactionManager.Instance.WorkerSatisfaction <= 0)
        {
            return _satifactionLoss.ToString();
        }
        return "Game over...";
    }
}
