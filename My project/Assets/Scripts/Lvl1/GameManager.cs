using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const int FirstLevelSceneBuildIndex = 0;
    private const int MenuSceneBuildIndex = 1;
    private const int SecondLevelSceneBuildIndex = 5; // Índice de la escena del Nivel 2

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("Music")]
    public GameMusicManager musicManager;

    private bool ended = false;

    void Awake()
    {
        if (musicManager == null)
        {
            musicManager = FindFirstObjectByType<GameMusicManager>();
        }
    }

    public void GameOver()
    {
        if (ended) return;
        ended = true;

        if (musicManager != null)
        {
            musicManager.PlayLoseSound();
        }

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Victory()
    {
        if (ended) return;
        ended = true;

        if (musicManager != null)
        {
            musicManager.PlayWinSound();
        }

        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Restaura el tiempo antes de cambiar de escena
        SceneManager.LoadScene(FirstLevelSceneBuildIndex);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f; // Restaura el tiempo
        SceneManager.LoadScene(MenuSceneBuildIndex);
    }

    // Nuevo método para ir al Nivel 2 (Escena 5)
    public void GoToNextLevel()
    {
        Time.timeScale = 1f; // Restaura el tiempo
        SceneManager.LoadScene(SecondLevelSceneBuildIndex);
    }
}