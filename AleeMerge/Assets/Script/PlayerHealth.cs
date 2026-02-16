using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHealth = 5;
    public int currentHealth = 5;

    [Header("Hit Flash")]
    public FlashWhite flash;

    [Header("Hit Sound")]
    public AudioSource audioSource;   // 拖入 AudioSource
    public AudioClip hitClip;         // 受击音效

    [Header("Health Bar UI")]
    public HealthBarUI healthBarUI;

    [Header("Scene Settings")]
    public bool loadNextSceneOnDeath = true;
    public float deathDelay = 1f;

    private bool isDead = false;

    // 血量变化事件
    public event Action<int, int> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // 播放闪白
        if (flash != null)
            flash.Flash();

        // 🔊 播放受击音效
        if (audioSource != null && hitClip != null)
            audioSource.PlayOneShot(hitClip);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthBar();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealth(currentHealth, maxHealth);
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player Dead");

        if (loadNextSceneOnDeath)
        {
            Invoke(nameof(LoadNextScene), deathDelay);
        }
    }

    void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }
}
