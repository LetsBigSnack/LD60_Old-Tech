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
        if(!SoundManager.Instance.IsLoopSFXPlaying("Clock_Tick"))
            SoundManager.Instance.StartLoopSFX("Clock_Tick");
        
        GameManager.Instance.StartWorkDay();
    }
}
