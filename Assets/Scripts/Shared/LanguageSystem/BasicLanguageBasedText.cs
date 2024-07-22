using UnityEngine;
using TMPro;

public class BasicLanguageBasedText : MonoBehaviour, ILanguageChangeable
{
    [SerializeField] private LanguageItem LanguageText;
    private TMP_Text _text;

    public void UpdateText()
    {
        if (_text == null)
            _text = GetComponent<TMP_Text>();
        _text.text = LanguageText.ToString();
    }

    private void OnEnable() => UpdateText();

}
