using UnityEngine;

public class DestroySfxMarkerB : MonoBehaviour
{
    void OnDestroy()
    {
        if (DestroySfxManager.Instance != null)
            DestroySfxManager.Instance.PlayB();
    }
}
