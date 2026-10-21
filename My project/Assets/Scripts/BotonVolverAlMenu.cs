using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Para botones de "Volver al menú". Con el booleano activado, además desbloquea el nivel que elijas.
/// Dos formas de usarlo:
///  A) Conexión automática: ponlo en el botón y deja "Conectar Automaticamente" activado.
///  B) Manual: desactiva "Conectar Automaticamente" y en el OnClick() del botón elige BotonVolverAlMenu > Ejecutar().
/// </summary>
public class BotonVolverAlMenu : MonoBehaviour
{
    [Header("Desbloqueo de nivel")]
    [Tooltip("Activado: al pulsar este botón se desbloquea el nivel indicado. Desactivado: no desbloquea nada.")]
    public bool desbloquearNivel = true;

    [Tooltip("Nivel que se desbloquea (1, 2 o 3)")]
    [Range(1, 3)]
    public int nivelADesbloquear = 2;

    [Header("Volver al menú")]
    [Tooltip("Activado: este script también carga la escena del menú. Desactívalo si tu botón ya la carga por otro lado.")]
    public bool cargarEscenaMenu = true;

    [Tooltip("Número de escena del menú en Build Profiles / Build Settings (normalmente 0)")]
    public int escenaMenu = 0;

    [Header("Conexión")]
    [Tooltip("Activado: se engancha solo al Button de este objeto. Desactívalo si prefieres llamar a Ejecutar() desde el OnClick() del botón.")]
    public bool conectarAutomaticamente = true;

    private void Awake()
    {
        if (!conectarAutomaticamente) return;

        Button boton = GetComponent<Button>();
        if (boton != null)
            boton.onClick.AddListener(Ejecutar);
        else
            Debug.LogWarning("[BotonVolverAlMenu] '" + name + "' no tiene un Button. Ponlo en el botón o conéctalo manualmente al OnClick().");
    }

    // Puedes ponerlo directamente en el OnClick() del botón
    public void Ejecutar()
    {
        if (desbloquearNivel)
        {
            ProgresoJuego.DesbloquearNivel(nivelADesbloquear);
            Debug.Log("[BotonVolverAlMenu] Nivel " + nivelADesbloquear + " desbloqueado. Máximo actual: " + ProgresoJuego.NivelMaximoDesbloqueado);
        }

        if (cargarEscenaMenu)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(escenaMenu);
        }
    }
}