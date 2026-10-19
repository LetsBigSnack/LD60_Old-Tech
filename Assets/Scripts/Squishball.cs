using UnityEngine;
using UnityEngine.EventSystems;

public class Squishball : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Animator animator;

    public void OnPointerDown(PointerEventData eventData)
    {
        animator.Play("hold");
        SoundManager.Instance.PlaySFX("Squish");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        animator.Play("release");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.Play("idle"); 
    }
}
