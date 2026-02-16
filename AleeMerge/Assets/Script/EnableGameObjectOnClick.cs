using UnityEngine;

public class EnableGameObjectOnClick : MonoBehaviour
{
    [Tooltip("点击后要启用的物体")]
    public GameObject targetObject;

    [Header("Debug")]
    public bool debugLog = true;

    public void EnableObject()
    {
        if (debugLog)
            Debug.Log("Button Clicked! EnableObject() 被调用了");

        if (targetObject != null)
        {
            targetObject.SetActive(true);

            if (debugLog)
                Debug.Log("目标物体已启用: " + targetObject.name);
        }
        else
        {
            Debug.LogWarning("No targetObject assigned!");
        }
    }
}
