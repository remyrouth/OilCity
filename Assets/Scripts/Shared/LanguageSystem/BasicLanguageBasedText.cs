using UnityEngine;
using TMPro;

public class BasicLanguageBasedText : MonoBehaviour, ILanguageChangeable
{
    [SerializeField] private LanguageItem LanguageText;
    private TMP_Text Text;

    public void UpdateText()
    {
        Text.text = LanguageText.ToString();
    }

    private void OnEnable() => UpdateText();

}
