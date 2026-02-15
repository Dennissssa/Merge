using UnityEngine;

public class DestroyBulletStoneOnCollision : MonoBehaviour
{
    [Tooltip("销毁前延迟（比如留给特效时间）")]
    public float destroyDelay = 0f;

    void OnCollisionEnter(Collision collision)
    {
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

        // 防止两个物体同时触发两次销毁
        if (GetInstanceID() > other.GetInstanceID()) return;

        Destroy(gameObject, destroyDelay);
        Destroy(other, destroyDelay);
    }
}
