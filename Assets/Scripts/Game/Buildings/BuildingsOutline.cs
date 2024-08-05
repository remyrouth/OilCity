using UnityEngine;

public class BuildingsOutline : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outline;
    private void Start() => Disable();
    public void Enable() => outline.enabled = true;
    public void Disable() => outline.enabled = false;

}
