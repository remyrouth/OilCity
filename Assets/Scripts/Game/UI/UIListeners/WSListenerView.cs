using UnityEngine;
using UnityEngine.UI;

public class WSListenerView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Gradient _gradient;
    private void Awake()
    {
        WorkerSatisfactionManager.Instance.OnWorkersSatisfactionChanged += UpdateLabel;
    }
    private void OnDestroy()
    {
        WorkerSatisfactionManager.Instance.OnWorkersSatisfactionChanged -= UpdateLabel;
    }
    private float _desiredValue = 1;
    private void Update()
    {
        _image.fillAmount = Mathf.Lerp(_image.fillAmount, _desiredValue, Time.deltaTime * 20);
        _image.color = _gradient.Evaluate(_image.fillAmount);
    }
    private void UpdateLabel(float newWSvalue)
    {
        float newValue = newWSvalue / 200;
        _desiredValue = newValue;
    }
}
