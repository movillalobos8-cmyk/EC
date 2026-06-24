using UnityEngine;

public class SnakeAttackSpawner : MonoBehaviour
{
    public GameObject proyectilPrefab;

    public Transform puntoDisparo;

    [Header("Cantidad visible")]
    public int cantidadProyectiles = 10;

    [Header("Tiempo entre proyectiles")]
    public float intervalo = 0.15f;

    [Header("Initial Delay")]
    public float initialDelay = 1f;

    [Header("Boca")]
    public Sprite bocaCerrada;
    public Sprite bocaAbierta;
    public float tiempoBocaAbierta = 0.1f;

    private int proyectilesActivos;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        InvokeRepeating(
            nameof(Disparar),
            initialDelay,
            intervalo
        );
    }

    void Disparar()
    {
        if (proyectilesActivos >= cantidadProyectiles)
            return;

        sr.sprite = bocaAbierta;

        CancelInvoke(nameof(CerrarBoca));
        Invoke(nameof(CerrarBoca), tiempoBocaAbierta);

        GameObject p = Instantiate(
            proyectilPrefab,
            puntoDisparo.position,
            Quaternion.identity
        );

        SnakeProjectile snake =
            p.GetComponent<SnakeProjectile>();

        snake.spawner = this;

        proyectilesActivos++;
    }

    void CerrarBoca()
    {
        sr.sprite = bocaCerrada;
    }

    public void RemoverProyectil()
    {
        proyectilesActivos--;
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}