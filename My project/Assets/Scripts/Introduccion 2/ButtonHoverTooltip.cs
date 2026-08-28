using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Panel a mostrar u ocultar")]
    public GameObject panelDefinicion;

    void Awake()
    {
        // Se asegura de que la definición empiece oculta
        if (panelDefinicion != null)
        {
            panelDefinicion.SetActive(false);
        }
    }

    // Se dispara cuando el puntero ENTRA al botón
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (panelDefinicion != null)
        {
            panelDefinicion.SetActive(true);
        }
    }

    // Se dispara cuando el puntero SALE del botón
    public void OnPointerExit(PointerEventData eventData)
    {
        if (panelDefinicion != null)
        {
            panelDefinicion.SetActive(false);
        }
    }

    // Al desactivar el botón (por ejemplo al hacer clic), ocultamos la definición
    void OnDisable()
    {
        if (panelDefinicion != null)
        {
            panelDefinicion.SetActive(false);
        }
    }
}