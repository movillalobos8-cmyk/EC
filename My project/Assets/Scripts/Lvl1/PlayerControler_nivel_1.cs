using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float velocidadAdelante = 3f;

    [Header("Detección Estricta por GroundCheck")]
    public Transform groundCheck;
    public LayerMask capaSuelo; // Asigna aquí la capa donde están TODOS tus pisos
    public float distanciaVerificacion = 0.08f; // Radio o largo muy corto para evitar falsas lecturas
    [Range(0f, 60f)]
    public float anguloMaximoPiso = 45f; // Descarta paredes verticales (90°)

    [Header("Estado")]
    public bool estaEnSuelo = false;

    private void Update()
    {
        VerificarContactoEstrictoConPiso();

        float movimiento = 0;

        if (Keyboard.current.wKey.isPressed)
        {
            movimiento = 1;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            movimiento = -1;
        }

        transform.Translate(
            velocidadAdelante * Time.deltaTime,
            movimiento * velocidad * Time.deltaTime,
            0
        );
    }

    private void VerificarContactoEstrictoConPiso()
    {
        if (groundCheck == null) return;

        // Evaluamos si hay algun colisionador en la posicion exacta del GroundCheck
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, distanciaVerificacion, capaSuelo);

        if (hit != null)
        {
            // Obtenemos el punto mas cercano del colisionador de piso con respecto al GroundCheck
            Vector2 puntoCercano = hit.ClosestPoint(groundCheck.position);
            
            // Calculamos la direccion desde el punto de contacto hacia el GroundCheck
            Vector2 direccionImpacto = (Vector2)groundCheck.position - puntoCercano;

            // Calculamos el ángulo con respecto a la vertical (Vector2.up)
            float anguloInclinacion = Vector2.Angle(direccionImpacto, Vector2.up);

            // Solo estará en suelo si la superficie detectada está DEBAJO del GroundCheck
            // y no es una pared lateral (cumple con el angulo máximo de inclinación)
            estaEnSuelo = anguloInclinacion <= anguloMaximoPiso;
        }
        else
        {
            estaEnSuelo = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = estaEnSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, distanciaVerificacion);
        }
    }
}