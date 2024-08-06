using TMPro;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoneyListenerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] private GameObject GainMoney;
    [SerializeField] private GameObject LoseMoney;

    private float _currentValue = 0;
    private float _targetValue = 0;
    private float _accumulatedChange = 0;
    private Vector2 _basePos;

    private Color _baseColor;
    private void Awake()
    {
        //was UpdateLabel
        MoneyManager.Instance.OnMoneyChanged += AccumulateChange;
        _currentValue = MoneyManager.Instance.Money;
        _baseColor = _label.color;
        _basePos = _label.rectTransform.anchoredPosition;
    }

    private void Start()
    {
        _label.text = _currentValue.ToString("0.00");
    }

    private void OnDestroy()
    {
        MoneyManager.Instance.OnMoneyChanged -= AccumulateChange;
    }

    private const int DELTA = 10;

    private void FixedUpdate()
    {
        if (_targetValue == _currentValue)
            return;

        _currentValue += DELTA * Mathf.Sign(_targetValue - _currentValue);

        if (Mathf.Abs(_currentValue - _targetValue) < DELTA)
            _currentValue = _targetValue;

        _label.text = _currentValue.ToString("0.00");
    }

    private void LateUpdate()
    {
        if (_accumulatedChange != 0)
        {
            CreateIndicator(_accumulatedChange);
            _accumulatedChange = 0;
        }
    }

    private void AccumulateChange(float newValue)
    {
        if (newValue == 0)
        {
            _label.DOComplete();
            _label.rectTransform.anchoredPosition = _basePos;
            _label.color = Color.red;
            _label.transform.DOShakePosition(2, 10, 100);
            _label.transform.DOScale(1.5f, 0.2f);
        }
        else
        {
            _label.DOComplete();
            _label.rectTransform.anchoredPosition = _basePos;
            _label.color = _baseColor;
            _label.transform.localScale = Vector3.one;
        }
        _accumulatedChange += (newValue - _targetValue);
        _targetValue = newValue;
    }

    private void CreateIndicator(float amount)
    {
        GameObject indicatorPrefab = amount > 0 ? GainMoney : LoseMoney;
        GameObject indicator = Instantiate(indicatorPrefab, _label.transform.position, Quaternion.identity, _label.transform.parent);
        RectTransform rectTransform = indicator.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        TMP_Text indicatorText = indicator.GetComponent<TMP_Text>();

        if (indicatorText == null)
        {
            Debug.LogError("TMP_Text component missing from the indicator prefab.");
            return;
        }

        indicatorText.text = (amount > 0 ? "+" : "") + amount.ToString("0.00");
        StartCoroutine(FadeAndMoveIndicator(indicator));
    }

    private IEnumerator FadeAndMoveIndicator(GameObject indicator)
    {
        TMP_Text text = indicator.GetComponent<TMP_Text>();
        Color initialColor = text.color;
        Vector3 initialPosition = indicator.transform.position;
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            text.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1 - t);
            indicator.transform.position = initialPosition + new Vector3(0, t, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(indicator);
    }
}