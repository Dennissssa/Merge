using UnityEngine;

public class EnableGameObjectOnClick : MonoBehaviour
{
    [Tooltip("点击后要启用的物体")]
    public GameObject targetObject;

    public void EnableObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No targetObject assigned!");
        }
    }
}
