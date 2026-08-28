using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    [Header("Audio Manager")]
    public GameMusicManager musicManager; // Arrastra el objeto con GameMusicManager desde el Inspector

    [Header("Interfaz UI (Corazones y Game Over)")]
    public Image[] corazones;            
    public Sprite corazonLleno;          
    public Sprite corazonVacio;          
    public GameObject canvasGameOver;    

    [Header("Ajustes de Salud")]
    public int saludMaxima = 3;
    private int saludActual;

    [Header("Inmunidad al Recibir Daño")]
    public float tiempoInmunidad = 1.5f;
    public float velocidadParpadeo = 0.1f;

    [Header("Retroceso (Knockback)")]
    public float fuerzaKnockbackX = 6f;
    public float fuerzaKnockbackY = 4f;

    private bool esInvulnerable = false;
    private bool estaMuerto = false;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        saludActual = saludMaxima;

        // Búsqueda automática del GameMusicManager si no se asignó manualmente
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

        // 1. Restar vida
        saludActual -= cantidadDanio;
        if (saludActual < 0) saludActual = 0;

        ActualizarUI();

        // 2. Comprobar si murió
        if (saludActual <= 0)
        {
            Morir();
            return;
        }

        // 3. Aplicar empujón de golpe (Knockback)
        if (rb != null && posicionOrigen != Vector2.zero)
        {
            float dirX = transform.position.x < posicionOrigen.x ? -1f : 1f;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(dirX * fuerzaKnockbackX, fuerzaKnockbackY), ForceMode2D.Impulse);
        }

        // 4. Parpadeo de inmunidad
        StartCoroutine(RutinaInmunidad());
    }

    private void Morir()
    {
        estaMuerto = true;

        // Detener movimiento
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Desactivar controles
        ControladorDelJugador scriptMov = GetComponent<ControladorDelJugador>();
        if (scriptMov != null) scriptMov.enabled = false;

        // Detener música de fondo y reproducir el sonido de Game Over
        if (musicManager != null)
        {
            musicManager.PlayLoseSound();
        }

        // Mostrar pantalla de Game Over
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
        esInvulnerable = true;
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

    // --- COLISIONES CON ENEMIGOS, PROYECTILES, VACÍO O FIN DE NIVEL ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cambiar a la escena 6 al tocar el piso con la etiqueta "End"
        if (collision.gameObject.CompareTag("End"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(6);
            return;
        }

        // Detecta si el objeto se llama "Vacio" o tiene la etiqueta "Vacio"
        if (collision.gameObject.CompareTag("Vacio") || collision.gameObject.name.Contains("Vacio"))
        {
            RecibirDanio(saludActual, collision.transform.position, true);
        }
        else if (collision.gameObject.CompareTag("Enemigo"))
        {
            RecibirDanio(1, collision.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cambiar a la escena 6 al entrar a la zona con la etiqueta "End"
        if (collision.CompareTag("End"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(6);
            return;
        }

        // Detecta si la zona Trigger se llama "Vacio" o tiene la etiqueta "Vacio"
        if (collision.CompareTag("Vacio") || collision.gameObject.name.Contains("Vacio"))
        {
            RecibirDanio(saludActual, collision.transform.position, true);
        }
        else if (collision.CompareTag("Enemigo"))
        {
            RecibirDanio(1, collision.transform.position);
        }
    }
}