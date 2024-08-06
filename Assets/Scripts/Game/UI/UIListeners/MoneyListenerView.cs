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
        MoneyManager.Instance.OnMoneyChanged += AccumulateChange; // Subscribe to the money changed event
        _currentValue = MoneyManager.Instance.Money;
    }

    private void Start()
    {
        _label.text = _currentValue.ToString("0.00");
    }

    private void OnDestroy()
    {
        MoneyManager.Instance.OnMoneyChanged -= AccumulateChange; // Unsubscribe from the event to avoid memory leaks
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
            CreateIndicator(_accumulatedChange); // Create an indicator for the accumulated change
            _accumulatedChange = 0; // Reset the accumulated change after creating the indicator
        }
    }

    private void AccumulateChange(float newWSvalue)
    {
        _accumulatedChange += (newWSvalue - _targetValue); // Accumulate the change
        _targetValue = newWSvalue; // Update the target value
    }

    private void CreateIndicator(float amount)
    {
        GameObject indicatorPrefab = amount > 0 ? GainMoney : LoseMoney;
        GameObject indicator = Instantiate(indicatorPrefab, _label.transform.position, Quaternion.identity, _label.transform.parent);
        RectTransform rectTransform = indicator.GetComponent<RectTransform>();

        float newYPosition = -activeIndicators.Count * verticalSpacing;
        rectTransform.anchoredPosition = new Vector2(0, newYPosition); // Set anchored position to zero and apply newYPosition
        TMP_Text indicatorText = indicator.GetComponent<TMP_Text>();

        if (indicatorText == null)
        {
            Debug.LogError("TMP_Text component missing from the indicator prefab.");
            return;
        }

        indicatorText.text = (amount > 0 ? "+" : "") + amount.ToString("0.00");
        activeIndicators.Add(indicator); // Add to the list of active indicators
        StartCoroutine(FadeAndMoveIndicator(indicator)); // Start the fade and move animation
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
            text.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1 - t); // Fade out the text color
            indicator.transform.position = initialPosition + new Vector3(0, t, 0); // Move the indicator upwards
            elapsed += Time.deltaTime;
            yield return null;
        }

        activeIndicators.Remove(indicator); // Remove from the list of active indicators
        Destroy(indicator); // Destroy the indicator after the animation
    }

    public void CreatePlayerIndicator(float amount)
    {
        GameObject indicatorPrefab = amount > 0 ? GainMoney : LoseMoney;
        GameObject indicator = Instantiate(indicatorPrefab, _label.transform.position, Quaternion.identity, _label.transform.parent);
        RectTransform rectTransform = indicator.GetComponent<RectTransform>();

        float newYPosition = -activeIndicators.Count * verticalSpacing - verticalSpacing;
        rectTransform.anchoredPosition = new Vector2(0, newYPosition); // Correct anchored position

        TMP_Text indicatorText = indicator.GetComponent<TMP_Text>();

        if (indicatorText == null)
        {
            Debug.LogError("TMP_Text component missing from the indicator prefab.");
            return;
        }

        indicatorText.text = (amount > 0 ? "+" : "") + amount.ToString("0.00");
        activeIndicators.Add(indicator); // Add to the list of active indicators
        StartCoroutine(FadeAndMoveIndicator(indicator)); // Start the fade and move animation
    }
}