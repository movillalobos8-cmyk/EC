using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    public float speed = 5f;

    [HideInInspector]
    public bool tutorialObstacle = false;

    void Update()
    {
        transform.Translate(
            Vector2.left * speed * Time.deltaTime
        );

        // Los obstáculos normales se destruyen
        if (!tutorialObstacle)
        {
            if (transform.position.x <
                Camera.main.transform.position.x - 30f)
            {
                Destroy(gameObject);
            }
        }
    }
}