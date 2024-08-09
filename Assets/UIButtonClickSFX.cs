using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonClickSFX : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { 
            SoundManager.Instance.SelectButtonSFXTrigger();
        }); 
    }

}
