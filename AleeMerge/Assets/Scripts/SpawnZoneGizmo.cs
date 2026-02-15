using UnityEngine;

[ExecuteAlways]
public class SpawnZoneGizmo : MonoBehaviour
{
    public Color fill = new Color(1, 0.6f, 0f, 0.15f);

    void OnDrawGizmos()
    {
        var bc = GetComponent<BoxCollider>();
        if (!bc) return;

        Gizmos.color = fill;
        var old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawCube(bc.center, bc.size);
        Gizmos.color = new Color(fill.r, fill.g, fill.b, 1f);
        Gizmos.DrawWireCube(bc.center, bc.size);

        Gizmos.matrix = old;
    }
}
