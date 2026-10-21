using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuProgreso : MonoBehaviour
{
    [System.Serializable]
    public class NivelBoton
    {
        [Tooltip("Botón de este nivel")]
        public Button boton;

        [Tooltip("Número de escena en Build Profiles / Build Settings (el menú suele ser 0)")]
        public int numeroDeEscena = 1;

        [Tooltip("Opcional: icono de candado que se muestra mientras el nivel está bloqueado")]
        public GameObject candado;
    }

    [Header("Botones del menú")]
    public GameObject botonNuevaPartida;
    public GameObject botonContinuar;
    [Tooltip("Si está activo, 'Nueva partida' se oculta cuando ya hay progreso y solo queda 'Continuar'")]
    public bool ocultarNuevaPartidaSiHayProgreso = false;

    [Header("Panel de niveles")]
    public GameObject panelNiveles;
    [Tooltip("Opcional: panel del menú principal, se oculta mientras se ven los niveles")]
    public GameObject panelMenuPrincipal;

    [Header("Niveles (Elemento 0 = Nivel 1, Elemento 1 = Nivel 2, Elemento 2 = Nivel 3)")]
    public NivelBoton[] niveles = new NivelBoton[3];

    private void Start()
    {
        Time.timeScale = 1f;

        // Los botones de nivel se configuran por código (no los conectes a mano en el Inspector)
        for (int i = 0; i < niveles.Length; i++)
        {
            if (niveles[i] == null || niveles[i].boton == null) continue;

            int numeroNivel = i + 1;
            niveles[i].boton.onClick.RemoveAllListeners();
            niveles[i].boton.onClick.AddListener(() => IrANivel(numeroNivel));
        }

        ActualizarMenu();
    }

    private void ActualizarMenu()
    {
        bool hayProgreso = ProgresoJuego.PartidaEnCurso;

        if (botonContinuar != null) botonContinuar.SetActive(hayProgreso);

        if (botonNuevaPartida != null)
            botonNuevaPartida.SetActive(!(hayProgreso && ocultarNuevaPartidaSiHayProgreso));

        if (panelNiveles != null) panelNiveles.SetActive(false);
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(true);

        ActualizarNiveles();
    }

    private void ActualizarNiveles()
    {
        for (int i = 0; i < niveles.Length; i++)
        {
            if (niveles[i] == null) continue;

            bool desbloqueado = ProgresoJuego.NivelDesbloqueado(i + 1);

            if (niveles[i].boton != null) niveles[i].boton.interactable = desbloqueado;
            if (niveles[i].candado != null) niveles[i].candado.SetActive(!desbloqueado);
        }
    }

    // --- Conecta estos métodos al OnClick() de cada botón ---

    // Botón "Nueva partida"
    public void NuevaPartida()
    {
        ProgresoJuego.NuevaPartida();

        if (niveles.Length > 0 && niveles[0] != null)
            CargarEscena(niveles[0].numeroDeEscena);
    }

    // Botón "Continuar": abre la sección de niveles
    public void Continuar()
    {
        ActualizarNiveles();
        if (panelNiveles != null) panelNiveles.SetActive(true);
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(false);
    }

    // Botón "Volver" dentro del panel de niveles
    public void CerrarPanelNiveles()
    {
        if (panelNiveles != null) panelNiveles.SetActive(false);
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(true);
    }

    public void IrANivel(int numeroNivel)
    {
        if (!ProgresoJuego.NivelDesbloqueado(numeroNivel)) return;
        if (numeroNivel < 1 || numeroNivel > niveles.Length) return;

        NivelBoton nivel = niveles[numeroNivel - 1];
        if (nivel == null) return;

        Debug.Log("[MenuProgreso] Nivel " + numeroNivel + " -> escena " + nivel.numeroDeEscena);
        CargarEscena(nivel.numeroDeEscena);
    }

    private void CargarEscena(int indice)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(indice);
    }
}