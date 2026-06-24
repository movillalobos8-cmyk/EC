using UnityEngine;
using UnityEngine.Video;

public class VideoCameraTrigger : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Panel del Video")]
    public GameObject videoPanel;

    [Header("Posición objetivo")]
    public float targetX = 985.8f;

    [Header("UI Final")]
    public GameObject gameOverPanel;

    [Header("Audio")]
    public GameMusicManager musicManager;

    private bool activado = false;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (videoPanel != null)
            videoPanel.SetActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.loopPointReached += VideoTerminado;
        }
    }

    void Update()
    {
        if (activado)
            return;

        if (Camera.main != null &&
            Camera.main.transform.position.x >= targetX)
        {
            activado = true;

            // Pausar el juego
            Time.timeScale = 0f;

            // Detener música
            if (musicManager != null)
            {
                musicManager.StopMusic();
            }

            // Mostrar panel del video
            if (videoPanel != null)
            {
                videoPanel.SetActive(true);
            }

            // Reproducir video
            if (videoPlayer != null)
            {
                videoPlayer.Play();
            }
        }
    }

    void VideoTerminado(VideoPlayer vp)
    {
        // Ocultar video
        if (videoPanel != null)
        {
            videoPanel.SetActive(false);
        }

        // Mantener el juego pausado
        Time.timeScale = 0f;

        // Mostrar Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= VideoTerminado;
        }
    }
}