using UnityEngine;

public class LevelMusicManager : MonoBehaviour
{
    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip levelMusic;

    [Header("Config")]
    public bool playOnStart = true;
    public bool loop = true;

    void Awake()
    {
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        if (playOnStart)
        {
            PlayLevelMusic();
        }
    }

    public void PlayLevelMusic()
    {
        PlayMusic(levelMusic);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource == null || musicClip == null)
            return;

        musicSource.Stop();
        musicSource.clip = musicClip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.UnPause();
        }
    }
}
