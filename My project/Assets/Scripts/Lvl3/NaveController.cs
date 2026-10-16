using UnityEngine;

namespace Nivel3
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class NaveController : MonoBehaviour
    {
        [Header("Movimiento")]
        [SerializeField] private float velocidadDerecha = 5f;
        [SerializeField] private float fuerzaSalto = 7f;

        [Header("Configuración de Vida / Daño")]
        [SerializeField] private int dañoPico = 20;

        private Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            // Salto tipo Flappy Bird al presionar Espacio
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Salto();
            }
        }

        void FixedUpdate()
        {
            // Mantiene el avance constante a la derecha reteniendo la velocidad vertical del Rigidbody
            rb.linearVelocity = new Vector2(velocidadDerecha, rb.linearVelocity.y);
        }

        private void Salto()
        {
            // Reiniciamos la velocidad en Y antes de saltar para que el impulso sea siempre reactivo
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // Colisiones Físicas
        private void OnCollisionEnter2D(Collision2D collision)
        {
            ProcesarImpacto(collision.gameObject);
        }

        // Colisiones con Triggers
        private void OnTriggerEnter2D(Collider2D collision)
        {
            ProcesarImpacto(collision.gameObject);
        }

        private void ProcesarImpacto(GameObject objetoColisionado)
        {
            if (objetoColisionado.CompareTag("Pico"))
            {
                RecibirDano(dañoPico);
            }
            else if (objetoColisionado.CompareTag("Piso"))
            {
                MorirInstantaneamente();
            }
        }

        public void RecibirDano(int cantidad)
        {
            Debug.Log($"[Nave] ¡Toco un Pico! Recibe {cantidad} de daño.");
            
            // TODO: Integración con tu script de vida
            // Ejemplo: GetComponent<SistemaVida>().RestarVida(cantidad);
        }

        public void MorirInstantaneamente()
        {
            Debug.Log("[Nave] ¡Toco el Piso! Muerte instantánea.");

            // TODO: Integración con tu script de vida
            // Ejemplo: GetComponent<SistemaVida>().MuerteDirecta();
        }
    }
}