using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const int FirstLevelSceneBuildIndex = 0;
    private const int MenuSceneBuildIndex = 1;

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
        Time.timeScale = 1f;
        SceneManager.LoadScene(FirstLevelSceneBuildIndex);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MenuSceneBuildIndex);
    }
}
