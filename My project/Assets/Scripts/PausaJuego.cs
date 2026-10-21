using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausaJuego : MonoBehaviour
{
    [Header("Panel de pausa")]
    [Tooltip("Panel (Canvas o hijo de un Canvas) con los botones Continuar y Volver al menú")]
    public GameObject panelPausa;

    [Header("Botón de pausa en pantalla")]
    [Tooltip("Botón visible durante el juego. Se conecta solo (no hace falta tocar su OnClick). Se oculta mientras el juego está en pausa.")]
    public Button botonPausa;

    [Header("Volver al menú")]
    [Tooltip("Número de escena del menú en Build Profiles / Build Settings (normalmente 0)")]
    public int escenaMenu = 0;

    [Header("Teclado")]
    public bool usarTeclaParaPausar = true;
    public KeyCode teclaPausa = KeyCode.Escape;

    private bool pausado = false;
    private float escalaDeTiempoAnterior = 1f;

    private void Start()
    {
        pausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);

        if (botonPausa != null)
        {
            botonPausa.onClick.RemoveAllListeners();
            botonPausa.onClick.AddListener(Pausar);
            botonPausa.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (usarTeclaParaPausar && Input.GetKeyDown(teclaPausa))
            AlternarPausa();
    }

    // --- Conecta estos métodos al OnClick() de los botones ---

    // Botón de pausa (el que está durante el juego)
    public void Pausar()
    {
        if (pausado) return;

        // Si el juego ya está congelado por otra cosa (tutorial, WinPanel, etc.), no se puede pausar
        if (Time.timeScale <= 0f) return;

        pausado = true;
        escalaDeTiempoAnterior = Time.timeScale;

        Time.timeScale = 0f;        // Congela el juego
        AudioListener.pause = true; // Pausa TODO el audio (la música queda en pausa, no se corta)

        if (panelPausa != null) panelPausa.SetActive(true);
        if (botonPausa != null) botonPausa.gameObject.SetActive(false);
    }

    // Botón "Continuar"
    public void Continuar()
    {
        if (!pausado) return;
        pausado = false;

        if (panelPausa != null) panelPausa.SetActive(false);
        if (botonPausa != null) botonPausa.gameObject.SetActive(true);

        Time.timeScale = escalaDeTiempoAnterior > 0f ? escalaDeTiempoAnterior : 1f;
        AudioListener.pause = false; // La música sigue justo donde estaba
    }

    // Botón "Volver al menú"
    public void VolverAlMenu()
    {
        pausado = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene(escenaMenu);
    }

    public void AlternarPausa()
    {
        if (pausado) Continuar();
        else Pausar();
    }

    private void OnDestroy()
    {
        // Seguridad: si se cambia de escena estando en pausa, no deja el audio ni el tiempo congelados
        if (pausado)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}