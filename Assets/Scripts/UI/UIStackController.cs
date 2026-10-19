using UnityEngine;
using UnityEngine.EventSystems;

public class UIStackController : MonoBehaviour, IDragHandler, IBeginDragHandler
{

    [SerializeField] private GameObject punchCardPrefab;
    
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject punchCard = Instantiate(punchCardPrefab, transform);
        eventData.pointerDrag = punchCard;
        SoundManager.Instance.PlaySFX("Take_Paper");
        ExecuteEvents.Execute(punchCard, eventData, ExecuteEvents.beginDragHandler);
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }
    
}
