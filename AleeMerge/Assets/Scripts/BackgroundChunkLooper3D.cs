using System.Collections.Generic;
using UnityEngine;

public class BackgroundChunkLooper3D : MonoBehaviour
{
    [Header("Setup")]
    public Camera cam;
    public List<Transform> chunks;

    [Header("Motion")]
    public float scrollSpeed = 5f;
    public float recycleBuffer = 1f;
    public float seamOverlap = 0f;

    float chunkWidth;

    public DistanceTracker distanceTracker;

    void Awake()
    {
        if (!cam) cam = Camera.main;

        chunks.Sort((a, b) => a.position.x.CompareTo(b.position.x));

        var r = chunks[0].GetComponentInChildren<Renderer>();
        chunkWidth = r.bounds.size.x;
    }

    void Update()
    {
        float dx = scrollSpeed * Time.deltaTime;

        for (int i = 0; i < chunks.Count; i++)
            chunks[i].position += Vector3.left * dx;

        float depth = Vector3.Dot(chunks[0].position - cam.transform.position, cam.transform.forward);
        float camLeftX = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, depth)).x;

        Transform leftMost = chunks[0];
        Transform rightMost = chunks[chunks.Count - 1];

        var leftR = leftMost.GetComponentInChildren<Renderer>();
        var rightR = rightMost.GetComponentInChildren<Renderer>();

        if (leftR.bounds.max.x < camLeftX - recycleBuffer)
        {
            float shift = (rightR.bounds.max.x - leftR.bounds.min.x) - seamOverlap;
            leftMost.position += Vector3.right * shift;

            chunks.RemoveAt(0);
            chunks.Add(leftMost);
        }

        if (distanceTracker) distanceTracker.metersPerSecond = scrollSpeed;
    }
}
