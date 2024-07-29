using UnityEngine;

public class TreeMat_listener : MonoBehaviour
{
    [SerializeField] private Material _treeMaterial;
    private void Awake()
    {
        PollutionManager.Instance.OnPollutionChanged += UpdateValue;
    }
    private void UpdateValue(float newValue)
    {
        _treeMaterial.SetFloat("_WitherAmount", newValue);
    }

}
