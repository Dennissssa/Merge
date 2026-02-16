using UnityEngine;

public class DestroySfxMarkerA : MonoBehaviour
{
    void OnDestroy()
    {
        if (DestroySfxManager.Instance != null)
            DestroySfxManager.Instance.PlayA();
    }
}
