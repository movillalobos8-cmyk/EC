using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuPrincipal : MonoBehaviour
{
    [Header("--- BOTONES MENÚ PRINCIPAL ---")]
    public Button botonNuevaPartida;
    public Button botonContinuar;
    public TMP_Text textoBotonContinuar; // Opcional: muestra "Continuar (Niv. X)"
    public Button botonSelectorNiveles;

    [Header("--- PANEL SELECTOR DE NIVELES ---")]
    public GameObject panelSelectorNiveles; // El panel pop-up que muestra los botones de cada nivel
    public Button[] botonesDeNivel;        // Arrastra aquí: Element 0 = Nivel 1, Element 1 = Nivel 2, etc.

    private void Start()
    {
        ActualizarEstadoMenu();
        
        if (panelSelectorNiveles != null)
        {
            panelSelectorNiveles.SetActive(false);
        }
    }

    private void ActualizarEstadoMenu()
    {
        int nivelAlcanzado = GameDataManager.ObtenerNivelAlcanzado();

        // Si ya pasaron al menos el Nivel 1 (nivelAlcanzado >= 2)
        if (nivelAlcanzado > 1)
        {
            if (botonContinuar != null)
            {
                botonContinuar.gameObject.SetActive(true);
                if (textoBotonContinuar != null)
                {
                    textoBotonContinuar.text = "Continuar (Nivel " + nivelAlcanzado + ")";
                }
            }

            if (botonSelectorNiveles != null)
            {
                botonSelectorNiveles.gameObject.SetActive(true);
            }
        }
        else
        {
            // Primera vez jugando o le dio a Nueva Partida
            if (botonContinuar != null) botonContinuar.gameObject.SetActive(false);
            if (botonSelectorNiveles != null) botonSelectorNiveles.gameObject.SetActive(false);
        }

        // Configura la interacción de los botones dentro del Selector de Niveles
        if (botonesDeNivel != null)
        {
            for (int i = 0; i < botonesDeNivel.Length; i++)
            {
                int numNivel = i + 1;
                // Si el nivel está dentro del progreso alcanzado, se activa; si no, queda bloqueado
                botonesDeNivel[i].interactable = (numNivel <= nivelAlcanzado);
            }
        }
    }

    // --- ACCIONES DE LOS BOTONES ---

    // Botón "Nueva Partida"
    public void BotonNuevaPartida()
    {
        GameDataManager.ReiniciarProgreso();
        SceneManager.LoadScene("Nivel1"); // Asegúrate de que tu primera escena se llame así en el Build Settings
    }

    // Botón "Continuar"
    public void BotonContinuar()
    {
        int nivelACargar = GameDataManager.ObtenerNivelAlcanzado();
        SceneManager.LoadScene("Nivel" + nivelACargar); // Carga "Nivel2", "Nivel3", etc.
    }

    // Botón "Selector de Niveles" (Abre la ventana con los niveles)
    public void AbrirSelectorNiveles()
    {
        if (panelSelectorNiveles != null)
        {
            panelSelectorNiveles.SetActive(true);
        }
    }

    // Botón de Cerrar/Volver del Selector de Niveles
    public void CerrarSelectorNiveles()
    {
        if (panelSelectorNiveles != null)
        {
            panelSelectorNiveles.SetActive(false);
        }
    }

    // Función para los botones del panel de selector ("Nivel 1", "Nivel 2", etc.)
    public void CargarNivelDirecto(int numeroNivel)
    {
        SceneManager.LoadScene("Nivel" + numeroNivel);
    }
}