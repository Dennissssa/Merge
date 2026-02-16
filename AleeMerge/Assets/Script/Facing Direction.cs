using UnityEngine;

/// <summary>
/// 让物体始终朝向自己的移动方向（根据每帧位移计算）。
/// 适合子弹/飞行物：没有输入，只要在移动，就会自动转向。
/// </summary>
[DisallowMultipleComponent]
public class BulletFaceMoveDirection : MonoBehaviour
{
    [Header("Optional")]
    public Rigidbody rb;                 // 如果有 Rigidbody，拖进来更准
    public bool useRigidbodyVelocity = true;

    [Header("Rotation")]
    public Vector3 forwardAxis = Vector3.forward; // 子弹模型“前方”轴（一般是Z+）
    public float minSpeed = 0.001f;               // 低于这个速度就不转（避免抖动）
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    Vector3 _lastPos;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        _lastPos = transform.position;
    }

    void LateUpdate()
    {
        Vector3 moveDir = GetMoveDirection();
        if (moveDir.sqrMagnitude < minSpeed * minSpeed) return;

        // 生成目标旋转：让 forwardAxis 指向 moveDir
        // 默认假设模型 forwardAxis=Vector3.forward (Z+)
        Quaternion targetRot = Quaternion.LookRotation(moveDir.normalized, Vector3.up);

        // 如果你的模型前方不是Z+，做一个轴校正
        // 让“模型的 forwardAxis”对齐到 Unity 的 Vector3.forward
        Quaternion axisFix = Quaternion.FromToRotation(forwardAxis.normalized, Vector3.forward);
        targetRot *= Quaternion.Inverse(axisFix);

        Vector3 e = targetRot.eulerAngles;

        // 可选锁轴
        if (lockX) e.x = transform.eulerAngles.x;
        if (lockY) e.y = transform.eulerAngles.y;
        if (lockZ) e.z = transform.eulerAngles.z;

        transform.rotation = Quaternion.Euler(e);
    }

    Vector3 GetMoveDirection()
    {
        if (useRigidbodyVelocity && rb != null)
        {
            return rb.velocity; // Unity 6 用 linearVelocity；旧版本用 rb.velocity
        }

        Vector3 currentPos = transform.position;
        Vector3 delta = currentPos - _lastPos;
        _lastPos = currentPos;

        // delta / Time.deltaTime 其实就是速度方向，这里只要方向
        return delta;
    }
}
