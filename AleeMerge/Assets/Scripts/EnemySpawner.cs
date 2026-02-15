using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Refs")]
    public DistanceTracker tracker;
    public DifficultyProfile profile;
    public BoxCollider spawnArea; // 3D BoxCollider
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;

    [Header("Spawn Control")]
    public int maxAlive = 15;
    public float intervalRandomJitter = 0.2f; // 让间隔有点随机，不死板

    float timer;

    enum SpawnEdge { Left, Top, Bottom }

    public float spawnZ = 0f;          // 敌人生成的固定Z
    public float outsideOffset = 1f;   // 往边缘外推，确保不进镜头

    public BoxCollider leftZone;
    public BoxCollider topZone;
    public BoxCollider bottomZone;

    [Header("Zone weights")]
    public float wLeft = 0.6f;
    public float wTop = 0.2f;
    public float wBottom = 0.2f;

    [Header("Fix axis")]
    public float fixedY = 0f;   // 你希望敌人生成时Y固定到多少


    void Update()
    {
        if (!tracker || !profile || !spawnArea) return;

        // 简单上限控制（后续可改成更精确的活怪统计/对象池）
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxAlive) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        float d = tracker.DistanceMeters;

        // 1) 决定生成间隔
        float baseInterval = profile.GetSpawnInterval(d);
        float jitter = Random.Range(-intervalRandomJitter, intervalRandomJitter);
        timer = Mathf.Max(0.05f, baseInterval + jitter);

        // 2) 按权重选怪
        var (w1, w2, w3) = profile.GetWeights(d);
        GameObject prefab = WeightedPick(enemy1, enemy2, enemy3, w1, w2, w3);

        // 3) 在区域里取随机点生成
        // 只从左/上/下三边刷
        var zone = PickZone(leftZone, topZone, bottomZone, wLeft, wTop, wBottom);
        Vector3 pos = RandomPointInBox(zone);

        // 固定Y（你想要的逻辑）
        pos.y = fixedY;

        Instantiate(prefab, pos, Quaternion.identity);

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

    static Vector3 RandomPointOnEdges(BoxCollider box, float outsideOffset, SpawnEdge edge)
    {
        Vector3 c = box.center;
        Vector3 h = box.size * 0.5f; // half size (local)

        Vector3 local;
        switch (edge)
        {
            case SpawnEdge.Left:
                local = new Vector3(c.x - h.x - outsideOffset, Random.Range(c.y - h.y, c.y + h.y), c.z);
                break;
            case SpawnEdge.Top:
                local = new Vector3(Random.Range(c.x - h.x, c.x + h.x), c.y + h.y + outsideOffset, c.z);
                break;
            default: // Bottom
                local = new Vector3(Random.Range(c.x - h.x, c.x + h.x), c.y - h.y - outsideOffset, c.z);
                break;
        }

        return box.transform.TransformPoint(local);
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
