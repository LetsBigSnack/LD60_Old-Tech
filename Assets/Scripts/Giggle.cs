using UnityEngine;
using UnityEngine.EventSystems;

public class Giggle : MonoBehaviour, IPointerEnterHandler
{


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag)
        {
            return;
        }
        SoundManager.Instance.PlaySFX("Giggle");
    }
}
