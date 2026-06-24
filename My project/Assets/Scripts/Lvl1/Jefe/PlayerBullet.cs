using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 15f;

    [Header("Daño")]
    public int damage = 1;

    [Header("Tiempo de vida")]
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position +=
            Vector3.right *
            speed *
            Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HeadHealth head =
            other.GetComponentInParent<HeadHealth>();

        if (head != null)
        {
            head.TakeDamage(damage);

            Destroy(gameObject);

            return;
        }
    }
}