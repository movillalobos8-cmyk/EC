using UnityEngine;

namespace Nivel3
{
    public class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform nave;             // Arrastra el Transform de la nave en el Inspector
        [SerializeField] private float offsetHorizontal = 3f;  // Separación horizontal adelante de la nave

        private float posZ;

        void Start()
        {
            posZ = transform.position.z;
        }

        void LateUpdate()
        {
            if (nave != null)
            {
                // Mantiene el movimiento de la cámara siguiendo a la nave en X
                transform.position = new Vector3(nave.position.x + offsetHorizontal, transform.position.y, posZ);
            }
        }
    }
}