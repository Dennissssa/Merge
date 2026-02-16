using UnityEngine;
using UnityEngine.EventSystems;

public class HideImageOnPointerClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }
}
