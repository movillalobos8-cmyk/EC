using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    public int health = 6;

    [Header("Inmunidad")]
    public float tiempoInmunidad = 5f;
    public float velocidadParpadeo = 0.1f;

    public Sprite spriteNormal;
    public Sprite spriteBlanco;

    private bool esInvulnerable = false;
    private bool dead = false;

    private SpriteRenderer sr;
    private GameManager gameManager;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        gameManager = FindFirstObjectByType<GameManager>();

        UpdateHearts();
    }

    public void TakeDamage()
    {
        if (esInvulnerable || dead) return;

        health--;

        UpdateHearts();

        if (health <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(Inmunidad());
    }

    void Die()
    {
        dead = true;

        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }

    IEnumerator Inmunidad()
    {
        esInvulnerable = true;

        float tiempo = 0f;

        while (tiempo < tiempoInmunidad)
        {
            sr.sprite = spriteBlanco;
            yield return new WaitForSeconds(velocidadParpadeo);

            sr.sprite = spriteNormal;
            yield return new WaitForSeconds(velocidadParpadeo);

            tiempo += velocidadParpadeo * 2f;
        }

        sr.sprite = spriteNormal;
        esInvulnerable = false;
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            int heartValue = health - (i * 2);

            if (heartValue >= 2)
                hearts[i].sprite = fullHeart;
            else if (heartValue == 1)
                hearts[i].sprite = halfHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}