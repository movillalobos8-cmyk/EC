using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class ControladorDelJugador : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidadMovimiento = 8f;
    public float fuerzaSalto = 12f;
    public float fuerzaDobleSalto = 9f;

    [Header("Disparo del Jugador")]
    public Transform puntoDisparoArriba;      
    public GameObject prefabProyectilJugador;  
    public float cadenciaDisparo = 0.25f; // Tiempo entre cada disparo continuo
    private float timerDisparo;

    [Header("Sonidos de Salto")]
    public AudioClip sonidoSalto;
    public AudioClip sonidoDobleSalto;
    [Range(0f, 1f)] public float volumenSalto = 1f;

    [Header("Sonidos de Caminata")]
    public AudioClip sonidoPasos;
    [Range(0f, 1f)] public float volumenPasos = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private float entradaHorizontal;
    private bool mirandoDerecha = true;

    public bool estaEnSuelo = false;
    private int saltosRealizados = 0;
    private const int MAX_SALTOS = 2;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        entradaHorizontal = Input.GetAxisRaw("Horizontal");

        // --- SALTO ---
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            EjecutarSalto();
        }

        // --- DISPARO CONTINUO (MANTENER TECLA E) ---
        timerDisparo -= Time.deltaTime;
        if (Input.GetKey(KeyCode.E) && timerDisparo <= 0f)
        {
            DispararHaciaArriba();
            timerDisparo = cadenciaDisparo;
        }

        // --- GIRO ---
        if (entradaHorizontal > 0 && !mirandoDerecha) Girar();
        else if (entradaHorizontal < 0 && mirandoDerecha) Girar();

        ManejarSonidoPasos();

        // --- ANIMATOR ---
        if (animator != null)
        {
            bool estaMoviendose = Mathf.Abs(entradaHorizontal) > 0.1f;
            animator.SetBool("estaCorriendo", estaMoviendose);
            animator.SetBool("estaEnSuelo", estaEnSuelo);
            animator.SetFloat("velocidadY", rb.linearVelocity.y);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(entradaHorizontal * velocidadMovimiento, rb.linearVelocity.y);
    }

    private void DispararHaciaArriba()
    {
        if (prefabProyectilJugador != null && puntoDisparoArriba != null)
        {
            Instantiate(prefabProyectilJugador, puntoDisparoArriba.position, Quaternion.identity);
        }

        if (animator != null)
        {
            bool estaMoviendose = Mathf.Abs(entradaHorizontal) > 0.1f;

            if (estaMoviendose)
            {
                animator.SetTrigger("disparoMove");
            }
            else
            {
                animator.SetTrigger("disparoIdle");
            }
        }
    }

    private void ManejarSonidoPasos()
    {
        bool estaCaminando = Mathf.Abs(entradaHorizontal) > 0.1f && estaEnSuelo && Mathf.Abs(rb.linearVelocity.y) < 0.1f;

        if (estaCaminando)
        {
            if (sonidoPasos != null && !audioSource.isPlaying)
            {
                audioSource.clip = sonidoPasos;
                audioSource.volume = volumenPasos;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying && audioSource.clip == sonidoPasos)
            {
                audioSource.Stop();
            }
        }
    }

    private void EjecutarSalto()
    {
        if (estaEnSuelo || saltosRealizados == 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            saltosRealizados = 1;
            estaEnSuelo = false;
            ReproducirSonidoSalto(sonidoSalto);
        }
        else if (saltosRealizados < MAX_SALTOS)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaDobleSalto);
            saltosRealizados = 2;
            AudioClip clipAJugar = (sonidoDobleSalto != null) ? sonidoDobleSalto : sonidoSalto;
            ReproducirSonidoSalto(clipAJugar);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Piso"))
        {
            estaEnSuelo = true;
            saltosRealizados = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Piso"))
        {
            estaEnSuelo = false;
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void ReproducirSonidoSalto(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(clip, volumenSalto);
        }
    }
}