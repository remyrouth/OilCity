using UnityEngine;
using System.Linq;

public class SettingsManager : Singleton<SettingsManager>
{
    public Language CurrentLanguage { get; private set; } = Language.English;

    public void SetLanguage(Language language)
    {
        CurrentLanguage = language;
        var languageBasedObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .OfType<ILanguageChangeable>();
        foreach (var lbo in languageBasedObjects)
            lbo.UpdateText();
    }

}
