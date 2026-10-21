using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GusanoBoss : MonoBehaviour
{
    public enum FaseBoss { Fase1, Fase2, Fase3 }

    [Header("--- AUDIO MANAGER ---")]
    public GameMusicManager musicManager;

    [Header("--- PANEL DE INICIO / CONTROLES ---")]
    public GameObject panelControlesInicio;

    [Header("--- REFERENCIAS ---")]
    public Transform jugador;
    public Transform puntoDisparo;
    public GameObject prefabProyectil;

    [Header("--- EXPRESIONES (CAMBIO MOMENTÁNEO DE IMAGEN) ---")]
    [Tooltip("SpriteRenderer que muestra al gusano. Si lo dejas vacío, busca el primero en este objeto o en sus hijos.")]
    public SpriteRenderer spriteRendererGusano;
    [Tooltip("Imagen del gusano con los ojos cerrados (parpadeo antes de disparar)")]
    public Sprite spriteOjosCerrados;
    [Tooltip("Imagen del gusano enojado (al embestir)")]
    public Sprite spriteEnojado;
    [Tooltip("Cuántas veces parpadea antes de disparar")]
    public int cantidadParpadeos = 2;
    [Tooltip("Segundos con los ojos cerrados en cada parpadeo")]
    public float duracionOjosCerrados = 0.18f;
    [Tooltip("Segundos con los ojos abiertos entre parpadeos")]
    public float pausaEntreParpadeos = 0.12f;
    [Tooltip("Segundos ANTES de embestir en los que ya se pone enojado")]
    public float tiempoAvisoEnojo = 1f;
    [Tooltip("0 = enojado hasta que termina la embestida. Mayor que 0 = se calma esos segundos después de que empieza a embestir.")]
    public float duracionEnojo = 0f;

    [Header("--- HITBOX EXTRA EN EL SUELO ---")]
    public HitboxSueloBoss hitboxExtraEmbestida;

    [Header("--- POSICIONES EN EL PISO (EMPTIES) ---")]
    public Transform pisoIzquierdo; 
    public Transform pisoDerecho;   

    [Header("--- SALUD Y BARRA DE VIDA (180 DISPAROS TOTALES) ---")]
    public int disparosPorBarra = 20; 
    public int totalBarras = 9;       
    private int impactosTotalesMaximos;
    public int impactosTotalesActuales; 

    public Image imagenBarraVidaUI;
    [Tooltip("Arrastra los 10 sprites: Element 0 = Vacía, Element 9 = Llena")]
    public Sprite[] spritesBarraVida;
    public GameObject canvasVictoria;
    public GameObject canvasDerrota;

    [Header("--- INICIO DEL COMBATE ---")]
    [Tooltip("Segundos que espera desde que el jugador se mueve hasta que el jefe empieza a atacar (antes era 3)")]
    public float retrasoInicioCombate = 2f;
    [Tooltip("Segundos hasta el PRIMER disparo una vez que empieza el combate (después sigue el cooldown normal de la fase)")]
    public float primerDisparoTrasInicio = 1.5f;

    [Header("--- FASES DEL JEFE ---")]
    public FaseBoss faseActual = FaseBoss.Fase1;

    [Header("--- ROTACIÓN EN EL PISO ---")]
    public float anguloHorizontal = 90f;
    public float tiempoEnPiso = 0.8f;

    [Header("--- FASE 1 (180 a 121 Disparos) ---")]
    public float cdEmbestidaFase1 = 7f;
    public float cdDisparoFase1 = 3f;
    public float velEmbestidaFase1 = 12f;
    public float velProyectilFase1 = 12f;

    [Header("--- FASE 2 (120 a 61 Disparos) ---")]
    public float cdEmbestidaFase2 = 5f;
    public float cdDisparoFase2 = 2f;
    public float velEmbestidaFase2 = 18f;
    public float velProyectilFase2 = 18f;

    [Header("--- FASE 3 (60 a 0 Disparos) ---")]
    public float cdEmbestidaFase3 = 3f;
    public float cdRafagaFase3 = 1.5f;
    public float velEmbestidaFase3 = 26f;
    public float velProyectilFase3 = 24f;
    
    public Vector2[] direccionesFase3; 

    private bool haciendoAccion = false;
    private float timerEmbestida;
    private float timerDisparo;

    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;

    // --- VARIABLES DE INICIO ---
    private bool esperandoJugador = false;
    private bool secuenciaIniciada = false;
    private Vector3 posicionInicialJugador;

    // --- ESTADO DE MUERTE ---
    private bool muerto = false;

    // --- EXPRESIONES TEMPORALES ---
    private bool parpadeoActivo = false;
    private bool enojoActivo = false;
    private bool avisoDisparoMostrado = false;

    private float DuracionTotalParpadeo
    {
        get { return cantidadParpadeos * (duracionOjosCerrados + pausaEntreParpadeos); }
    }

    private void Awake()
    {
        impactosTotalesMaximos = totalBarras * disparosPorBarra;
        impactosTotalesActuales = impactosTotalesMaximos;
    }

    private void Start()
    {
        impactosTotalesActuales = impactosTotalesMaximos;
        faseActual = FaseBoss.Fase1;

        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;

        if (canvasVictoria != null) canvasVictoria.SetActive(false);
        if (canvasDerrota != null) canvasDerrota.SetActive(false);

        if (musicManager == null)
        {
            musicManager = FindObjectOfType<GameMusicManager>();
        }

        if (spriteRendererGusano == null)
        {
            spriteRendererGusano = GetComponentInChildren<SpriteRenderer>();
        }

        if (hitboxExtraEmbestida != null) hitboxExtraEmbestida.DesactivarHitbox();

        if (jugador == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            jugador = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (direccionesFase3 == null || direccionesFase3.Length == 0)
        {
            direccionesFase3 = new Vector2[]
            {
                new Vector2(-1, 0),
                new Vector2(-0.707f, 0.707f),
                new Vector2(0, 1),
                new Vector2(0.707f, 0.707f),
                new Vector2(1, 0),
                new Vector2(0, -1)
            };
        }

        ActualizarBarraUI();
        ResetearTimers();

        PausarYMostrarControles();
    }

    private void PausarYMostrarControles()
    {
        if (panelControlesInicio != null)
        {
            panelControlesInicio.SetActive(true);
            Time.timeScale = 0f;

            if (musicManager != null)
            {
                musicManager.StopMusic();
            }
        }
        else
        {
            IniciarCombateDirecto();
        }
    }

    public void BotonEstoyListo()
    {
        if (panelControlesInicio != null)
        {
            panelControlesInicio.SetActive(false);
        }

        Time.timeScale = 1f;

        if (musicManager != null)
        {
            musicManager.PlayBackgroundMusic();
        }

        if (jugador != null)
        {
            posicionInicialJugador = jugador.position;
        }

        esperandoJugador = true;
    }

    private void IniciarCombateDirecto()
    {
        Time.timeScale = 1f;

        if (musicManager != null)
        {
            musicManager.PlayBackgroundMusic();
        }

        if (jugador != null) posicionInicialJugador = jugador.position;
        esperandoJugador = true;
    }

    private void Update()
    {
        if (muerto) return;

        if (esperandoJugador)
        {
            if (jugador != null)
            {
                if (Vector3.Distance(jugador.position, posicionInicialJugador) > 0.05f)
                {
                    esperandoJugador = false;
                    StartCoroutine(RutinaIniciarCombate());
                }
            }
            return; 
        }

        if (!secuenciaIniciada || haciendoAccion) return;

        timerEmbestida -= Time.deltaTime;
        timerDisparo -= Time.deltaTime;

        float cdEmbestida = cdEmbestidaFase1;
        float cdDisparo = cdDisparoFase1;
        float velEmbestida = velEmbestidaFase1;
        float velProyectil = velProyectilFase1;

        if (faseActual == FaseBoss.Fase2)
        {
            cdEmbestida = cdEmbestidaFase2;
            cdDisparo = cdDisparoFase2;
            velEmbestida = velEmbestidaFase2;
            velProyectil = velProyectilFase2;
        }
        else if (faseActual == FaseBoss.Fase3)
        {
            cdEmbestida = cdEmbestidaFase3;
            cdDisparo = cdRafagaFase3;
            velEmbestida = velEmbestidaFase3;
            velProyectil = velProyectilFase3;
        }

        // AVISO: parpadea justo antes de disparar (termina cuando sale el disparo, sin retrasarlo)
        if (!avisoDisparoMostrado && timerDisparo > 0f && timerDisparo <= DuracionTotalParpadeo)
        {
            avisoDisparoMostrado = true;
            StartCoroutine(RutinaParpadeo());
        }

        // AVISO: se enoja 1 segundo antes de embestir
        if (!enojoActivo && timerEmbestida > 0f && timerEmbestida <= tiempoAvisoEnojo)
        {
            enojoActivo = true;
        }

        if (timerEmbestida <= 0)
        {
            StartCoroutine(RutinaEmbestidaPiso(velEmbestida));
            timerEmbestida = cdEmbestida;
        }
        else if (timerDisparo <= 0)
        {
            if (faseActual == FaseBoss.Fase3)
            {
                DispararRafagaFase3(velProyectil);
            }
            else
            {
                DispararProyectilDirigido(velProyectil);
            }
            timerDisparo = cdDisparo;
            avisoDisparoMostrado = false;
        }
    }

    // Se ejecuta DESPUÉS de que el Animator actualiza el sprite, así el cambio de imagen
    // es momentáneo y la animación normal sigue corriendo por debajo.
    private void LateUpdate()
    {
        if (muerto || spriteRendererGusano == null) return;

        if (enojoActivo && spriteEnojado != null)
        {
            spriteRendererGusano.sprite = spriteEnojado;
        }
        else if (parpadeoActivo && spriteOjosCerrados != null)
        {
            spriteRendererGusano.sprite = spriteOjosCerrados;
        }
    }

    private IEnumerator RutinaParpadeo()
    {
        for (int i = 0; i < cantidadParpadeos; i++)
        {
            parpadeoActivo = true;
            yield return new WaitForSeconds(duracionOjosCerrados);

            parpadeoActivo = false;
            yield return new WaitForSeconds(pausaEntreParpadeos);
        }

        parpadeoActivo = false;
    }

    private IEnumerator ApagarEnojoTras(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        enojoActivo = false;
    }

    private IEnumerator RutinaIniciarCombate()
    {
        yield return new WaitForSeconds(retrasoInicioCombate);
        if (!muerto)
        {
            secuenciaIniciada = true;

            // El primer disparo sale antes que el cooldown normal, para que la fase 1 no empiece tan lenta
            timerDisparo = Mathf.Min(timerDisparo, primerDisparoTrasInicio);
        }
    }

    public void RecibirDano(float cantidad)
    {
        if (muerto || impactosTotalesActuales <= 0) return;

        impactosTotalesActuales -= (int)cantidad;
        if (impactosTotalesActuales < 0) impactosTotalesActuales = 0;

        ActualizarBarraUI();
        VerificarCambioDeFase();

        if (impactosTotalesActuales <= 0)
        {
            DerrotarJefe();
        }
    }

    private void VerificarCambioDeFase()
    {
        if (impactosTotalesActuales <= 60)
        {
            if (faseActual != FaseBoss.Fase3)
            {
                faseActual = FaseBoss.Fase3;
                Debug.Log("<color=red>¡JEFE CAMBIA A FASE 3!</color>");
                ResetearTimers();
            }
        }
        else if (impactosTotalesActuales <= 120)
        {
            if (faseActual != FaseBoss.Fase2)
            {
                faseActual = FaseBoss.Fase2;
                Debug.Log("<color=yellow>¡JEFE CAMBIA A FASE 2!</color>");
                ResetearTimers();
            }
        }
    }

    private void ActualizarBarraUI()
    {
        if (imagenBarraVidaUI != null && spritesBarraVida != null && spritesBarraVida.Length > 0)
        {
            int indiceSprite = Mathf.CeilToInt((float)impactosTotalesActuales / disparosPorBarra);
            indiceSprite = Mathf.Clamp(indiceSprite, 0, spritesBarraVida.Length - 1);
            
            imagenBarraVidaUI.sprite = spritesBarraVida[indiceSprite];
        }
    }

    private void ResetearTimers()
    {
        avisoDisparoMostrado = false;
        if (!haciendoAccion) enojoActivo = false;

        if (faseActual == FaseBoss.Fase1)
        {
            timerEmbestida = cdEmbestidaFase1;
            timerDisparo = cdDisparoFase1;
        }
        else if (faseActual == FaseBoss.Fase2)
        {
            timerEmbestida = cdEmbestidaFase2;
            timerDisparo = cdDisparoFase2;
        }
        else if (faseActual == FaseBoss.Fase3)
        {
            timerEmbestida = cdEmbestidaFase3;
            timerDisparo = cdRafagaFase3;
        }
    }

    private IEnumerator RutinaEmbestidaPiso(float velocidad)
    {
        haciendoAccion = true;
        avisoDisparoMostrado = false; // Si todavía debe disparar después, volverá a parpadear

        // SE ENOJA al embestir
        enojoActivo = true;
        if (duracionEnojo > 0f) StartCoroutine(ApagarEnojoTras(duracionEnojo));

        float espera = (faseActual == FaseBoss.Fase3) ? tiempoEnPiso * 0.3f : tiempoEnPiso;

        if (hitboxExtraEmbestida != null) hitboxExtraEmbestida.DesactivarHitbox();

        if (pisoIzquierdo != null)
        {
            Quaternion rotIzquierda = Quaternion.Euler(0, 0, -anguloHorizontal);
            yield return StartCoroutine(MoverYRotarFluido(pisoIzquierdo.position, rotIzquierda, velocidad));

            if (hitboxExtraEmbestida != null) hitboxExtraEmbestida.ActivarHitbox();
            yield return new WaitForSeconds(espera);
            if (hitboxExtraEmbestida != null) hitboxExtraEmbestida.DesactivarHitbox();

            yield return StartCoroutine(MoverYRotarFluido(posicionOriginal, rotacionOriginal, velocidad));
        }

        yield return new WaitForSeconds(0.1f);

        if (pisoDerecho != null)
        {
            Quaternion rotDerecha = Quaternion.Euler(0, 0, anguloHorizontal);
            yield return StartCoroutine(MoverYRotarFluido(pisoDerecho.position, rotDerecha, velocidad));

            if (hitboxExtraEmbestida != null) hitboxExtraEmbestida.ActivarHitbox();
            yield return new WaitForSeconds(espera);
            if (hitboxExtraEmbestida != null) hitboxExtraEmbestida.DesactivarHitbox();

            yield return StartCoroutine(MoverYRotarFluido(posicionOriginal, rotacionOriginal, velocidad));
        }

        // Termina la embestida: vuelve su animación normal
        if (duracionEnojo <= 0f) enojoActivo = false;

        haciendoAccion = false;
    }

    private IEnumerator MoverYRotarFluido(Vector3 destinoPos, Quaternion destinoRot, float velocidad)
    {
        Vector3 posInicial = transform.position;
        Quaternion rotInicial = transform.rotation;

        float distancia = Vector3.Distance(posInicial, destinoPos);
        float duracion = distancia / velocidad;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracion);
            float tSuave = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(posInicial, destinoPos, tSuave);
            transform.rotation = Quaternion.Slerp(rotInicial, destinoRot, tSuave);

            yield return null;
        }

        transform.position = destinoPos;
        transform.rotation = destinoRot;
    }

    private void DispararProyectilDirigido(float velocidad)
    {
        if (muerto || prefabProyectil == null || puntoDisparo == null || jugador == null) return;

        GameObject proyectilObj = Instantiate(prefabProyectil, puntoDisparo.position, Quaternion.identity);
        ProyectilBoss proyectil = proyectilObj.GetComponent<ProyectilBoss>();

        if (proyectil != null)
        {
            Vector2 direccion = (jugador.position - puntoDisparo.position).normalized;
            proyectil.Inicializar(direccion, velocidad, gameObject);
        }
    }

    private void DispararRafagaFase3(float velocidad)
    {
        if (muerto || prefabProyectil == null || puntoDisparo == null || direccionesFase3 == null) return;

        for (int i = 0; i < direccionesFase3.Length; i++)
        {
            if (direccionesFase3[i] == Vector2.zero) continue;

            GameObject proyectilObj = Instantiate(prefabProyectil, puntoDisparo.position, Quaternion.identity);
            ProyectilBoss proyectil = proyectilObj.GetComponent<ProyectilBoss>();

            if (proyectil != null)
            {
                proyectil.Inicializar(direccionesFase3[i].normalized, velocidad, gameObject);
            }
        }
    }

    // --- PANTALLA DE VICTORIA Y ORDEN ABSOLUTA DE DETENCIÓN ---
    private void DerrotarJefe()
    {
        if (muerto) return;
        muerto = true;

        Debug.Log("¡El Jefe ha sido derrotado!");

        // 1. Detiene de golpe cualquier movimiento o ráfaga activa
        StopAllCoroutines();
        parpadeoActivo = false;
        enojoActivo = false;

        // 2. Apaga la hitbox de daño al piso inmediatamente
        if (hitboxExtraEmbestida != null)
        {
            hitboxExtraEmbestida.DesactivarHitbox();
        }

        // 3. Activa Canvas de Victoria y reproduce sonido
        if (canvasVictoria != null) canvasVictoria.SetActive(true);

        if (musicManager != null)
        {
            musicManager.PlayWinSound();
        }

        // 4. Desactiva física y renderizado del jefe
        foreach (Collider2D c in GetComponentsInChildren<Collider2D>()) c.enabled = false;
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>()) r.enabled = false;

        // 5. Apaga este script para congelar el Update y evitar futuros ataques
        this.enabled = false;
    }

    // --- PANTALLA DE DERROTA ---
    public void JugadorDerrotado()
    {
        if (canvasDerrota != null) canvasDerrota.SetActive(true);

        if (musicManager != null)
        {
            musicManager.PlayLoseSound();
        }

        Time.timeScale = 0f;
    }
}