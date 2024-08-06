using UnityEngine;
using UnityEditor;
using Game.Managers;
// This script was made before "Poland In Pixels" project
// Original author: Piotr Trzeœniowski
[CreateAssetMenu]
public class LanguageItem : ScriptableObject
{
    [SerializeField] [TextArea] private string[] text;

    public override string ToString()
    {
        int langID = (int)SettingsManager.Instance.CurrentLanguage;
        if (langID < text.Length && langID >= 0)
            return text[langID];
        Debug.LogError("Couldn't find correct translated text!");
        return text[0];
    }
}
public enum Language
{
    English,
    Polish
}

#if UNITY_EDITOR
[CustomEditor(typeof(LanguageItem))]
class LanguageEditor : Editor
{
    private SerializedProperty textProperty;

    void OnEnable()
    {
        textProperty = serializedObject.FindProperty("text");
    }
    public override void OnInspectorGUI()
    {
        int enumSize = System.Enum.GetValues(typeof(Language)).Length;

        int size = serializedObject.FindProperty("text.Array.size").intValue;

        if (enumSize != size)
            serializedObject.FindProperty("text.Array.size").intValue = enumSize;
        for (int i = 0; i < enumSize; i++)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(((Language)i).ToString(), GUILayout.Width(64));
            EditorGUILayout.PropertyField(textProperty.GetArrayElementAtIndex(i), GUIContent.none);
            GUILayout.EndHorizontal();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif