using TMPro;
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
    private void UpdateLabel(int newWSvalue)
    {
        _image.fillAmount = (float)newWSvalue / 100;
    }
}
