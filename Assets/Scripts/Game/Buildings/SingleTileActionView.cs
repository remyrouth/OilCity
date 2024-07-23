using UnityEngine;
using UnityEngine.UI;

public class SingleTileActionView : MonoBehaviour
{
    [SerializeField] private Image _renderer;
    [SerializeField] private Button _button;
    public void Initialize(TileAction action, float rotation,TileObjectController toc,TileSelector ts)
    {
        transform.localEulerAngles = Vector3.forward * rotation;
        _renderer.transform.localEulerAngles = -Vector3.forward * rotation;
        _renderer.sprite = action.icon;
        _button.onClick.AddListener(()=> { action.OnClicked(toc);ts.EndFocus(); });
    }



}
