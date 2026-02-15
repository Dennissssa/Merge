using UnityEngine;

public class EnemyChaseAndAttack : MonoBehaviour
{
    [Header("Target")]
    public Transform target;          // ��� Transform
    public PlayerHealth playerHealth; // ���Ѫ���ű��������ϣ�

    [Header("Move (separate axes)")]
    public float speedX = 3f;         // ����X�����ٶ�
    public float speedZ = 3f;         // ����Z�����ٶ�
    public float stopDistance = 1.0f; // ����ֹͣ���룬���ⶶ��

    [Header("Attack")]
    public float attackRange = 2.0f;
    public float attackFrequency = 1.0f; // ÿ�빥��������1=1��һ�Σ�
    public int damage = 1;

    [Header("Flash")]
    public FlashWhite selfFlash;      // �����Լ����ף��ɲ�����Զ��ң�

    float nextAttackTime;

    void Awake()
    {
        if (selfFlash == null) selfFlash = GetComponentInChildren<FlashWhite>();

        // ���û�� playerHealth�����Դ� target ��
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
            // ��ѡ�����뷶ΧҲ�������� stopDistance
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

        // ����X/Z�ٶȣ��������꣩
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
        // ���˹���˲������
        if (selfFlash != null) selfFlash.Flash();

        // ����ܻ������������ PlayerHealth �
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
        else
        {
            // ���ף��ֳ���
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
