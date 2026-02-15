using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Refs")]
    public DistanceTracker tracker;
    public DifficultyProfile profile;

    [Header("Player Target (assign in Inspector)")]
    public Transform playerTarget;   // 拖你的玩家Prefab实例(场景里的Player)进来
    public PlayerHealth playerHealth; // 可选：也拖进来（更快）

    [Header("Enemy Prefabs")]
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;

    [Header("Spawn Control")]
    public float intervalRandomJitter = 0.2f;
    float timer;
    public BoxCollider leftZone;
    public BoxCollider topZone;
    public BoxCollider bottomZone;

    [Header("Zone weights")]
    public float wLeft = 0.6f;
    public float wTop = 0.2f;
    public float wBottom = 0.2f;

    [Header("Fix axis")]
    public float fixedY = 0f;

    void Awake()
    {
        // 如果你没手动拖 PlayerHealth，但拖了 playerTarget，这里自动找一次
        if (playerTarget != null && playerHealth == null)
            playerHealth = playerTarget.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (!tracker || !profile) return;

        // 没目标就不刷（避免刷出来全是呆子）
        if (playerTarget == null) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        float d = tracker.DistanceMeters;

        // 生成间隔
        float baseInterval = profile.GetSpawnInterval(d);
        float jitter = Random.Range(-intervalRandomJitter, intervalRandomJitter);
        timer = Mathf.Max(0.05f, baseInterval + jitter);

        // 刷怪权重
        var (w1, w2, w3) = profile.GetWeights(d);
        GameObject prefab = WeightedPick(enemy1, enemy2, enemy3, w1, w2, w3);

        // 区域随机生成
        var zone = PickZone(leftZone, topZone, bottomZone, wLeft, wTop, wBottom);
        Vector3 pos = RandomPointInBox(zone);
        pos.y = fixedY;

        // 生成
        GameObject enemyGO = Instantiate(prefab, pos, Quaternion.identity);

        // ✅ 关键：把目标塞给敌人脚本
        var chase = enemyGO.GetComponent<EnemyChaseAndAttack>();
        if (chase != null)
        {
            chase.target = playerTarget;

            // 可选：顺便把 PlayerHealth 塞进去，避免每次 GetComponent
            if (playerHealth != null)
                chase.playerHealth = playerHealth;
        }
    }

    static Vector3 RandomPointInBox(BoxCollider box)
    {
        Vector3 c = box.center;
        Vector3 h = box.size * 0.5f;

        Vector3 local = new Vector3(
            Random.Range(c.x - h.x, c.x + h.x),
            Random.Range(c.y - h.y, c.y + h.y),
            Random.Range(c.z - h.z, c.z + h.z)
        );

        return box.transform.TransformPoint(local);
    }

    static GameObject WeightedPick(GameObject a, GameObject b, GameObject c, float wa, float wb, float wc)
    {
        float sum = Mathf.Max(0f, wa) + Mathf.Max(0f, wb) + Mathf.Max(0f, wc);
        if (sum <= 0f) return a;

        float r = Random.value * sum;
        if (r < wa) return a;
        r -= wa;
        if (r < wb) return b;
        return c;
    }

    BoxCollider PickZone(BoxCollider left, BoxCollider top, BoxCollider bottom, float wl, float wt, float wb)
    {
        float sum = wl + wt + wb;
        float r = Random.value * sum;
        if (r < wl) return left;
        r -= wl;
        if (r < wt) return top;
        return bottom;
    }
}
