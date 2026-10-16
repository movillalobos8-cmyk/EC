using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Nivel3
{
    public class VidaJugadorNivel3 : MonoBehaviour
    {
        [Header("Audio Manager")]
        public GameMusicManager musicManager;

        [Header("Interfaz UI (Corazones y Game Over)")]
        public Image[] corazones;            
        public Sprite corazonLleno;          
        public Sprite corazonVacio;          
        public GameObject canvasGameOver;    

        [Header("Cambio de Escena / Final del Nivel")]
        public int indiceEscenaFinal = 6;    

        [Header("Ajustes de Salud")]
        public int saludMaxima = 3;
        private int saludActual;

        [Header("Inmunidad al Recibir Daño")]
        public float tiempoInmunidad = 1.5f;
        public float velocidadParpadeo = 0.1f;

        [Header("Retroceso (Knockback)")]
        public float fuerzaKnockbackX = 4f;
        public float fuerzaKnockbackY = 3f;

        private bool esInvulnerable = false;
        private bool estaMuerto = false;

        private SpriteRenderer sr;
        private Rigidbody2D rb;

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();

            saludActual = saludMaxima;

            if (musicManager == null)
            {
                musicManager = FindObjectOfType<GameMusicManager>();
            }

            if (canvasGameOver != null)
            {
                canvasGameOver.SetActive(false);
            }

            ActualizarUI();
        }

        public void RecibirDanio(int cantidadDanio, Vector2 posicionOrigen, bool ignorarInmunidad = false)
        {
            if ((esInvulnerable && !ignorarInmunidad) || estaMuerto) return;

            if (!ignorarInmunidad)
            {
                esInvulnerable = true;
            }

            saludActual -= cantidadDanio;
            if (saludActual < 0) saludActual = 0;

            ActualizarUI();

            if (saludActual <= 0)
            {
                Morir();
                return;
            }

            // Impulso de retroceso / Knockback opcional
            if (rb != null && posicionOrigen != Vector2.zero)
            {
                float dirX = transform.position.x < posicionOrigen.x ? -1f : 1f;
                SetRigidbodyVelocity(Vector2.zero);
                rb.AddForce(new Vector2(dirX * fuerzaKnockbackX, fuerzaKnockbackY), ForceMode2D.Impulse);
            }

            StartCoroutine(RutinaInmunidad());
        }

        public void MuerteInstantanea()
        {
            if (estaMuerto) return;
            RecibirDanio(saludActual, Vector2.zero, true);
        }

        private void Morir()
        {
            estaMuerto = true;

            if (rb != null) SetRigidbodyVelocity(Vector2.zero);

            // Desactiva el control de la nave al morir
            NaveController scriptMov = GetComponent<NaveController>();
            if (scriptMov != null) scriptMov.enabled = false;

            if (musicManager != null)
            {
                musicManager.PlayLoseSound();
            }

            if (canvasGameOver != null)
            {
                canvasGameOver.SetActive(true);
            }
        }

        private void ActualizarUI()
        {
            if (corazones == null) return;

            for (int i = 0; i < corazones.Length; i++)
            {
                if (corazones[i] == null) continue;

                if (i < saludActual)
                {
                    corazones[i].sprite = corazonLleno;
                }
                else
                {
                    corazones[i].sprite = corazonVacio;
                }
            }
        }

        private IEnumerator RutinaInmunidad()
        {
            float tiempo = 0f;

            while (tiempo < tiempoInmunidad)
            {
                if (sr != null) sr.enabled = !sr.enabled;
                yield return new WaitForSeconds(velocidadParpadeo);
                tiempo += velocidadParpadeo;
            }

            if (sr != null) sr.enabled = true;
            esInvulnerable = false;
        }

        // --- DETECCIÓN DE COLISIONES adaptada para el Nivel 3 ---

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ProcesarColision(collision.gameObject, collision.transform.position);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ProcesarColision(collision.gameObject, collision.transform.position);
        }

        private void ProcesarColision(GameObject objeto, Vector3 posicion)
        {
            // Transición de nivel al final
            if (objeto.CompareTag("End"))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(indiceEscenaFinal);
                return;
            }

            // Tocar el Piso quita toda la vida de inmediato
            if (objeto.CompareTag("Piso") || objeto.CompareTag("Vacio") || objeto.name.Contains("Vacio"))
            {
                MuerteInstantanea();
            }
            // Tocar Picos u otros Obstáculos / Enemigos quita 1 de vida
            else if (objeto.CompareTag("Pico") || objeto.CompareTag("Enemigo") || objeto.CompareTag("Boss"))
            {
                RecibirDanio(1, posicion);
            }
        }

        // Método auxiliar para mantener compatibilidad de velocidad según la versión de Unity
        private void SetRigidbodyVelocity(Vector2 nuevaVelocidad)
        {
            #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = nuevaVelocidad;
            #else
            rb.velocity = nuevaVelocidad;
            #endif
        }
    }
}