using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIDraggable : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField, Range(0f, 1f)]
        private float dragAlpha = 0.6f;

        public event Action DragStarted;

        public bool IsDragging { get; private set; }

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Canvas canvas;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponentInParent<Canvas>().rootCanvas;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
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
            IsDragging = false;

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
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