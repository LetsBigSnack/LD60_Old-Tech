
using Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICardslotHolder : MonoBehaviour, IDropHandler
{
    [SerializeField] private MonoBehaviour receiver;

    private IPunchcardReceiver punchcardReceiver;
    private UIPunchcardController punchcard;

    private void Awake()
    {
        punchcardReceiver = receiver as IPunchcardReceiver;

        if (punchcardReceiver == null)
            Debug.LogError(
                "Receiver must implement IPunchcardReceiver.",
                this
            );
    }

    private void Start()
    {
        TransmissionManager.Instance.TransmissionUpdated +=
            DeletePunchcard;
    }

    private void OnDestroy()
    {
        if (TransmissionManager.Instance != null)
        {
            TransmissionManager.Instance.TransmissionUpdated -=
                DeletePunchcard;
        }
    }

    private void DeletePunchcard()
    {
        if (punchcard == null)
            return;

        UIPunchcardController card = punchcard;

        RemovePunchcard();
        Destroy(card.gameObject);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null ||
            punchcardReceiver == null)
            return;

        if (!eventData.pointerDrag.TryGetComponent(
                out UIPunchcardController card))
            return;

        if (punchcard != null)
            return;

        if (!punchcardReceiver.InsertPunchcard(card.PunchCardData))
            return;

        punchcard = card;
        card.CurrentSlot = this;

        RectTransform cardRect =
            card.GetComponent<RectTransform>();
        
        cardRect.SetParent(transform, false);

        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = Vector2.zero;
        
        if (card.TryGetComponent(out UIZoneResizer resizer))
            resizer.UpdateVersion();
    }

    public void RemovePunchcard()
    {
        if (punchcard == null)
            return;

        punchcardReceiver.RemovePunchcard();

        UIPunchcardController card = punchcard;

        punchcard = null;
        card.CurrentSlot = null;

        RectTransform cardRect =
            card.GetComponent<RectTransform>();

        Canvas canvas =
            GetComponentInParent<Canvas>().rootCanvas;
        
        cardRect.SetParent(canvas.transform, true);

        if (card.TryGetComponent(out UIZoneResizer resizer))
            resizer.UpdateVersion();
    }
}
