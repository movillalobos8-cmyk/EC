using System.Collections;
using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    // CUÁNTO SE MUEVE HACIA LA IZQUIERDA
    public float distanciaMovimiento = 3f;

    // VELOCIDAD DEL MOVIMIENTO
    public float velocidad = 2f;

    // TIEMPO QUE SE QUEDA QUIETA
    public float tiempoQuieta = 2f;

    // TIEMPO ENTRE MOVIMIENTOS
    public float cooldown = 5f;

    [Header("Initial Delay")]
    public float initialDelay = 1f;

    private Vector3 posicionInicial;
    private Vector3 posicionObjetivo;

    private bool moviendose = false;

    void Start()
    {
        posicionInicial = transform.position;

        // OBJETIVO HACIA LA IZQUIERDA
        posicionObjetivo =
            posicionInicial + Vector3.left * distanciaMovimiento;
        // Evitar iniciar el movimiento inmediatamente; habilitar después del delay
        moviendose = true;
        Invoke(nameof(EnableMovement), initialDelay);
    }

    void EnableMovement()
    {
        moviendose = false;
    }

    void Update()
    {
        if (!moviendose)
        {
            StartCoroutine(MoverCabeza());
        }
    }

    IEnumerator MoverCabeza()
    {
        moviendose = true;

        // MOVER HACIA LA IZQUIERDA
        while (Vector3.Distance(transform.position, posicionObjetivo) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                posicionObjetivo,
                velocidad * Time.deltaTime
            );

            yield return null;
        }

        // ESPERAR QUIETA
        yield return new WaitForSeconds(tiempoQuieta);

        // VOLVER A LA POSICIÓN ORIGINAL
        while (Vector3.Distance(transform.position, posicionInicial) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                posicionInicial,
                velocidad * Time.deltaTime
            );

            yield return null;
        }

        // ESPERAR ANTES DE REPETIR
        yield return new WaitForSeconds(cooldown);

        moviendose = false;
    }
}