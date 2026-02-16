using UnityEngine;
using TMPro;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHealth = 5;
    public int currentHealth = 5;

    [Header("Hit Flash")]
    public FlashWhite flash;

    [Header("Health Bar UI")]
    public HealthBarUI healthBarUI;

    [Header("Death Settings")]
    public GameObject deathObjectToEnable;
    public float deathDelay = 1f;

    [Header("Death Time UI (TMP)")]
    public TMP_Text deathTimeText;          // 拖入Canvas上的TMP Text
    public string deathTimePrefix = "Time: ";

    private bool isDead = false;

    // 记录死亡时的场景时间（秒）
    public float deathSceneTime { get; private set; } = -1f;

    public event Action<int, int> OnHealthChanged;

    void Start()
    {
        Time.timeScale = 1f;   // 防止上一局
        currentHealth = maxHealth;

        if (deathObjectToEnable != null)
            deathObjectToEnable.SetActive(false);

        if (deathTimeText != null)
            deathTimeText.text = "";

        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (flash != null)
            flash.Flash();

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            // ✅ 记录死亡时间
            deathSceneTime = Time.timeSinceLevelLoad;

            // ✅ 显示
            UpdateDeathTimeUI();

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
            healthBarUI.UpdateHealth(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player Dead");

        Invoke(nameof(EnableDeathObject), deathDelay);
    }

    void EnableDeathObject()
    {
        if (deathObjectToEnable != null)
            deathObjectToEnable.SetActive(true);

        // ✅ 暂停游戏
        Time.timeScale = 0f;
    }


    void UpdateDeathTimeUI()
    {
        if (deathTimeText == null) return;

        int seconds = Mathf.FloorToInt(deathSceneTime);
        deathTimeText.text = seconds + " m";
    }



    // 可读性更好的 mm:ss 格式
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
