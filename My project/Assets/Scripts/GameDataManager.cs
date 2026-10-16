using UnityEngine;

public static class GameDataManager
{
    private const string CLAVE_NIVEL_DESBLOQUEADO = "NivelDesbloqueado";

    // Guarda el nivel actual alcanzado
    public static void GuardarNivelAlcanzado(int numeroNivel)
    {
        int nivelActualGuardado = ObtenerNivelAlcanzado();
        
        // Solo actualiza si avanzamos a un nivel superior al guardado
        if (numeroNivel > nivelActualGuardado)
        {
            PlayerPrefs.SetInt(CLAVE_NIVEL_DESBLOQUEADO, numeroNivel);
            PlayerPrefs.Save();
        }
    }

    // Devuelve el nivel en el que se quedó (por defecto es 1)
    public static int ObtenerNivelAlcanzado()
    {
        return PlayerPrefs.GetInt(CLAVE_NIVEL_DESBLOQUEADO, 1);
    }

    // Reinicia los datos cuando se presiona "Nueva Partida"
    public static void ReiniciarProgreso()
    {
        PlayerPrefs.SetInt(CLAVE_NIVEL_DESBLOQUEADO, 1);
        PlayerPrefs.Save();
    }
}