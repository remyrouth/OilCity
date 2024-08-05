using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OilPriceListenerView : MonoBehaviour, ITickReceiver
{
    [SerializeField] private TMP_Text _label;

    public void OnTick()
    {
        _label.text = (KeroseneManager.Instance.EvaluateFalloffCurve() * KeroseneManager.KEROSINE_PRICE).ToString("0.00") + "/L";
    }

    private void Start()
    {
        TimeManager.Instance.RegisterReceiver(this);
        _label.text = (KeroseneManager.Instance.EvaluateFalloffCurve() * KeroseneManager.KEROSINE_PRICE).ToString("0.00") + "/L";
    }
}
