using TMPro;
using UnityEngine;

public class OilPriceListenerView : MonoBehaviour, ITickReceiver
{
    [SerializeField] private TMP_Text _label;

    public void OnTick()
    {
        _label.text = KeroseneManager.Instance.GetKerosenePrice().ToString("0.00") + "/ <sprite name=kerosene>";
    }

    private void Start()
    {
        TimeManager.Instance.RegisterReceiver(this);
        _label.text = KeroseneManager.Instance.GetKerosenePrice().ToString("0.00") + "/ <sprite name=kerosene>";
    }
}
