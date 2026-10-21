using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Music")]
    public AudioClip backgroundMusic;
    public AudioClip winSound;
    public AudioClip loseSound;

    [Header("Config")]
    public bool playOnStart = true;

    private bool musicaPausada = false;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        if (playOnStart)
        {
            PlayBackgroundMusic();
        }
    }

    public void PlayBackgroundMusic()
    {
        if (audioSource == null || backgroundMusic == null)
            return;

        musicaPausada = false;
        audioSource.Stop();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayWinSound()
    {
        PlayResultSound(winSound);
    }

    public void PlayLoseSound()
    {
        PlayResultSound(loseSound);
    }

    // Corta la música del todo (al volver a sonar empieza desde el principio)
    public void StopMusic()
    {
        musicaPausada = false;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Pausa la música sin perder el punto en el que iba
    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
            musicaPausada = true;
        }
    }

    // Continúa la música justo donde se pausó. Si no estaba sonando nada, arranca la música de fondo.
    public void ResumeMusic()
    {
        if (audioSource == null) return;

        if (musicaPausada)
        {
            audioSource.UnPause();
            musicaPausada = false;
        }
        else if (!audioSource.isPlaying)
        {
            PlayBackgroundMusic();
        }
    }

    void PlayResultSound(AudioClip resultSound)
    {
        if (audioSource == null || resultSound == null)
            return;

        musicaPausada = false;
        audioSource.Stop();
        audioSource.clip = resultSound;
        audioSource.loop = false;
        audioSource.Play();
    }
}