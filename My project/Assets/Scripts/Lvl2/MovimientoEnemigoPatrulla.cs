using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovimientoEnemigoPatrulla : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidad = 3f;
    public bool mirandoDerecha = true;

    [Header("Detector de Precipicio")]
    public Transform detectorPiso;
    public float distanciaPiso = 0.6f;
    public LayerMask capaObstaculos;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (!mirandoDerecha && transform.localScale.x > 0)
        {
            Vector3 escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;
        }
    }

    private void FixedUpdate()
    {
        // 1. Mover constantemente
        float direccion = mirandoDerecha ? 1f : -1f;
        rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);

        // 2. Verificar precipicio / objetos sin tag Piso debajo
        RaycastHit2D impactoPiso = Physics2D.Raycast(detectorPiso.position, Vector2.down, distanciaPiso, capaObstaculos);
        
        bool pisoValido = impactoPiso.collider != null && impactoPiso.collider.CompareTag("Piso");

        // Si se acaba el piso o toca algo sin tag "Piso" abajo, da la vuelta
        if (!pisoValido)
        {
            Girar();
        }
    }

    // 3. GIRAR AL CHOCAR CONTRA UNA PARED O BLOQUE CON TAG "PISO"
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Piso"))
        {
            // Verificamos si el choque fue frontal (horizontal) y no por pisarlo
            foreach (ContactPoint2D punto in collision.contacts)
            {
                // Si la normal en X es diferente de 0, significa que chocó de lado contra una pared
                if (Mathf.Abs(punto.normal.x) > 0.5f)
                {
                    Girar();
                    break;
                }
            }
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;

        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnDrawGizmosSelected()
    {
        if (detectorPiso != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(detectorPiso.position, Vector3.down * distanciaPiso);
        }
    }
}