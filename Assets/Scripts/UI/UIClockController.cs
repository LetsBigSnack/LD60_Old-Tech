using TMPro;
using UnityEngine;

public class UIClockController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clockText;
    
    void Update()
    {
        clockText.text = GameManager.Instance.GetCurrentTime();
    }

    public void StartDay()
    {
        
        GameManager.Instance.StartWorkDay();
    }
}
