using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonClickSFX : MonoBehaviour
{
    GetComponent<Button>().onClick.AddListener(() => { 
        SoundManager.Instance.SelectButtonSFXTrigger();
    });
}
