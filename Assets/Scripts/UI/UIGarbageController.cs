using UnityEngine;
using UnityEngine.EventSystems;

public class UIGarbageController : MonoBehaviour, IDropHandler
{
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        if (!eventData.pointerDrag.TryGetComponent(
                out UIPunchcardController card))
            return;
        
        Destroy(card.gameObject);
        
    }
}
