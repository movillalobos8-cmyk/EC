using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    [Header("Audio Manager")]
    public GameMusicManager musicManager;

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
        // BLINDAJE DE SEGURIDAD: Si está en inmunidad o muerto, IGNORA CUALQUIER OTRO GOLPE
        if ((esInvulnerable && !ignorarInmunidad) || estaMuerto) return;

        // Activa la invulnerabilidad DE INMEDIATO para evitar multillegadas en el mismo frame
        if (!ignorarInmunidad)
        {
            esInvulnerable = true;
        }

        // 1. Restar vida (fijado a 1 por golpe normal si no es vacío)
        saludActual -= cantidadDanio;
        if (saludActual < 0) saludActual = 0;

        ActualizarUI();

        // 2. Comprobar si murió
        if (saludActual <= 0)
        {
            Morir();
            return;
        }

        // 3. Aplicar empujón (Knockback)
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

        if (rb != null) rb.linearVelocity = Vector2.zero;

        ControladorDelJugador scriptMov = GetComponent<ControladorDelJugador>();
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
        esInvulnerable = false; // Aquí recién recupera la vulnerabilidad
    }

    // --- DETECCIÓN DE COLISIONES ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("End"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(6);
            return;
        }

        if (collision.gameObject.CompareTag("Vacio") || collision.gameObject.name.Contains("Vacio"))
        {
            RecibirDanio(saludActual, collision.transform.position, true);
        }
        else if (collision.gameObject.CompareTag("Enemigo") || collision.gameObject.CompareTag("Boss"))
        {
            RecibirDanio(1, collision.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("End"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(6);
            return;
        }

        if (collision.CompareTag("Vacio") || collision.gameObject.name.Contains("Vacio"))
        {
            RecibirDanio(saludActual, collision.transform.position, true);
        }
        else if (collision.CompareTag("Enemigo") || collision.CompareTag("Boss") || collision.CompareTag("ProyectilBoss"))
        {
            RecibirDanio(1, collision.transform.position);
        }
    }
}