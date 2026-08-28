using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class ControladorDelJugador : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidadMovimiento = 8f;
    public float fuerzaSalto = 12f;
    public float fuerzaDobleSalto = 9f;

    [Header("Sonidos de Salto")]
    public AudioClip sonidoSalto;
    public AudioClip sonidoDobleSalto;
    [Range(0f, 1f)] public float volumenSalto = 1f;

    [Header("Sonidos de Caminata")]
    public AudioClip sonidoPasos;
    [Range(0f, 1f)] public float volumenPasos = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource; // Un solo AudioSource para todo

    private float entradaHorizontal;
    private bool mirandoDerecha = true;

    // Control de suelo y saltos
    public bool estaEnSuelo = false;
    private int saltosRealizados = 0;
    private const int MAX_SALTOS = 2;

    private void Awake()
    {
        // Obtenemos el único AudioSource del objeto
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

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            EjecutarSalto();
        }

        if (entradaHorizontal > 0 && !mirandoDerecha) Girar();
        else if (entradaHorizontal < 0 && mirandoDerecha) Girar();

        ManejarSonidoPasos();

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

    private void ManejarSonidoPasos()
    {
        bool estaCaminando = Mathf.Abs(entradaHorizontal) > 0.1f && estaEnSuelo && Mathf.Abs(rb.linearVelocity.y) < 0.1f;

        if (estaCaminando)
        {
            if (sonidoPasos != null)
            {
                // Si estaba sonando un salto, dejamos que termine antes de meter el loop de pasos
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = sonidoPasos;
                    audioSource.volume = volumenPasos;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
        }
        else
        {
            // CLAVE: Solo detiene el AudioSource SI lo que está sonando actualmente son los pasos.
            // Esto evita que detenga el sonido del salto o doble salto.
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
            // Si no asignaste un sonido de doble salto especifico, usa el de salto normal
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Piso"))
        {
            estaEnSuelo = true;
            saltosRealizados = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Piso"))
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
            // Cortamos cualquier loop activo (como los pasos) para lanzar el salto inmediatamente
            audioSource.Stop();
            audioSource.PlayOneShot(clip, volumenSalto);
        }
    }
}