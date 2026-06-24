using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;

    public float spawnRate = 1f;

    public float minY = -3f;
    public float maxY = 3f;

    public float distanceFromCamera = 20f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("No obstacle prefab assigned!");
            return;
        }

        float randomY = Random.Range(minY, maxY);

        float spawnX = Camera.main.transform.position.x + distanceFromCamera;

        Vector2 spawnPosition = new Vector2(spawnX, randomY);

        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
}