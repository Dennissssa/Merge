using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Tooltip("多少秒后自动销毁")]
    public float lifeTime = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
