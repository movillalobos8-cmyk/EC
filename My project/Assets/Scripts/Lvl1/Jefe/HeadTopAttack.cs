using UnityEngine;

public class HeadTopAttack : MonoBehaviour
{
    public GameObject proyectilPrefab;

    public Transform puntoDisparo;

    public Transform player;

    public float cooldown = 3f;

    [Header("Initial Delay")]
    public float initialDelay = 1f;

    // SPRITES
    public Sprite bocaCerrada;
    public Sprite bocaAbierta;

    // TIEMPO QUE LA BOCA QUEDA ABIERTA
    public float tiempoBocaAbierta = 0.2f;

    private float timer;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Invoke(nameof(StartFirstAttack), initialDelay);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            Atacar();
            timer = 0f;
        }
    }

    void Atacar()
    {
        sr.sprite = bocaAbierta;

        Invoke(nameof(CerrarBoca), tiempoBocaAbierta);

        GameObject proyectil = Instantiate(
            proyectilPrefab,
            puntoDisparo.position,
            Quaternion.identity
        );

        Vector2 direccion =
            player.position - puntoDisparo.position;

        BloodProjectile blood =
            proyectil.GetComponent<BloodProjectile>();

        blood.SetDirection(direccion);

        Collider2D proyectilCollider =
            proyectil.GetComponent<Collider2D>();

        Collider2D[] bossColliders =
            transform.root.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D bossCollider in bossColliders)
        {
            Physics2D.IgnoreCollision(
                proyectilCollider,
                bossCollider
            );
        }
    }

    void StartFirstAttack()
    {
        Atacar();
        timer = 0f;
    }

    void CerrarBoca()
    {
        sr.sprite = bocaCerrada;
    }
}