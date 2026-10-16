using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Nivel3
{
    public class SecretariaTutorialManager : MonoBehaviour
    {
        [Header("1. UI Tutorial 1 (Secretaria)")]
        public GameObject panelTutorial1;
        public TMP_Text dialogueText1;
        public Button entendidoButton1;

        [TextArea(6, 12)]
        public string tutorialText1 = "Bienvenido. Estás dentro del sistema circulatorio en una pequeña nave, viajando por las venas del brazo de Denis.\n\nComo bien dijimos antes, la nave simula ser un glóbulo rojo y por eso el cuerpo no la rechaza, aún así, ten cuidado.";

        [Header("2. UI de Controles")]
        public GameObject panelControles;
        public Button entendidoButton2; // Botón en el panel de controles

        [Header("3. UI Tutorial 2 (Secretaria Final)")]
        public GameObject panelTutorial2;
        public TMP_Text dialogueText2;
        public Button estoyPreparadoButton;

        [TextArea(8, 15)]
        public string tutorialText2 = "Lo has hecho bien. Ahora que ya lo entiendes, vendrán más glóbulos rojos.\n\nSi bien la nave simula ser uno, nunca logrará comportarse como tal, por eso, para no dañar a Denis tienes que tener mucho cuidado de no chocarte con ninguna. Cuida bien a tu paciente.\n\nPor otro lado, tu objetivo es encontrar la Alteplasa, una medicina que te va a ayudar a disipar el problema principal de la trombosis.\n\nMantente alerta… el sistema no es seguro.";

        [Header("Secretaria")]
        public Image secretariaImage;
        public Sprite idleSprite;
        public Sprite talkingSprite;

        [Header("Obstáculo de Tutorial")]
        public GameObject globuloTutorialPrefab;

        [Header("Spawners del Nivel")]
        public GameObject[] obstacleSpawners;

        [Header("Configuración de Texto")]
        public float textSpeed = 0.03f;

        private string[] parrafostutorial1;
        private string[] parrafostutorial2;
        private int currentParagraphIndex = 0;
        private bool isTyping = false;
        private Coroutine typingCoroutine;
        private GameObject tutorialObstacle;

        private enum EstadoTutorial { Dialogo1, Controles, EsperandoObstaculo, Dialogo2, JuegoEnCurso }
        private EstadoTutorial estadoActual = EstadoTutorial.Dialogo1;

        private void Start()
        {
            if (panelTutorial1 != null) panelTutorial1.SetActive(true);
            if (panelControles != null) panelControles.SetActive(false);
            if (panelTutorial2 != null) panelTutorial2.SetActive(false);

            if (entendidoButton1 != null)
            {
                entendidoButton1.gameObject.SetActive(false);
                entendidoButton1.onClick.RemoveAllListeners();
                entendidoButton1.onClick.AddListener(BotonEntendido1);
            }

            if (entendidoButton2 != null)
            {
                entendidoButton2.onClick.RemoveAllListeners();
                entendidoButton2.onClick.AddListener(BotonEntendido2);
            }

            if (estoyPreparadoButton != null)
            {
                estoyPreparadoButton.gameObject.SetActive(false);
                estoyPreparadoButton.onClick.RemoveAllListeners();
                estoyPreparadoButton.onClick.AddListener(BotonEstoyPreparado);
            }

            DesactivarSpawners();

            Time.timeScale = 0f;

            // Procesar y mostrar el primer texto
            parrafostutorial1 = SepararTextoEnParrafos(tutorialText1);
            currentParagraphIndex = 0;

            if (parrafostutorial1.Length > 0)
            {
                IniciarTipeado(dialogueText1, parrafostutorial1[0], entendidoButton1, parrafostutorial1);
            }
            else if (entendidoButton1 != null)
            {
                entendidoButton1.gameObject.SetActive(true);
            }
        }

        private string[] SepararTextoEnParrafos(string textoBruto)
        {
            if (string.IsNullOrEmpty(textoBruto)) return new string[0];

            string textoLimpio = textoBruto.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
            string[] parrafos = textoLimpio.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            if (parrafos.Length == 0)
            {
                parrafos = new string[] { textoLimpio };
            }

            return parrafos;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                if (estadoActual == EstadoTutorial.Dialogo1)
                {
                    AvanzarOCompletarTexto(dialogueText1, parrafostutorial1, entendidoButton1);
                }
                else if (estadoActual == EstadoTutorial.Dialogo2)
                {
                    AvanzarOCompletarTexto(dialogueText2, parrafostutorial2, estoyPreparadoButton);
                }
            }
        }

        private void AvanzarOCompletarTexto(TMP_Text campoTexto, string[] parrafos, Button botonAparecer)
        {
            if (parrafos == null || parrafos.Length == 0) return;

            if (isTyping)
            {
                // Detiene el tipeado actual y muestra todo el párrafo de golpe
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);

                if (campoTexto != null)
                {
                    campoTexto.text = parrafos[currentParagraphIndex];
                }

                if (secretariaImage != null && idleSprite != null) secretariaImage.sprite = idleSprite;
                isTyping = false;

                if (currentParagraphIndex >= parrafos.Length - 1 && botonAparecer != null)
                {
                    botonAparecer.gameObject.SetActive(true);
                }
            }
            else if (currentParagraphIndex < parrafos.Length - 1)
            {
                currentParagraphIndex++;
                IniciarTipeado(campoTexto, parrafos[currentParagraphIndex], botonAparecer, parrafos);
            }
        }

        private void IniciarTipeado(TMP_Text campoTexto, string parrafo, Button botonAparecer, string[] parrafosReferencia)
        {
            // Frena cualquier corrutina de texto anterior para evitar duplicados
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            typingCoroutine = StartCoroutine(EscribirTexto(campoTexto, parrafo, botonAparecer, parrafosReferencia));
        }

        private IEnumerator EscribirTexto(TMP_Text campoTexto, string parrafo, Button botonAparecer, string[] parrafosReferencia)
        {
            isTyping = true;

            if (campoTexto == null)
            {
                Debug.LogError("[SecretariaTutorialManager] ¡Falta asignar la casilla de texto (TMP_Text) en el Inspector!");
                isTyping = false;
                if (botonAparecer != null) botonAparecer.gameObject.SetActive(true);
                yield break;
            }

            // Blanquea el texto explícitamente antes de empezar
            campoTexto.text = "";

            if (secretariaImage != null && talkingSprite != null) secretariaImage.sprite = talkingSprite;
            if (botonAparecer != null) botonAparecer.gameObject.SetActive(false);

            for (int i = 0; i < parrafo.Length; i++)
            {
                campoTexto.text += parrafo[i];
                yield return new WaitForSecondsRealtime(textSpeed);
            }

            if (secretariaImage != null && idleSprite != null) secretariaImage.sprite = idleSprite;
            isTyping = false;

            if (currentParagraphIndex >= parrafosReferencia.Length - 1 && botonAparecer != null)
            {
                botonAparecer.gameObject.SetActive(true);
            }
        }

        // PASO 1 -> CONTROLES
        public void BotonEntendido1()
        {
            if (panelTutorial1 != null) panelTutorial1.SetActive(false);

            estadoActual = EstadoTutorial.Controles;

            if (panelControles != null)
            {
                panelControles.SetActive(true);
            }
            else
            {
                BotonEntendido2();
            }
        }

        // PASO 2 -> OBSTÁCULO DE PRUEBA
        public void BotonEntendido2()
        {
            if (panelControles != null) panelControles.SetActive(false);

            estadoActual = EstadoTutorial.EsperandoObstaculo;
            Time.timeScale = 1f;

            SpawnTutorialObstacle();
        }

        private void SpawnTutorialObstacle()
        {
            if (globuloTutorialPrefab == null || Camera.main == null)
            {
                IniciarTutorial2();
                return;
            }

            Vector3 spawnPos = new Vector3(Camera.main.transform.position.x + 12f, 0f, 0f);
            tutorialObstacle = Instantiate(globuloTutorialPrefab, spawnPos, Quaternion.identity);

            ObstaculoMovimiento move = tutorialObstacle.GetComponent<ObstaculoMovimiento>();
            if (move != null)
            {
                move.tutorialObstacle = true;
            }

            StartCoroutine(CheckTutorialObstacle());
        }

        private IEnumerator CheckTutorialObstacle()
        {
            while (tutorialObstacle != null)
            {
                if (Camera.main != null && tutorialObstacle.transform.position.x < Camera.main.transform.position.x - 10f)
                {
                    Destroy(tutorialObstacle);
                    IniciarTutorial2();
                    yield break;
                }
                yield return null;
            }

            IniciarTutorial2();
        }

        // OBSTÁCULO TERMINADO -> PASO 3 (TUTORIAL 2)
        private void IniciarTutorial2()
        {
            Time.timeScale = 0f;
            estadoActual = EstadoTutorial.Dialogo2;

            // Reinicia variables de control para evitar letras duplicadas
            currentParagraphIndex = 0;
            isTyping = false;

            if (panelTutorial2 != null) panelTutorial2.SetActive(true);

            parrafostutorial2 = SepararTextoEnParrafos(tutorialText2);
            if (parrafostutorial2.Length > 0)
            {
                IniciarTipeado(dialogueText2, parrafostutorial2[0], estoyPreparadoButton, parrafostutorial2);
            }
            else if (estoyPreparadoButton != null)
            {
                estoyPreparadoButton.gameObject.SetActive(true);
            }
        }

        // PASO 3 -> INICIO DEL JUEGO
        public void BotonEstoyPreparado()
        {
            estadoActual = EstadoTutorial.JuegoEnCurso;

            if (panelTutorial2 != null) panelTutorial2.SetActive(false);

            ActivarSpawners();

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.StartLevel();
            }

            Time.timeScale = 1f;
            this.enabled = false;
        }

        private void DesactivarSpawners()
        {
            if (obstacleSpawners == null) return;

            foreach (GameObject spawnerObj in obstacleSpawners)
            {
                if (spawnerObj != null)
                {
                    ObstacleSpawner script = spawnerObj.GetComponent<ObstacleSpawner>();
                    if (script != null) script.enabled = false;
                }
            }
        }

        private void ActivarSpawners()
        {
            if (obstacleSpawners == null) return;

            foreach (GameObject spawnerObj in obstacleSpawners)
            {
                if (spawnerObj != null)
                {
                    ObstacleSpawner script = spawnerObj.GetComponent<ObstacleSpawner>();
                    if (script != null) script.enabled = true;
                }
            }
        }
    }
}