using System;
using Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPunchcardController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    
    [SerializeField]
    private PunchCardData punchCardData;
    
    public PunchCardData PunchCardData => punchCardData;

    private UICardslotHolder currentSlot = null;
    private Button[] _buttons;
    
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
        AssignedButtons();
    }

    private void AssignedButtons()
    {
        _buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < _buttons.Length; i++)
        {
            var i1 = i;
            _buttons[i].onClick.AddListener(delegate { PierceHole(i1); });
        }
    }

    private void PierceHole(int index)
    {
        punchCardData.Punch(index/6, index%6);
        _buttons[index].interactable = false;
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
