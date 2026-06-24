using UnityEngine;
using UnityEngine.UI;

public class HeadHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 100;

    [Header("Daño por disparo")]
    public int damagePerHit = 5;

    [Header("Barras (como corazones)")]
    public Image[] bars;
    // 👉 3 imágenes por cabeza

    [Header("Estados visuales")]
    public Sprite[] barStates;
    // 👉 8 sprites (0 full → 7 destruida)

    [Header("Cabeza central")]
    public bool esCabezaCentral = false;

    [Header("Muerte")]
    public Sprite spriteMuerto;

    [Header("Fondo (solo centro)")]
    public SpriteRenderer spriteFondo;
    public Sprite spriteMuerteFondo;

    private int currentHealth;
    private bool muerta = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateBars();
    }

    public void TakeDamage(int damage)
    {
        if (muerta) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateBars();
    }

    void UpdateBars()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] == null) continue;

            // 🔥 calculo global de estado
            float percent = (float)currentHealth / maxHealth;

            int state = Mathf.RoundToInt((1f - percent) * 7f);
            state = Mathf.Clamp(state, 0, 7);

            bars[i].sprite = barStates[state];
        }
    }

    void Die()
    {
        muerta = true;

         if (esCabezaCentral)
    {
        // Cambiar fondo
        if (spriteFondo != null && spriteMuerteFondo != null)
        {
            spriteFondo.sprite = spriteMuerteFondo;
        }

        // Desaparecer la cabeza central
        if (TryGetComponent<SpriteRenderer>(out var sr))
        {
            sr.enabled = false; // o sr.sprite = null;
        }
    }
     else
    {
        // 🟦 CABEZAS LATERALES: solo cambiar sprite
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null && spriteMuerto != null)
        {
            sr.sprite = spriteMuerto;
        }
    }

        foreach (var s in GetComponents<MonoBehaviour>())
            if (s != this) s.enabled = false;

        foreach (var c in GetComponentsInChildren<Collider2D>())
            c.enabled = false;
    }

    public bool IsDead()
    {
        return muerta;
    }
}