using UnityEngine;
using UnityEngine.UI;

public class ControlTutorialManager : MonoBehaviour
{
    [Header("UI de Controles")]
    public GameObject panelControles;
    public Button estoyPreparadoButton;

    void Start()
    {
        // 1. Congelar el tiempo al iniciar la escena
        Time.timeScale = 0f;

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
    }

    public void BotonEstoyPreparado()
    {
        // 1. Ocultar el panel de controles
        if (panelControles != null)
        {
            panelControles.SetActive(false);
        }

        // 2. Reanudar el tiempo del juego
        Time.timeScale = 1f;

        // 3. Desactivar este script
        this.enabled = false;
    }
}