using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyHitDestroy : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Drag Script File Here")]
    public MonoScript scriptToDetect;
#endif

    public float destroyDelay = 0.1f;

    bool isDead = false;
    System.Type detectedType;

    void Awake()
    {
#if UNITY_EDITOR
        if (scriptToDetect != null)
        {
            detectedType = scriptToDetect.GetClass();
        }
#endif
    }

    void OnTriggerEnter(Collider other)
    {
        Check(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Check(collision.gameObject);
    }

    void Check(GameObject other)
    {
        if (isDead) return;
        if (detectedType == null) return;

        if (other.GetComponent(detectedType) != null)
        {
            isDead = true;
            StartCoroutine(DestroyBoth(other));
        }
    }

    IEnumerator DestroyBoth(GameObject other)
    {
        yield return new WaitForSeconds(destroyDelay);

        if (other != null)
            Destroy(other);

        Destroy(gameObject);
    }
}
