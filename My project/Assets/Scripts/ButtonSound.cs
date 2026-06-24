using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Button")]
    public Button button;

    void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (audioSource == null)
        {
            audioSource = FindFirstObjectByType<AudioSource>();
        }
    }

    void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(PlayClickSound);
        }
    }

    public void PlayClickSound()
    {
        if (audioSource == null || clickSound == null)
            return;

        audioSource.PlayOneShot(clickSound);
    }
}
