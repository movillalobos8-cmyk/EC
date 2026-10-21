using UnityEngine;

/// <summary>
/// Checkpoint usable desde el Inspector. Ponlo en un objeto de la escena del nivel
/// y elige qué nivel desbloquea. Por defecto se activa solo al entrar a la escena.
/// </summary>
public class CheckpointNivel : MonoBehaviour
{
    [Tooltip("Número de nivel (1, 2 o 3) que queda desbloqueado al llegar a este checkpoint")]
    [Range(1, 3)]
    public int nivelQueDesbloquea = 1;

    [Tooltip("Si está activo, se registra automáticamente al cargar la escena")]
    public bool activarAlEntrar = true;

    private void Start()
    {
        if (activarAlEntrar) Activar();
    }

    // También puedes llamarlo desde un UnityEvent / botón / otro script
    public void Activar()
    {
        ProgresoJuego.DesbloquearNivel(nivelQueDesbloquea);
        Debug.Log("[Checkpoint] Nivel " + nivelQueDesbloquea + " desbloqueado. Máximo actual: " + ProgresoJuego.NivelMaximoDesbloqueado);
    }
}