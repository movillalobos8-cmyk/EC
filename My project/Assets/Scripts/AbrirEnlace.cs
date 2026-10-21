using UnityEngine;

/// <summary>
/// Abre direcciones web (redes sociales, página, etc.) en el navegador.
/// Ponlo en un objeto (por ejemplo, en el del menú) y conecta los métodos al OnClick() de cada botón:
///   - Botón de Instagram -> AbrirInstagram()
///   - Botón de la página web -> AbrirWeb()
///   - Cualquier otro enlace -> Abrir() (usa el campo "Url Personalizada")
/// </summary>
public class AbrirEnlace : MonoBehaviour
{
    [Header("Enlaces del juego")]
    public string urlInstagram = "https://www.instagram.com/ec_entertainment.ok/";
    public string urlWeb = "https://ecentertainment.netlify.app/";

    [Header("Enlace extra (opcional)")]
    [Tooltip("Para cualquier otro botón. Dirección completa, con https://")]
    public string urlPersonalizada = "https://";

    public void AbrirInstagram()
    {
        AbrirURL(urlInstagram);
    }

    public void AbrirWeb()
    {
        AbrirURL(urlWeb);
    }

    public void Abrir()
    {
        AbrirURL(urlPersonalizada);
    }

    private void AbrirURL(string direccion)
    {
        if (string.IsNullOrWhiteSpace(direccion) || direccion.Trim() == "https://")
        {
            Debug.LogWarning("[AbrirEnlace] '" + name + "' no tiene una URL configurada.");
            return;
        }

        Application.OpenURL(direccion.Trim());
    }
}