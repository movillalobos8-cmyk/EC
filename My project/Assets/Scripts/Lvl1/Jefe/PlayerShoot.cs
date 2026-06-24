using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bulletPrefab;

    public Transform firePoint;

    public float cooldown = 0.2f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            if (timer >= cooldown)
            {
                Shoot();
                timer = 0f;
            }
        }
    }

    void Shoot()
    {
        Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );
    }
}