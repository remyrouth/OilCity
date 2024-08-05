using UnityEngine;

public class BuildingsOutline : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer outline;
    protected virtual void Start() => Disable();
    public void Enable() => outline.enabled = true;
    public void Disable() => outline.enabled = false;

}
