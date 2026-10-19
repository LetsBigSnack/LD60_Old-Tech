using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPhoneController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialedNumber1;
    [SerializeField] private TextMeshProUGUI dialedNumber2;
    [SerializeField] private TextMeshProUGUI dialedNumber3;
    [SerializeField] private Button dialButton;


    void Start()
    {
        PhoneController.Instance.NumberChanged += UpdateDisplay;
        dialButton.interactable = false;
    }

    private void OnDestroy()
    {
        PhoneController.Instance.NumberChanged -= UpdateDisplay;
    }


    private void UpdateDisplay(List<int> numbers)
    {
        dialButton.interactable = numbers.Count >= 3;
        
        dialedNumber1.text = numbers.Count > 0 ? numbers[0].ToString() : "";
        dialedNumber2.text = numbers.Count > 1 ? numbers[1].ToString() : "";
        dialedNumber3.text = numbers.Count > 2 ? numbers[2].ToString() : "";
    }
    
    public void ClearNumbers()
    {
        SoundManager.Instance.PlaySFX("Clear");
        PhoneController.Instance.Reset();
    }

    public void SubmitTransmission()
    {
        PhoneController.Instance.SubmitTransmission();
    }
    
}
