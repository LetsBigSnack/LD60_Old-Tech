using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICalendarController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI remainingDayText;
    
    void Start()
    {
        GameManager.Instance.OnDayChange += UpdateDate;
    }

    private void UpdateDate(int date)
    {
        dayText.text = date.ToString();

        remainingDayText.text = GameManager.Instance.GetRemainingTime();


    }

    private void OnDestroy()
    {
        GameManager.Instance.OnDayChange -= UpdateDate;
    }

}
