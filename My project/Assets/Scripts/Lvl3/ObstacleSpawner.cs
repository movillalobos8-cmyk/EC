using System.Collections;
using UnityEngine;

namespace Nivel3
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Prefabs de Obstáculos")]
        public GameObject[] prefabsObstaculos; // Arrastra aquí tus prefabs con las distintas texturas

        [Header("Tiempo de Espera")]
        [Tooltip("Segundos que esperará antes de spawnear el primer obstáculo")]
        public float delayInicial = 2f; 

        [Header("Configuración de Tiempo")]
        public float spawnRate = 2.5f;

        [Header("Distancia por delante de la Cámara")]
        public float distanceFromCamera = 15f;

        [Header("Alturas Fijas (Arriba, Medio, Abajo)")]
        public float posYArriba = 3f;
        public float posYMedio = 0f;
        public float posYAbajo = -3f;

        private float timer = 0f;
        private int pasoPatron = 0;
        private bool listoParaSpawnear = false;

        private void OnEnable()
        {
            // Reinicia el estado cuando el script se activa (al pulsar "Estoy Preparado")
            listoParaSpawnear = false;
            timer = 0f;
            pasoPatron = 0;

            StartCoroutine(EsperarDelayInicial());
        }

        private IEnumerator EsperarDelayInicial()
        {
            // Espera el tiempo configurado en el Inspector
            yield return new WaitForSeconds(delayInicial);
            listoParaSpawnear = true;
        }

        void Update()
        {
            // No hace nada hasta que pase el tiempo del delay inicial
            if (!listoParaSpawnear) return;

            timer += Time.deltaTime;

            if (timer >= spawnRate)
            {
                SpawnObstacle();
                timer = 0f;
            }
        }

        void SpawnObstacle()
        {
            if (prefabsObstaculos == null || prefabsObstaculos.Length == 0)
            {
                Debug.LogError("No hay prefabs asignados en ObstacleSpawner.");
                return;
            }

            if (Camera.main == null)
            {
                Debug.LogError("No se encontró Camera.main en la escena.");
                return;
            }

            // Calcula la posición de spawn en X con respecto a la cámara
            float spawnX = Camera.main.transform.position.x + distanceFromCamera;

            // Secuencia del patrón:
            // 0 -> Arriba
            // 1 -> Medio
            // 2 -> Medio y Abajo juntos
            switch (pasoPatron)
            {
                case 0:
                    InstanciarEnPosicion(spawnX, posYArriba);
                    break;
                case 1:
                    InstanciarEnPosicion(spawnX, posYMedio);
                    break;
                case 2:
                    InstanciarEnPosicion(spawnX, posYMedio);
                    InstanciarEnPosicion(spawnX, posYAbajo);
                    break;
            }

            // Avanza al siguiente paso del patrón
            pasoPatron = (pasoPatron + 1) % 3;
        }

        void InstanciarEnPosicion(float x, float y)
        {
            int indiceAleatorio = Random.Range(0, prefabsObstaculos.Length);
            GameObject prefabElegido = prefabsObstaculos[indiceAleatorio];

            if (prefabElegido != null)
            {
                Vector3 spawnPosition = new Vector3(x, y, 0f);
                Instantiate(prefabElegido, spawnPosition, Quaternion.identity);
            }
        }
    }
}