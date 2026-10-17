using System;
using Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class PunchcardController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    private Camera mainCamera;
    private Vector3 dragOffset;
    private Collider2D cardCollider;
    
    [SerializeField] private PunchCardData  punchCardData;

    public PunchCardData Data => punchCardData;
    
    private void Awake()
    {
        mainCamera = Camera.main;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                eventData.position.x,
                eventData.position.y,
                -mainCamera.transform.position.z
            )
        );

        mousePosition.z = transform.position.z;

        transform.position = mousePosition;
        
        
    }
}
