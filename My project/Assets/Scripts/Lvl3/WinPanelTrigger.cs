using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class WinSequence : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas;
    public GameObject winPanelCanvas;

    [Header("Música")]
    [Tooltip("Arrastra aquí el objeto con el script GameMusicManager. Si lo dejas vacío, lo busca solo.")]
    public GameMusicManager musicManager;

    [Header("Cambio de escena al terminar el video")]
    public int numeroDeEscena = 0;

    [Header("Detección")]
    public string tagFinal = "Final";
    public float margenContacto = 0.05f;
    public float tiempoDeGracia = 0.3f;

    private bool iniciada = false;
    private bool videoIniciado = false;
    private float tiempoInicio;
    private float proximaBusqueda = 0f;

    private Collider2D[] collidersPlayer;
    private readonly List<Collider2D> collidersFinal = new List<Collider2D>();

    private void Start()
    {
        Time.timeScale = 1f;
        tiempoInicio = Time.unscaledTime;

        if (videoCanvas) videoCanvas.SetActive(false);
        if (winPanelCanvas) winPanelCanvas.SetActive(false);

        collidersPlayer = GetComponentsInChildren<Collider2D>();
        if (collidersPlayer.Length == 0)
            Debug.LogError("[WinSequence] ERROR: este objeto (" + name + ") NO tiene Collider2D. El script debe ir en el Player.");

        BuscarObjetosFinal();

        if (videoPlayer)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            videoPlayer.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
            videoPlayer.loopPointReached += vp => CambiarDeEscena();
            videoPlayer.errorReceived += (vp, m) =>
            {
                Debug.LogError("[WinSequence] Error de video: " + m);
                CambiarDeEscena();
            };
            videoPlayer.Prepare();
        }
    }

    private void BuscarObjetosFinal()
    {
        collidersFinal.Clear();
        try
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tagFinal);
            int validos = 0;

            foreach (GameObject o in objs)
            {
                // Si el objeto Final es el Player mismo o un padre del Player, se ignora
                if (o == gameObject || transform.IsChildOf(o.transform))
                {
                    Debug.LogWarning("[WinSequence] '" + o.name + "' tiene el tag '" + tagFinal + "' pero contiene al Player (o es el Player). Quítale el tag.");
                    continue;
                }

                validos++;
                foreach (Collider2D c in o.GetComponentsInChildren<Collider2D>())
                {
                    if (System.Array.IndexOf(collidersPlayer, c) < 0)
                        collidersFinal.Add(c);
                }
            }

            if (validos == 0)
                Debug.LogWarning("[WinSequence] No hay ningún objeto válido con el tag '" + tagFinal + "'.");
            else if (collidersFinal.Count == 0)
                Debug.LogError("[WinSequence] El objeto con tag '" + tagFinal + "' NO tiene Collider2D (¿tiene un Collider 3D por error?).");
            else
                Debug.Log("[WinSequence] Objetos Final encontrados: " + validos + " | Colliders2D: " + collidersFinal.Count);
        }
        catch (UnityException)
        {
            Debug.LogError("[WinSequence] El tag '" + tagFinal + "' NO EXISTE. Créalo en Tags & Layers y asígnalo a la pared.");
        }
    }

    private void Update()
    {
        if (iniciada) return;
        if (Time.unscaledTime - tiempoInicio < tiempoDeGracia) return;

        // Si no se encontró nada, vuelve a buscar cada segundo
        if (collidersFinal.Count == 0)
        {
            if (Time.unscaledTime >= proximaBusqueda)
            {
                proximaBusqueda = Time.unscaledTime + 1f;
                BuscarObjetosFinal();
            }
            return;
        }

        foreach (Collider2D mio in collidersPlayer)
        {
            if (mio == null || !mio.enabled) continue;

            foreach (Collider2D final in collidersFinal)
            {
                if (final == null || !final.enabled) continue;
                if (final == mio) continue; // Protección contra el error "same collider"

                ColliderDistance2D d = mio.Distance(final);
                if (d.isOverlapped || d.distance <= margenContacto)
                {
                    Debug.Log("[WinSequence] TOQUE con " + final.name + " -> secuencia iniciada");
                    IniciarSecuencia();
                    return;
                }
            }
        }
    }

    // Respaldo extra por eventos de física
    private void OnCollisionEnter2D(Collision2D c) { if (c.gameObject.CompareTag(tagFinal)) IniciarSecuencia(); }
    private void OnTriggerEnter2D(Collider2D o) { if (o.CompareTag(tagFinal)) IniciarSecuencia(); }

    private void IniciarSecuencia()
    {
        if (iniciada) return;
        if (Time.unscaledTime - tiempoInicio < tiempoDeGracia) return;
        iniciada = true;

        Time.timeScale = 0f;                                // 1. Congelar
        ReproducirMusicaWin();                              // 2. Música de win
        if (winPanelCanvas) winPanelCanvas.SetActive(true); // 3. WinPanel
    }

    private void ReproducirMusicaWin()
    {
        // Si no se asignó a mano, lo busca en la escena
        if (musicManager == null)
            musicManager = FindFirstObjectByType<GameMusicManager>();

        if (musicManager != null)
        {
            musicManager.PlayWinSound(); // Corta la música de fondo y reproduce el sonido de win
            Debug.Log("[WinSequence] Música de win reproducida.");
        }
        else
        {
            Debug.LogWarning("[WinSequence] No se encontró el GameMusicManager en la escena.");
        }
    }

    // OnClick() del botón del WinPanel
    public void ReproducirVideo()
    {
        if (videoIniciado) return;
        videoIniciado = true;

        if (winPanelCanvas) winPanelCanvas.SetActive(false); // El panel se va
        if (videoCanvas) videoCanvas.SetActive(true);        // Aparece el video

        if (videoPlayer) videoPlayer.Play();
        else CambiarDeEscena();
    }

    private void CambiarDeEscena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(numeroDeEscena);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}