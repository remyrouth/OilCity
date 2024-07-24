using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

void Start()
{
    LoadSceneAddiitively("UITestingScene");
}
public void LoadSceneAddiitively(string sceneName)
{
    SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
}
}
