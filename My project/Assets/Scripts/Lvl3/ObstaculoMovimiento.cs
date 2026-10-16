using UnityEngine;

namespace Nivel3
{
    public class ObstaculoMovimiento : MonoBehaviour
    {
        public float speed = 5f;

        [HideInInspector]
        public bool tutorialObstacle = false;

        void Update()
        {
            // Mueve hacia la izquierda
            transform.Translate(Vector2.left * speed * Time.deltaTime);

            // Destruye el objeto cuando sale de la pantalla por la izquierda
            if (!tutorialObstacle && Camera.main != null)
            {
                if (transform.position.x < Camera.main.transform.position.x - 30f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}