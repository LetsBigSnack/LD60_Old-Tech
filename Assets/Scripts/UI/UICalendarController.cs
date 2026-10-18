using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICalendarController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    
    
    void Start()
    {
        GameManager.Instance.OnDayChange += UpdateDate;
    }

    private void UpdateDate(int date)
    {
        dayText.text = date.ToString();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnDayChange -= UpdateDate;
    }

}
