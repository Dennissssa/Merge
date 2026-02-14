using UnityEngine;

public class EnemyChaseAndAttack : MonoBehaviour
{
    [Header("Target")]
    public Transform target;          // 玩家 Transform
    public PlayerHealth playerHealth; // 玩家血量脚本（建议拖）

    [Header("Move (separate axes)")]
    public float speedX = 3f;         // 世界X方向速度
    public float speedZ = 3f;         // 世界Z方向速度
    public float stopDistance = 1.0f; // 贴脸停止距离，避免抖动

    [Header("Attack")]
    public float attackRange = 2.0f;
    public float attackFrequency = 1.0f; // 每秒攻击次数（1=1秒一次）
    public int damage = 10;

    [Header("Flash")]
    public FlashWhite selfFlash;      // 敌人自己闪白（可不填，会自动找）

    float nextAttackTime;

    void Awake()
    {
        if (selfFlash == null) selfFlash = GetComponentInChildren<FlashWhite>();

        // 如果没拖 playerHealth，尝试从 target 找
        if (playerHealth == null && target != null)
            playerHealth = target.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > attackRange)
        {
            MoveTowardsTarget();
        }
        else
        {
            // 可选：进入范围也可以贴到 stopDistance
            if (dist > stopDistance) MoveTowardsTarget();
            TryAttack();
        }

        FaceTargetXZ();
    }

    void MoveTowardsTarget()
    {
        Vector3 to = target.position - transform.position;
        to.y = 0f;
        if (to.sqrMagnitude < 0.0001f) return;

        Vector3 dir = to.normalized;

        // 分离X/Z速度（世界坐标）
        Vector3 velocity = new Vector3(
            dir.x * speedX,
            0f,
            dir.z * speedZ
        );

        transform.position += velocity * Time.deltaTime;
    }

    void FaceTargetXZ()
    {
        Vector3 to = target.position - transform.position;
        to.y = 0f;
        if (to.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(to),
            12f * Time.deltaTime
        );
    }

    void TryAttack()
    {
        float interval = (attackFrequency <= 0f) ? 999999f : (1f / attackFrequency);

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + interval;
            DoAttack();
        }
    }

    void DoAttack()
    {
        // 敌人攻击瞬间闪白
        if (selfFlash != null) selfFlash.Flash();

        // 玩家受击（玩家闪白在 PlayerHealth 里）
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
        else
        {
            // 兜底：现场找
            var ph = target.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
