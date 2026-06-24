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

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void PlayResultSound(AudioClip resultSound)
    {
        if (audioSource == null || resultSound == null)
            return;

        audioSource.Stop();
        audioSource.clip = resultSound;
        audioSource.loop = false;
        audioSource.Play();
    }
}
