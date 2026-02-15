using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera targetCamera;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (targetCamera == null) return;

        transform.forward = targetCamera.transform.forward;
    }
}
