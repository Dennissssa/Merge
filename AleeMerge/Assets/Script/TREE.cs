using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}
