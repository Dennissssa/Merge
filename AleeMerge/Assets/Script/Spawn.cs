using UnityEngine;

public class DestroySfxManager : MonoBehaviour
{
    public static DestroySfxManager Instance { get; private set; }

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Destroy SFX")]
    public AudioClip destroyClipA;
    public AudioClip destroyClipB;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayA()
    {
        if (audioSource != null && destroyClipA != null)
            audioSource.PlayOneShot(destroyClipA);
    }

    public void PlayB()
    {
        if (audioSource != null && destroyClipB != null)
            audioSource.PlayOneShot(destroyClipB);
    }
}
