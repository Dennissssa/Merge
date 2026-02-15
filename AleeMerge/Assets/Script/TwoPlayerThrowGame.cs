using UnityEngine;

/// <summary>
/// 3rd-person one-object two-player auto attack (STAGGERED):
/// - Player A (WASD): moves a GROUND crosshair (world icon). ROCK lands on it (projectile).
/// - Player B (Arrow keys): rotates a HORIZONTAL aim around the player. BULLET is a flying prefab (Rigidbody).
/// - ROCK and BULLET fire on the same base interval, but BULLET is phase-offset (staggered).
///   Example: rock at 0,1,2... ; bullet at 0.5,1.5,2.5...
/// </summary>
public class DualAutoAttack_ThirdPerson_GroundCrosshair_BulletPrefab : MonoBehaviour
{
    [Header("References")]
    public Transform aimBase;               // player root/body
    public Transform throwOrigin;           // rock spawn point
    public Transform shootOrigin;           // bullet spawn point
    public Transform groundCrosshair;       // world ground reticle
    public Camera cam;                      // optional

    [Header("Ground Crosshair Move (WASD)")]
    public float crosshairMoveSpeed = 6f;   // units/sec on ground plane
    public float maxDistanceFromPlayer = 18f;
    public bool clampToRadius = true;

    [Header("Ground Placement")]
    public LayerMask groundLayers;          // Ground layer(s)
    public float groundRaycastHeight = 50f;
    public float groundStickOffset = 0.02f;

    [Header("Throw Rock (to ground crosshair)")]
    public GameObject rockPrefab;           // needs Rigidbody
    public float rockExtraHeight = 8f;      // higher = throws higher
    public float rockSpin = 10f;

    [Header("Aim (Arrow keys) - HORIZONTAL ONLY")]
    public float aimYawSpeed = 120f;        // deg/sec

    [Header("Bullet Prefab (Rigidbody)")]
    public GameObject bulletPrefab;         // needs Rigidbody
    public float bulletSpeed = 60f;
    public float bulletLifeTime = 3f;

    [Header("Staggered Fire Timing")]
    [Tooltip("Base interval for BOTH rock and bullet (seconds). Example: 1 = every 1 second.")]
    public float baseInterval = 1f;

    [Tooltip("Bullet fires after this offset from the rock beat. Example: 0.5 means bullet fires midway.")]
    public float bulletPhaseOffset = 0.5f;

    [Tooltip("If true, both start immediately according to their schedules; if false, they wait until their first scheduled time.")]
    public bool startAlignedImmediately = true;

    [Header("Optional Hitscan (instant hit check)")]
    public bool doHitscanRaycast = true;
    public float hitscanRange = 120f;
    public LayerMask hitLayers;
    public bool ignoreTriggers = true;

    [Header("Debug")]
    public bool debugDraw = true;
    public float debugRayLength = 20f;

    // internal
    private float _yaw;

    // schedule
    private float _nextRockTime;
    private float _nextBulletTime;

    void Awake()
    {
        if (!aimBase) aimBase = transform;
        if (!cam) cam = Camera.main;

        if (!groundCrosshair)
        {
            Transform t = transform.Find("GroundCrosshair");
            if (t) groundCrosshair = t;
        }
    }

    void Start()
    {
        baseInterval = Mathf.Max(0.01f, baseInterval);

        // 将 offset 规范到 [0, baseInterval)
        bulletPhaseOffset = Mathf.Repeat(bulletPhaseOffset, baseInterval);

        float t = Time.time;

        if (startAlignedImmediately)
        {
            // 立刻对齐：rock 现在就发一次；bullet 在 offset 后发
            _nextRockTime = t;
            _nextBulletTime = t + bulletPhaseOffset;
        }
        else
        {
            // 不立刻：rock 下一拍；bullet 下一拍+offset
            _nextRockTime = t + baseInterval;
            _nextBulletTime = t + baseInterval + bulletPhaseOffset;
        }
    }

    void Update()
    {
        if (!throwOrigin || !shootOrigin || !groundCrosshair) return;

        // Player A
        UpdateGroundCrosshairByWASD();

        // Player B
        UpdateAimByArrows();

        float now = Time.time;

        // 🪨 Rock fires on beat
        if (now >= _nextRockTime)
        {
            ThrowRockTo(groundCrosshair.position);
            _nextRockTime += baseInterval;

            // 防止掉帧时连续补太多发（可选保护）
            if (now - _nextRockTime > baseInterval * 3f)
                _nextRockTime = now + baseInterval;
        }

        // 🔫 Bullet fires on offset beat
        if (now >= _nextBulletTime)
        {
            Vector3 dir = GetShootDirectionAroundPlayerHorizontal();
            ShootBulletPrefab(dir);

            if (doHitscanRaycast)
                Hitscan(dir);

            _nextBulletTime += baseInterval;

            if (now - _nextBulletTime > baseInterval * 3f)
                _nextBulletTime = now + baseInterval;
        }

        if (debugDraw)
        {
            Vector3 dir = GetShootDirectionAroundPlayerHorizontal();
            Debug.DrawRay(shootOrigin.position, dir * debugRayLength, Color.yellow);
            Debug.DrawLine(throwOrigin.position, groundCrosshair.position, Color.cyan);
        }
    }

    // -------------------- Ground Crosshair (WASD) --------------------
    void UpdateGroundCrosshairByWASD()
    {
        Vector3 forward = aimBase.forward; forward.y = 0f;
        forward = forward.sqrMagnitude > 0.0001f ? forward.normalized : Vector3.forward;

        Vector3 right = aimBase.right; right.y = 0f;
        right = right.sqrMagnitude > 0.0001f ? right.normalized : Vector3.right;

        float mx = 0f, mz = 0f;
        if (Input.GetKey(KeyCode.W)) mz += 1f;
        if (Input.GetKey(KeyCode.S)) mz -= 1f;
        if (Input.GetKey(KeyCode.D)) mx += 1f;
        if (Input.GetKey(KeyCode.A)) mx -= 1f;

        Vector3 move = (right * mx + forward * mz);
        if (move.sqrMagnitude > 1f) move.Normalize();

        Vector3 desired = groundCrosshair.position + move * crosshairMoveSpeed * Time.deltaTime;

        if (clampToRadius)
        {
            Vector3 center = aimBase.position;
            Vector3 flat = desired - center;
            flat.y = 0f;

            if (flat.magnitude > maxDistanceFromPlayer)
            {
                flat = flat.normalized * maxDistanceFromPlayer;
                desired = center + flat;
            }
        }

        desired = StickPointToGround(desired);
        groundCrosshair.position = desired;
    }

    Vector3 StickPointToGround(Vector3 point)
    {
        Vector3 origin = new Vector3(point.x, point.y + groundRaycastHeight, point.z);
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundRaycastHeight * 2f, groundLayers, QueryTriggerInteraction.Ignore))
            return hit.point + Vector3.up * groundStickOffset;

        return new Vector3(point.x, groundCrosshair.position.y, point.z);
    }

    // -------------------- Rock Throw --------------------
    void ThrowRockTo(Vector3 targetPoint)
    {
        if (!rockPrefab) return;

        GameObject rock = Instantiate(rockPrefab, throwOrigin.position, Quaternion.identity);
        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (!rb)
        {
            Debug.LogWarning("rockPrefab needs a Rigidbody.");
            Destroy(rock);
            return;
        }

        Vector3 v0 = CalculateBallisticVelocity_ByExtraHeight(throwOrigin.position, targetPoint, rockExtraHeight);
        rb.velocity = v0;

        if (rockSpin > 0f)
            rb.angularVelocity = Random.onUnitSphere * rockSpin;
    }

    Vector3 CalculateBallisticVelocity_ByExtraHeight(Vector3 origin, Vector3 target, float extraHeight)
    {
        Vector3 displacement = target - origin;
        Vector3 displacementXZ = new Vector3(displacement.x, 0f, displacement.z);

        float g = Mathf.Abs(Physics.gravity.y);
        extraHeight = Mathf.Max(0.01f, extraHeight);

        float vY = Mathf.Sqrt(2f * g * extraHeight);
        float tUp = vY / g;

        float peakY = origin.y + extraHeight;
        float downHeight = Mathf.Max(0.01f, peakY - target.y);
        float tDown = Mathf.Sqrt(2f * downHeight / g);

        float totalTime = tUp + tDown;

        Vector3 vXZ = displacementXZ / Mathf.Max(0.01f, totalTime);
        return vXZ + Vector3.up * vY;
    }

    // -------------------- Aim (Arrow keys) --------------------
    void UpdateAimByArrows()
    {
        float yawDelta = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) yawDelta -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) yawDelta += 1f;

        _yaw += yawDelta * aimYawSpeed * Time.deltaTime;
        _yaw = Mathf.Repeat(_yaw, 360f);
    }

    // HORIZONTAL ONLY direction
    Vector3 GetShootDirectionAroundPlayerHorizontal()
    {
        Vector3 up = aimBase ? aimBase.up : Vector3.up;

        Vector3 forward = aimBase ? aimBase.forward : Vector3.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
        forward.Normalize();

        Quaternion yawRot = Quaternion.AngleAxis(_yaw, up);
        Vector3 dir = yawRot * forward;

        dir.y = 0f;
        dir.Normalize();
        return dir;
    }

    // -------------------- Bullet Prefab --------------------
    void ShootBulletPrefab(Vector3 dir)
    {
        if (!bulletPrefab) return;

        GameObject bullet = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.LookRotation(dir, Vector3.up));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (!rb)
        {
            Debug.LogWarning("bulletPrefab needs a Rigidbody.");
            Destroy(bullet);
            return;
        }

        rb.velocity = dir * bulletSpeed;
        Destroy(bullet, bulletLifeTime);
    }

    // -------------------- Optional Hitscan --------------------
    void Hitscan(Vector3 dir)
    {
        QueryTriggerInteraction qti = ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide;

        if (Physics.Raycast(shootOrigin.position, dir, out RaycastHit hit, hitscanRange, hitLayers, qti))
        {
            // hit logic here
            if (debugDraw)
                Debug.DrawLine(shootOrigin.position, hit.point, Color.red, 0.2f);
        }
        else
        {
            if (debugDraw)
                Debug.DrawRay(shootOrigin.position, dir * hitscanRange, Color.green, 0.2f);
        }
    }
}
