using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Botones del menú")]
    public GameObject botonNuevaPartida;
    public GameObject botonContinuar;
    [Tooltip("Si está activo, 'Nueva partida' se oculta cuando ya hay progreso y solo queda 'Continuar'")]
    public bool ocultarNuevaPartidaSiHayProgreso = false;

    [Header("Panel de niveles")]
    public GameObject panelNiveles;
    [Tooltip("Opcional: panel del menú principal, se oculta mientras se ven los niveles")]
    public GameObject panelMenuPrincipal;

    [Header("Niveles (índices de escena del Build Profiles / Build Settings)")]
    public int[] escenasNiveles = new int[] { 1, 2, 3 };
    public Button[] botonesNiveles = new Button[3];
    [Tooltip("Opcional: iconos de candado, uno por nivel")]
    public GameObject[] candados = new GameObject[3];

    private void Start()
    {
        Time.timeScale = 1f;

        // Los botones de nivel se configuran por código (no los conectes a mano en el Inspector)
        for (int i = 0; i < botonesNiveles.Length; i++)
        {
            if (botonesNiveles[i] == null) continue;

            int numeroNivel = i + 1;
            botonesNiveles[i].onClick.RemoveAllListeners();
            botonesNiveles[i].onClick.AddListener(() => IrANivel(numeroNivel));
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
        for (int i = 0; i < botonesNiveles.Length; i++)
        {
            bool desbloqueado = ProgresoJuego.NivelDesbloqueado(i + 1);

            if (botonesNiveles[i] != null) botonesNiveles[i].interactable = desbloqueado;
            if (i < candados.Length && candados[i] != null) candados[i].SetActive(!desbloqueado);
        }
    }

    // --- Conecta estos métodos al OnClick() de cada botón ---

    // Botón "Nueva partida"
    public void NuevaPartida()
    {
        ProgresoJuego.NuevaPartida();
        CargarEscena(escenasNiveles[0]);
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
        if (numeroNivel < 1 || numeroNivel > escenasNiveles.Length) return;

        CargarEscena(escenasNiveles[numeroNivel - 1]);
    }

    private void CargarEscena(int indice)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(indice);
    }
}