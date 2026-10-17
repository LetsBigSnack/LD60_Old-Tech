using System;
using Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPunchcardController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    
    [SerializeField]
    private PunchCardData punchCardData;
    
    public PunchCardData PunchCardData => punchCardData;

    private UICardslotHolder currentSlot = null;

    public UICardslotHolder CurrentSlot
    {
        get => currentSlot;
        set => currentSlot = value;
    }
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>().rootCanvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Card: OnBeginDrag");
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        currentSlot.RemovePunchcard(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Card: OnEndDrag");
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Card: OnDrag");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
