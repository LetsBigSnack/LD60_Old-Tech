using TMPro;
using UnityEngine;

public class UIMorseMachineController : MonoBehaviour
{
    [SerializeField] private TMP_Text display;

    public void SetDisplayText(string text)
    {
        display.text = text;
    }
    
}
