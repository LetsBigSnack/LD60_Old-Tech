
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIDraggable : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private UIDraggableType draggableType = UIDraggableType.PunchCard;

        [SerializeField, Range(0f, 1f)]
        private float dragAlpha = 0.6f;

        public UIDraggableType DraggableType => draggableType;

        public event Action DragStarted;

        public bool IsDragging { get; private set; }

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Canvas canvas;

        private Vector2 originalPosition;
        private Transform originalParent;
        private float originalAlpha;
        private bool originalBlocksRaycasts;

        private readonly List<RaycastResult> raycastResults = new();
        
        public event Action DragEnded;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponentInParent<Canvas>().rootCanvas;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            originalPosition = rectTransform.anchoredPosition;
            originalParent = transform.parent;

            originalAlpha = canvasGroup.alpha;
            originalBlocksRaycasts = canvasGroup.blocksRaycasts;

            IsDragging = true;
            DragStarted?.Invoke();

            canvasGroup.alpha = dragAlpha;
            canvasGroup.blocksRaycasts = false;

            MoveToPointer(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveToPointer(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            bool valid = IsOverValidDropZone(eventData);

            if (!valid)
            {
                transform.SetParent(originalParent, false);
                rectTransform.anchoredPosition = originalPosition;
            }

            canvasGroup.alpha = originalAlpha;
            canvasGroup.blocksRaycasts = originalBlocksRaycasts;
            SoundManager.Instance.PlaySFX("Thup");
            IsDragging = false;
            DragEnded?.Invoke();
        }

        private bool IsOverValidDropZone(PointerEventData eventData)
        {
            raycastResults.Clear();

            EventSystem.current.RaycastAll(eventData, raycastResults);

            foreach (RaycastResult result in raycastResults)
            {
                UIDropZone zone =
                    result.gameObject.GetComponentInParent<UIDropZone>();

                if (zone != null &&
                    zone.isActiveAndEnabled &&
                    zone.Accepts(this))
                {
                    return true;
                }
            }

            return false;
        }

        private void MoveToPointer(PointerEventData eventData)
        {
            RectTransform parent =
                rectTransform.parent as RectTransform;

            if (parent == null)
                return;

            Camera camera = canvas.renderMode ==
                            RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parent,
                    eventData.position,
                    camera,
                    out Vector2 localPosition))
            {
                rectTransform.anchoredPosition = localPosition;
            }
        }
    }
}
