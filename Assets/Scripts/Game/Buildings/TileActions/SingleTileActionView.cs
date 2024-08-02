using DG.Tweening;
using UnityEngine;
public abstract class SingleTileActionView<T> : MonoBehaviour
    where T : TileAction
{
    [SerializeField] private Transform _rendererPivot;
    public virtual void Initialize(T action, float rotation, TileObjectController toc, TileSelector ts, bool upsideDown)
    {
        transform.localScale = Vector3.zero;
        transform.localEulerAngles = Vector3.forward * -90;
        transform.DOScale(Vector3.one * 0.02f, 0.1f);
        transform.DOLocalRotate(Vector3.forward * rotation, 0.25f);
        _rendererPivot.transform.DOLocalRotate(-Vector3.forward * rotation, 0.25f);
    }
    private void OnDestroy()
    {
        transform.DOKill();
    }
}
