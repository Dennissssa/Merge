using UnityEngine;

public class RotateSprite : MonoBehaviour
{
    [Tooltip("每秒旋转角度（正数顺时针，负数逆时针）")]
    public float rotationSpeed = 180f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
