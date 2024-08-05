using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuotaTimerListenerView : MonoBehaviour, ITickReceiver
{
    [SerializeField] private TMP_Text _label;
    private int _currentValue = 0;

    public void OnTick()
    {
        if (_currentValue == 0)
        {
            if (QuotaManager.Instance.currentQuota > 0)
            {
                GameStateManager.Instance.EndGame();
                return;
            }
            else
                QuotaManager.Instance.SetQuota();
            _currentValue = QuotaManager.Instance.timeTillQuotaDeadline;
        }
        else
            _currentValue--;

        _label.text = _currentValue.ToString();
    }
    private void Start()
    {
        TimeManager.Instance.RegisterReceiver(this);
        _currentValue = QuotaManager.Instance.timeTillQuotaDeadline;
        _label.text = _currentValue.ToString();
    }

}
