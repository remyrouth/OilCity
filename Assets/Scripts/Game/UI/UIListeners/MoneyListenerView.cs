using TMPro;
using UnityEngine;

public class MoneyListenerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;
    private float _currentValue = 0;
    private float _targetValue = 0;
    private void Awake()
    {
        MoneyManager.Instance.OnMoneyChanged += UpdateLabel;
    }
    private void Start()
    {
        _label.text = _currentValue.ToString("0.00") + " z³";
    }
    private void OnDestroy()
    {
        MoneyManager.Instance.OnMoneyChanged -= UpdateLabel;
    }
    private const int DELTA = 10;
    private void FixedUpdate()
    {
        if (_targetValue == _currentValue)
            return;
        _currentValue += DELTA * Mathf.Sign(_targetValue - _currentValue);
        if (Mathf.Abs(_currentValue - _targetValue) < DELTA)
            _currentValue = _targetValue;
        _label.text = _currentValue.ToString("C");
    }
    private void UpdateLabel(float newWSvalue)
    {
        _targetValue = newWSvalue;
    }
}
