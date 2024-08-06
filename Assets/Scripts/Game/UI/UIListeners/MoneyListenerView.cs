using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoneyListenerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] private GameObject GainMoney;
    [SerializeField] private GameObject LoseMoney;
    [SerializeField] private float verticalSpacing = 30f;

    private float _currentValue = 0;
    private float _targetValue = 0;
    private float _accumulatedChange = 0;
    private List<GameObject> activeIndicators = new List<GameObject>();

    private void Awake()
    {
        MoneyManager.Instance.OnMoneyChanged += AccumulateChange;
        _currentValue = MoneyManager.Instance.Money;
    }

    private void Start()
    {
        _label.text = _currentValue.ToString("0.00");
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

        _label.text = _currentValue.ToString("0.00");
    }

    private void UpdateLabel(float newWSvalue)
    {
        if (_currentValue != newWSvalue)
        {
            CreateIndicator(newWSvalue - _currentValue);
        }
        _targetValue = newWSvalue;
    }

    private void CreateIndicator(float amount)
    {
        GameObject indicatorPrefab = amount > 0 ? GainMoney : LoseMoney;
        GameObject indicator = Instantiate(indicatorPrefab, _label.transform.position, Quaternion.identity, _label.transform.parent);
        RectTransform rectTransform = indicator.GetComponent<RectTransform>();

        float newYPosition = -activeIndicators.Count * verticalSpacing;
        rectTransform.anchoredPosition = Vector2.zero;
        TMP_Text indicatorText = indicator.GetComponent<TMP_Text>();

        if (indicatorText == null)
        {
            Debug.LogError("TMP_Text component missing from the indicator prefab.");
            return;
        }

        indicatorText.text = (amount > 0 ? "+" : "") + amount.ToString("0.00");
        activeIndicators.add(indicator);
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

        activeIndicators.Remove(indicator);
        Destroy(indicator);
    }

    public void CreatePlayerIndicator(float amount)
    {
        GameObject indicatorPrefab = amount > 0 ? GainMoney : LoseMoney;
        GameObject indicator = Instantiate(indicatorPrefab, _label.transform.position, Quaternion.identity, _label.transform.parent);
        RectTransform rectTransform = indicator.GetComponent<RectTransform>();

        float newYPosition = -activeIndicators.Count * verticalSpacing - verticalSpacing;
        rectTransform.anchoredPosition = new Vector2(_, newYPosition);
        // potential issue above for later

        TMP_text indicatorText = indicator.GetComponent<TMP_Text>();

        if (indicatorText == null)
        {
            Debug.LogError("TMP_Text component missing from the indicator prefab.");
            retrun;
        }

        indicator.text = (amount > 0 ? "+" : "") + amount.ToString("0.00");
        activeIndicators.Add(indicator);
        StartCoroutine(FadeAndMoveIndicator(indicator));
    }
}