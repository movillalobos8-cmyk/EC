using UnityEngine;
using UnityEngine.SceneManagement;

public class scenetest : MonoBehaviour
{
    [Header("Configuración de Escena")]
    public int numeroEscena = 4;

    // DEBE SER 'public', 'void' Y NO TENER PARÁMETROS EN ESTE MÉTODO
    public void CambiarDeEscena()
    {
        SceneManager.LoadScene(numeroEscena);
    }
}