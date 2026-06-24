using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private const int FirstLevelSceneBuildIndex = 0;
    private const int MenuSceneBuildIndex = 1;

    [Header("Referencias")]
    public Transform player;
    public GameObject alteplazaPrefab;

    [Header("Spawn")]
    public float spawnDistance = 40f;

    private GameObject currentAlteplaza;
    private float nextSpawnX;

    private bool collected = false;
    private bool levelStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!levelStarted)
            return;

        if (!collected &&
            player.position.x > nextSpawnX + spawnDistance)
        {
            RespawnAlteplaza();
        }
    }

    public void StartLevel()
    {
        if (levelStarted)
            return;

        levelStarted = true;

        SpawnAlteplaza(player.position.x + spawnDistance);
    }

    void SpawnAlteplaza(float xPos)
    {
        Vector3 pos = new Vector3(
            xPos,
            Random.Range(-3f, 3f),
            0f
        );

        currentAlteplaza =
            Instantiate(alteplazaPrefab, pos, Quaternion.identity);

        nextSpawnX = xPos;
    }

    void RespawnAlteplaza()
    {
        if (currentAlteplaza != null)
        {
            Destroy(currentAlteplaza);
        }

        SpawnAlteplaza(player.position.x + spawnDistance);
    }

    public void Recolectado()
    {
        collected = true;

        SceneManager.LoadScene(3);
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
