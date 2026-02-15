using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public Image[] healthImages;  // 5个血格图片数组，按顺序排列
    public Sprite fullHealthSprite;  // 满血时的图片
    public Sprite emptyHealthSprite;  // 空血时的图片（可选，如果为空则隐藏）

    [Header("Layout Settings")]
    public float spacing = 10f;  // 血格之间的间距（如果使用自动布局）

    private int maxHealth = 5;
    private int currentHealth = 5;

    void Start()
    {
        // 如果没有手动设置图片数组，尝试自动查找子对象
        if (healthImages == null || healthImages.Length == 0)
        {
            SetupHealthImages();
        }

        // 初始化显示
        UpdateHealth(currentHealth, maxHealth);
    }

    // 自动设置血格图片（从子对象获取）
    void SetupHealthImages()
    {
        healthImages = new Image[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject healthSlot = new GameObject($"HealthSlot_{i}");
            healthSlot.transform.SetParent(transform, false);
            
            RectTransform rectTransform = healthSlot.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(50, 50);  // 默认大小
            
            Image image = healthSlot.AddComponent<Image>();
            healthImages[i] = image;
            
            // 设置位置（水平排列）
            rectTransform.anchoredPosition = new Vector2(i * (50 + spacing), 0);
        }
    }

    // 更新血条显示
    public void UpdateHealth(int current, int max)
    {
        currentHealth = Mathf.Clamp(current, 0, max);
        maxHealth = max;

        if (healthImages == null || healthImages.Length < maxHealth)
        {
            Debug.LogWarning($"HealthBarUI: 血格图片数组未正确设置！需要{maxHealth}个，但只有{healthImages?.Length ?? 0}个");
            return;
        }

        // 更新每个血格的显示
        for (int i = 0; i < maxHealth; i++)
        {
            if (healthImages[i] != null)
            {
                // 如果当前血量大于这个格子的索引，显示满血
                if (i < currentHealth)
                {
                    // 显示满血
                    if (fullHealthSprite != null)
                    {
                        healthImages[i].sprite = fullHealthSprite;
                        healthImages[i].enabled = true;
                    }
                    else
                    {
                        healthImages[i].enabled = true;
                        healthImages[i].color = Color.white;  // 如果没有sprite，用红色表示满血
                    }
                }
                else
                {
                    // 显示空血格
                    if (emptyHealthSprite != null)
                    {
                        healthImages[i].sprite = emptyHealthSprite;
                        healthImages[i].enabled = true;
                        healthImages[i].color = Color.white;
                    }
                    else
                    {
                        // 如果没有空血sprite，可以设置为半透明或隐藏
                        healthImages[i].enabled = false;
                    }
                }
            }
        }
    }

    // 设置血格图片数组（供外部调用）
    public void SetHealthImages(Image[] images)
    {
        healthImages = images;
        if (healthImages != null && healthImages.Length > 0)
        {
            maxHealth = healthImages.Length;
        }
    }
}

