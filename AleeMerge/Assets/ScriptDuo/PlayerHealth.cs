using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHealth = 5;  // 最大5格血
    public int currentHealth = 5;  // 当前血量（格数）

    [Header("Hit Flash")]
    public FlashWhite flash;

    [Header("Health Bar UI")]
    public HealthBarUI healthBarUI;  // 血条UI引用

    [Header("Scene Settings")]
    public bool loadNextSceneOnDeath = true;
    public float deathDelay = 1f;   // 延迟多少秒再跳转（可做死亡动画）

    private bool isDead = false;

    // 血量变化事件
    public event Action<int, int> OnHealthChanged;  // (currentHealth, maxHealth)

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        // 每格血代表1点，所以直接减少格数
        currentHealth -= amount;

        if (flash != null)
            flash.Flash();

        // 确保血量不会低于0
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthBar();
    }

    // 恢复血量
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthBar();
    }

    // 更新血条UI
    void UpdateHealthBar()
    {
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealth(currentHealth, maxHealth);
        }

        // 触发事件
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
