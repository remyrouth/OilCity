using Unity.VisualScripting;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [System.Serializable]
    private struct ParallaxLayer
    {
        [field: SerializeField] public RectTransform rect { get; private set; }
        [field: SerializeField] public float strength { get; private set; }
    }
    [SerializeField] private ParallaxLayer[] layers;

    private void LateUpdate()
    {
        float mouseX;
        if (overrideMouseX != null)
            mouseX = overrideMouseX!.Value;
        else
            mouseX = Mathf.Clamp01(ControlManager.Instance.RetrieveMousePosition().x / Screen.width) * 2 - 1;

        foreach (var layer in layers)
        {
            Vector2 targetPos = new Vector2(mouseX * layer.strength, layer.rect.anchoredPosition.y);
            layer.rect.anchoredPosition = Vector2.Lerp(layer.rect.anchoredPosition, targetPos, Time.deltaTime * 5);
        }
    }
    public float? overrideMouseX;
}
