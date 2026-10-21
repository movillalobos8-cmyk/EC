using UnityEngine;

namespace Nivel3
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Tutorial")]
        [Tooltip("Arrastra aquí el SecretariaTutorialManagerNivel3. Si lo dejas vacío, lo busca solo.")]
        public SecretariaTutorialManagerNivel3 tutorial;

        [Header("Prefabs de Obstáculos")]
        public GameObject[] prefabsObstaculos; // Arrastra aquí tus prefabs con las distintas texturas

        [Header("Tiempo de Espera")]
        [Tooltip("Segundos que esperará DESPUÉS de terminar el tutorial antes de spawnear el primer obstáculo")]
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
        private float timerDelayInicial = 0f;
        private int pasoPatron = 0;
        private bool listoParaSpawnear = false;

        private void Awake()
        {
            if (tutorial == null)
                tutorial = FindFirstObjectByType<SecretariaTutorialManagerNivel3>();
        }

        private void OnEnable()
        {
            // Reinicia el estado cada vez que el script se activa
            listoParaSpawnear = false;
            timer = 0f;
            timerDelayInicial = 0f;
            pasoPatron = 0;
        }

        void Update()
        {
            // 1. Mientras el tutorial no haya terminado (no se pulsó "Estoy Preparado"), no cuenta nada
            if (tutorial != null && !tutorial.TutorialTerminado) return;

            // 2. Si el juego está congelado por otra razón, tampoco cuenta
            if (Time.timeScale <= 0f) return;

            // 3. Delay inicial: empieza a contar recién cuando terminó el tutorial
            if (!listoParaSpawnear)
            {
                timerDelayInicial += Time.deltaTime;
                if (timerDelayInicial >= delayInicial)
                    listoParaSpawnear = true;
                else
                    return;
            }

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