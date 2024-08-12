using UIScripts;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] private Difficulty difficulty;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameDiffucultyManager.difficulty = difficulty;
            UiManager.Instance.ChangeScene(1);
        });
    }
}
