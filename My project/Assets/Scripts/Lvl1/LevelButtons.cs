using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtons : MonoBehaviour
{
    private const int FirstLevelSceneBuildIndex = 0;
    private const int MenuSceneBuildIndex = 1;
    private const int SecondLevelSceneBuildIndex = 4; // Índice de la escena del Nivel 2
    private const int SecondLevelRestartBuildIndex = 5; // Índice de la escena 5 para reiniciar Nivel 2

    public void RestartLvl1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(FirstLevelSceneBuildIndex);
    }

    public void RestartLvl2()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SecondLevelRestartBuildIndex);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MenuSceneBuildIndex);
    }

    public void GoToNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SecondLevelSceneBuildIndex);
    }
}