using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControlTutorialManager : MonoBehaviour
{
    [Header("UI de Controles")]
    public GameObject panelControles;
    public Button estoyPreparadoButton;

    [Header("Música")]
    [Tooltip("Arrastra el GameMusicManager. Si lo dejas vacío, lo busca solo.")]
    public GameMusicManager musicManager;

    private bool controlesActivos = false;
    private bool musicaPausada = false;

    void Start()
    {
        if (musicManager == null)
            musicManager = FindFirstObjectByType<GameMusicManager>();

        // 1. Congelar el tiempo al iniciar la escena
        Time.timeScale = 0f;
        controlesActivos = true;

        // 2. Asegurar que el panel de controles sea visible
        if (panelControles != null)
        {
            panelControles.SetActive(true);
        }

        // 3. Configurar el evento del botón automáticamente
        if (estoyPreparadoButton != null)
        {
            estoyPreparadoButton.onClick.RemoveAllListeners();
            estoyPreparadoButton.onClick.AddListener(BotonEstoyPreparado);
        }
        else
        {
            Debug.LogError("[ControlTutorialManager] ¡No has asignado el botón 'Estoy Preparado'!");
        }

        // 4. Pausar la música (espera un frame para que el GameMusicManager ya haya arrancado)
        StartCoroutine(PausarMusicaTrasUnFrame());
    }

    private IEnumerator PausarMusicaTrasUnFrame()
    {
        yield return null;

        if (!controlesActivos) yield break;

        if (musicManager != null)
        {
            musicManager.PauseMusic();
            musicaPausada = true;
        }
        else
        {
            Debug.LogWarning("[ControlTutorialManager] No se encontró el GameMusicManager en la escena.");
        }
    }

    public void BotonEstoyPreparado()
    {
        controlesActivos = false;

        // 1. Ocultar el panel de controles
        if (panelControles != null)
        {
            panelControles.SetActive(false);
        }

        // 2. Continuar la música donde se pausó
        if (musicManager != null && musicaPausada)
        {
            musicManager.ResumeMusic();
            musicaPausada = false;
        }

        // 3. Reanudar el tiempo del juego
        Time.timeScale = 1f;

        // 4. Desactivar este script
        this.enabled = false;
    }
}