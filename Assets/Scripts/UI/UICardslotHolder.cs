using System;
using Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICardslotHolder : MonoBehaviour, IDropHandler
{
    
    [SerializeField] private MonoBehaviour receiver;

    private IPunchcardReceiver punchcardReceiver;

    private UIPunchcardController punchard;
    
    private void Awake()
    {
        punchcardReceiver = receiver as IPunchcardReceiver;

        if (punchcardReceiver == null)
            Debug.LogError("Receiver must implement IPunchcardReceiver.", this);
    }

    private void Start()
    {
        TransmissionManager.Instance.TransmissionUpdated += DeletePunchard;
    }

    private void OnDestroy()
    {
        TransmissionManager.Instance.TransmissionUpdated -= DeletePunchard;
    }

    private void DeletePunchard()
    {
        if (punchard != null)
        {
            RemovePunchcard();
            Destroy(punchard.gameObject);
        }
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

        punchard = card;
        punchard.CurrentSlot = this;
        RectTransform cardRect = punchard.GetComponent<RectTransform>();

        cardRect.SetParent(transform, false);
        cardRect.anchoredPosition = Vector2.zero;
    }

    public void RemovePunchcard()
    {
        punchcardReceiver.RemovePunchcard();
        punchard.transform.SetParent(transform.parent, true);
    }
    
}
