using UnityEngine;
using UnityEngine.UI;

public class WSListenerView : MonoBehaviour
{
    [SerializeField] private Image _image;
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
    }
    private void UpdateLabel(int newWSvalue)
    {
        _desiredValue = (float)newWSvalue / 100;
    }
}
