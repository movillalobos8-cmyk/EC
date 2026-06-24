using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Sonidos")]
    public AudioClip topHeadShoot;
    public AudioClip bottomHeadShoot;
    public AudioClip playerShoot;
    public AudioClip playerHit;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayTopHeadShoot()
    {
        if (audioSource != null && topHeadShoot != null)
        {
            audioSource.PlayOneShot(topHeadShoot);
        }
    }

    public void PlayBottomHeadShoot()
    {
        if (audioSource != null && bottomHeadShoot != null)
        {
            audioSource.PlayOneShot(bottomHeadShoot);
        }
    }

    public void PlayPlayerShoot()
    {
        if (audioSource != null && playerShoot != null)
        {
            audioSource.PlayOneShot(playerShoot);
        }
    }

    public void PlayPlayerHit()
    {
        if (audioSource != null && playerHit != null)
        {
            audioSource.PlayOneShot(playerHit);
        }
    }
}