using UnityEngine;
using UnityEngine.EventSystems;

public class DropTest : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
    }
}
