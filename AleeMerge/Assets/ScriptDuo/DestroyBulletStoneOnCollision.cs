using UnityEngine;

public class SpawnVFXThenDestroyOnBulletStone : MonoBehaviour
{
    [Header("VFX Prefab (Sprite Animation)")]
    [Tooltip("一个Prefab：SpriteRenderer + Animator（动画设为不循环）")]
    public GameObject vfxPrefab;

    [Tooltip("如果 Animator 读不到长度，就用这个作为兜底秒数")]
    public float fallbackVfxDuration = 0.5f;

    [Tooltip("VFX 相对碰撞点的高度偏移（避免插进地里）")]
    public Vector3 vfxOffset = new Vector3(0f, 0.1f, 0f);

    [Header("Destroy Timing")]
    [Tooltip("额外延迟：在 VFX 播完之后再等多少秒才销毁（可为0）")]
    public float extraDestroyDelay = 0f;

    [Tooltip("销毁自己/对方时是否同时销毁他们的 Rigidbody（通常不用管）")]
    public bool destroyBothObjects = true;

    private bool _handled = false;

    void OnCollisionEnter(Collision collision)
    {
        if (_handled) return;

        GameObject other = collision.gameObject;

        bool iAmBullet = GetComponent<Bullet>() != null;
        bool iAmStone = GetComponent<Stone>() != null;

        bool otherIsBullet = other.GetComponent<Bullet>() != null;
        bool otherIsStone = other.GetComponent<Stone>() != null;

        // 只处理 Bullet <-> Stone
        bool isBulletStone =
            (iAmBullet && otherIsStone) ||
            (iAmStone && otherIsBullet);

        if (!isBulletStone) return;

        // 防止双方都触发：只允许一个实例处理
        if (GetInstanceID() > other.GetInstanceID()) return;

        _handled = true;

        // 取碰撞点（如果拿不到就用两个物体中点）
        Vector3 hitPoint = collision.contactCount > 0
            ? collision.GetContact(0).point
            : (transform.position + other.transform.position) * 0.5f;

        float vfxDuration = SpawnVfxAndGetDuration(hitPoint + vfxOffset);

        float destroyAfter = vfxDuration + Mathf.Max(0f, extraDestroyDelay);

        // 让物体在等待期间不再继续乱撞（可选但推荐）
        DisablePhysics(gameObject);
        DisablePhysics(other);

        if (destroyBothObjects)
        {
            Destroy(gameObject, destroyAfter);
            Destroy(other, destroyAfter);
        }
        else
        {
            Destroy(gameObject, destroyAfter);
        }
    }

    float SpawnVfxAndGetDuration(Vector3 pos)
    {
        if (!vfxPrefab) return fallbackVfxDuration;

        GameObject vfx = Instantiate(vfxPrefab, pos, Quaternion.identity);

        // 尝试从 Animator 读取当前状态长度（可靠前提：Animator 默认就播放你那条clip）
        float duration = fallbackVfxDuration;
        Animator anim = vfx.GetComponent<Animator>();

        if (anim != null && anim.runtimeAnimatorController != null)
        {
            // 注意：刚 Instantiate 的这一帧 Animator 可能还没进入 state
            // 所以用一个比较稳的方式：从 controller 的第一个 clip 读长度
            AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
            if (clips != null && clips.Length > 0 && clips[0] != null)
            {
                duration = clips[0].length;
            }
        }

        Destroy(vfx, duration); // VFX 自己也会清理掉
        return duration;
    }

    void DisablePhysics(GameObject go)
    {
        // 防止等待期间继续碰撞触发别的逻辑
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Collider[] cols = go.GetComponentsInChildren<Collider>();
        for (int i = 0; i < cols.Length; i++)
            cols[i].enabled = false;
    }
}
