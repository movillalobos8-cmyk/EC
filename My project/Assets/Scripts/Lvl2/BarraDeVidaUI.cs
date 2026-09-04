using UnityEngine;
using UnityEngine.UI;

public class BarraDeVidaUI : MonoBehaviour
{
    [Header("Componente de Imagen UI")]
    public Image imagenBarraVida; 

    [Header("Sprites de la Barra (Arreglo de 10 elementos)")]
    // Asigna las imágenes en este orden exacto en el Inspector:
    // Element 0: Imagen VACÍA (0 líneas)
    // Element 1: 1 línea
    // Element 2: 2 líneas
    // Element 3: 3 líneas
    // Element 4: 4 líneas
    // Element 5: 5 líneas
    // Element 6: 6 líneas
    // Element 7: 7 líneas
    // Element 8: 8 líneas
    // Element 9: Imagen LLENA (9 líneas)
    public Sprite[] spritesBarra; 

    public void ActualizarBarra(int vidaActual)
    {
        if (imagenBarraVida == null || spritesBarra == null || spritesBarra.Length == 0) return;

        // Asegura que la vida se mantenga entre 0 y el número de sprites disponibles
        int indiceSprite = Mathf.Clamp(vidaActual, 0, spritesBarra.Length - 1);

        // Muestra la imagen llena cuando la vida es 9, e irá bajando hacia la vacía (0) a medida que recibe disparos
        imagenBarraVida.sprite = spritesBarra[indiceSprite];
    }
}