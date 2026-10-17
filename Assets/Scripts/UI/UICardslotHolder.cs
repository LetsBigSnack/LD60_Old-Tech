using Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICardslotHolder : MonoBehaviour, IDropHandler
{
    
    [SerializeField] private MonoBehaviour receiver;

    private IPunchcardReceiver punchcardReceiver;

    private void Awake()
    {
        punchcardReceiver = receiver as IPunchcardReceiver;

        if (punchcardReceiver == null)
            Debug.LogError("Receiver must implement IPunchcardReceiver.", this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null ||
            punchcardReceiver == null)
            return;

        if (!eventData.pointerDrag.TryGetComponent(
                out UIPunchcardController card))
            return;
        
        if (!punchcardReceiver.InsertPunchcard(card.PunchCardData))
            return;
        
        card.CurrentSlot = this;
        
        RectTransform cardRect = card.GetComponent<RectTransform>();

        cardRect.SetParent(transform, false);
        cardRect.anchoredPosition = Vector2.zero;
    }

    public void RemovePunchcard(UIPunchcardController card)
    {
        punchcardReceiver.RemovePunchcard();
        card.transform.SetParent(transform.parent, true);
    }
    
}
