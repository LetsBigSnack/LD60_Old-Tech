using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    
    [Flags]
    public enum UIDraggableType
    {
        None = 0,
        PunchCard = 1 << 0,
        Booklet = 1 << 1,
        All = ~0
    }
    
    [RequireComponent(typeof(RectTransform))]
    public class UIDropZone : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private UIDraggableType acceptedTypes = UIDraggableType.All;

        public bool Accepts(UIDraggable draggable)
        {
            if (draggable == null)
                return false;

            return (acceptedTypes & draggable.DraggableType) != 0;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (acceptedTypes == UIDraggableType.All)
            {
                SoundManager.Instance.PlaySFX("Knock");
            }
        }
    }
}