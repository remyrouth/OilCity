using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public virtual void SetState(bool IsPossible)
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, IsPossible ? 1 : 0.75f);
    }
}
