using UnityEngine;

/// <summary>
/// Guarda el progreso SOLO en memoria: sobrevive al cambio de escenas
/// (menú -> nivel -> menú...) pero se borra al cerrar el juego.
/// </summary>
public static class ProgresoJuego
{
    public const int TotalNiveles = 3;

    public static bool PartidaEnCurso { get; private set; }
    public static int NivelMaximoDesbloqueado { get; private set; }

    // Garantiza que el progreso empiece limpio en cada ejecución (también en el Editor)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetAlIniciar()
    {
        Reiniciar();
    }

    /// <summary>Botón "Nueva partida": borra todo y deja solo el nivel 1.</summary>
    public static void NuevaPartida()
    {
        PartidaEnCurso = true;
        NivelMaximoDesbloqueado = 1;
    }

    /// <summary>Desbloquea un nivel (nunca baja el progreso ya alcanzado).</summary>
    public static void DesbloquearNivel(int nivel)
    {
        nivel = Mathf.Clamp(nivel, 1, TotalNiveles);

        if (!PartidaEnCurso)
        {
            PartidaEnCurso = true;
            NivelMaximoDesbloqueado = 1;
        }

        if (nivel > NivelMaximoDesbloqueado)
            NivelMaximoDesbloqueado = nivel;
    }

    public static bool NivelDesbloqueado(int nivel)
    {
        return PartidaEnCurso && nivel >= 1 && nivel <= NivelMaximoDesbloqueado;
    }

    public static void Reiniciar()
    {
        PartidaEnCurso = false;
        NivelMaximoDesbloqueado = 0;
    }
}