using UnityEngine;
using UnityEngine.UI;

public class VidaJefe : MonoBehaviour
{
    [Header("Configuración de Impactos")]
    [Tooltip("Cantidad de disparos necesarios para bajar 1 división de la barra")]
    public int disparosPorDivision = 20;
    
    [Tooltip("Cantidad total de divisiones de vida (9 en tu caso)")]
    public int totalDivisiones = 9;

    [Header("UI y Canvas Final")]
    public Image imagenBarraVida;
    [Tooltip("Arrastra los 10 sprites: Element 0 = Vacía, Element 9 = Llena")]
    public Sprite[] spritesBarra;
    public GameObject canvasVictoria;

    private int impactosTotalesMaximos;
    private int impactosTotalesActuales;

    private float cooldownDanio = 0.05f; // Permite recibir ráfagas rápidas de balas
    private float ultimoDanioTime = -1f;

    private void Awake()
    {
        // 9 divisiones * 20 disparos = 180 impactos totales
        impactosTotalesMaximos = totalDivisiones * disparosPorDivision;
        impactosTotalesActuales = impactosTotalesMaximos;
    }

    private void Start()
    {
        impactosTotalesActuales = impactosTotalesMaximos;

        if (canvasVictoria != null)
        {
            canvasVictoria.SetActive(false);
        }

        ActualizarBarraUI();
    }

    public void RecibirDano(int cantidad)
    {
        if (Time.time < ultimoDanioTime + cooldownDanio || impactosTotalesActuales <= 0) return;

        ultimoDanioTime = Time.time;

        // Le restamos 1 impacto por cada bala recibida
        impactosTotalesActuales -= cantidad;
        if (impactosTotalesActuales < 0) impactosTotalesActuales = 0;

        Debug.Log("Impactos restantes del Jefe: " + impactosTotalesActuales + " / " + impactosTotalesMaximos);

        ActualizarBarraUI();

        if (impactosTotalesActuales <= 0)
        {
            DerrotarJefe();
        }
    }

    private void ActualizarBarraUI()
    {
        if (imagenBarraVida != null && spritesBarra != null && spritesBarra.Length > 0)
        {
            // Calcula cuántas divisiones/barras le quedan proporcionalmente
            // Ej: 180 a 161 impactos = Sprite 9 | 160 a 141 impactos = Sprite 8 | etc.
            int indiceSprite = Mathf.CeilToInt((float)impactosTotalesActuales / disparosPorDivision);
            
            // Asegura que el índice esté dentro del rango del arreglo (0 a 9)
            indiceSprite = Mathf.Clamp(indiceSprite, 0, spritesBarra.Length - 1);
            
            imagenBarraVida.sprite = spritesBarra[indiceSprite];
        }
    }

    private void DerrotarJefe()
    {
        Debug.Log("¡El Jefe ha sido derrotado!");

        if (canvasVictoria != null)
        {
            canvasVictoria.SetActive(true);
        }

        // Apaga gráficos y colisiones
        foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
        {
            c.enabled = false;
        }

        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            r.enabled = false;
        }
    }
}