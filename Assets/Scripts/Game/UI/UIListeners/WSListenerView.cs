using UnityEngine;
using UnityEngine.UI;

public class WSListenerView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private SingleSoundPlayer workerIncreaseSfX;
    [SerializeField] private SingleSoundPlayer workerDecreaseSfX;
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
    private void UpdateLabel(int newWSvalue)
    {
        float newValue = (float)newWSvalue / 100;
        if (newValue > newValue) {
            // we trigger positive increase sfx
            workerIncreaseSfX.ActivateWithForeignTrigger();
        } else if (newValue == _desiredValue) {
            // we skip
        } else {
            // we trigger negative decrease sfx
            workerDecreaseSfX.ActivateWithForeignTrigger();
        }
        _desiredValue = newValue;
    }
}
