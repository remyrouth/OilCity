using UnityEngine;
using TMPro;

public class PopupDescriptorREAL : MonoBehaviour
{
    public static PopupDescriptorREAL Instance { get; private set; }

    [SerializeField] private GameObject popupWindow;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Show(string description, Vector3 position)
    {
        descriptionText.text = description;
        popupWindow.transform.position = position;
        popupWindow.SetActive(true);
    }

    public void Hide()
    {
        popupWindow.SetActive(false);
    }
}