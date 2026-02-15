using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 100;
    public int currentHP = 100;

    [Header("Hit Flash")]
    public FlashWhite flash; // 拖你玩家身上的 FlashWhite 组件进来

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (flash != null) flash.Flash();

        if (currentHP <= 0)
        {
            currentHP = 0;
            // TODO: 这里处理死亡
            Debug.Log("Player Dead");
        }
    }
}
