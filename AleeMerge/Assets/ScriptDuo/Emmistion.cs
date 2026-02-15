using System.Collections;
using UnityEngine;

public class FlashWhite : MonoBehaviour
{
    [Header("Flash Settings")]
    public Material flashMaterial;
    public float flashDuration = 0.06f;

    Renderer[] renderers;
    Material[][] originalMats;
    Coroutine running;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);

        originalMats = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMats[i] = renderers[i].materials; // ע�⣺����ʵ�����������飬runtime �滻��ȫ
        }
    }

    public void Flash()
    {
        if (flashMaterial == null || renderers == null || renderers.Length == 0) return;
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(DoFlash());
    }

    IEnumerator DoFlash()
    {
        // �滻Ϊ���ײ��ʣ����� slot ����һ�£�
        for (int i = 0; i < renderers.Length; i++)
        {
            var mats = renderers[i].materials;
            for (int m = 0; m < mats.Length; m++)
                mats[m] = flashMaterial;
            renderers[i].materials = mats;
        }

        yield return new WaitForSeconds(flashDuration);

        // ��ԭ
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMats[i];
        }

        running = null;
    }
}
