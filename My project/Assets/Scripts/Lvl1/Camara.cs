using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float distanciaAdelante = 6f;

    void Update()
    {
        transform.position = new Vector3(
            player.position.x + distanciaAdelante,
            transform.position.y,
            transform.position.z
        );
    }
}