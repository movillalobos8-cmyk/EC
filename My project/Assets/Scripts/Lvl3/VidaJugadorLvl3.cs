using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VidaJugadorLvl3 : MonoBehaviour
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
    public int saludActual;

    [Header("Inmunidad al Recibir Daño")]
    public float tiempoInmunidad = 1.5f;
    public float velocidadParpadeo = 0.1f;

    [Header("Sensibilidad de Colisión (Píxeles de Pantalla)")]
    [Tooltip("Distancia en píxeles reales de la pantalla para colisionar")]
    public float radioChoqueUI = 60f;

    private bool esInvulnerable = false;
    private bool estaMuerto = false;

    private Image uiImage;
    private RectTransform rectNave;
    private Camera camaraPrincipal;

    public bool EsInvulnerable() => esInvulnerable;
    public bool EstaMuerto() => estaMuerto;

    private void Start()
    {
        uiImage = GetComponent<Image>();
        rectNave = GetComponent<RectTransform>();
        camaraPrincipal = Camera.main;

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

    private void Update()
    {
        if (estaMuerto || esInvulnerable || rectNave == null) return;

        GameObject[] proyectiles = GameObject.FindGameObjectsWithTag("Enemigo");

        // Transforma la posición de la nave a píxeles de pantalla
        Vector2 posicionNavePantalla = RectTransformUtility.WorldToScreenPoint(camaraPrincipal, rectNave.position);

        foreach (GameObject proyectil in proyectiles)
        {
            if (proyectil == null) continue;

            // Transforma la posición del proyectil en el mundo a píxeles de pantalla
            Vector2 posicionProyectilPantalla = camaraPrincipal != null 
                ? (Vector2)camaraPrincipal.WorldToScreenPoint(proyectil.transform.position)
                : (Vector2)proyectil.transform.position;

            float distancia = Vector2.Distance(posicionNavePantalla, posicionProyectilPantalla);

            if (distancia <= radioChoqueUI)
            {
                RecibirDanio(1);
                Destroy(proyectil);
                break;
            }
        }
    }

    public void RecibirDanio(int cantidadDanio, bool ignorarInmunidad = false)
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

        StartCoroutine(RutinaInmunidad());
    }

    private void Morir()
    {
        estaMuerto = true;

        ControlNaveLvl3 scriptMov = GetComponent<ControlNaveLvl3>();
        if (scriptMov != null) scriptMov.enabled = false;

        if (musicManager != null)
        {
            musicManager.PlayLoseSound();
        }

        if (canvasGameOver != null)
        {
            canvasGameOver.transform.SetAsLastSibling();
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
            if (uiImage != null) uiImage.enabled = !uiImage.enabled;

            yield return new WaitForSeconds(velocidadParpadeo);
            tiempo += velocidadParpadeo;
        }

        if (uiImage != null) uiImage.enabled = true;

        esInvulnerable = false;
    }
}