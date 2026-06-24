using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtons : MonoBehaviour
{
    private const int FirstLevelSceneBuildIndex = 0;
    private const int MenuSceneBuildIndex = 1;

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(FirstLevelSceneBuildIndex);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MenuSceneBuildIndex);
    }
}
