using System;
using System.Collections.Generic;
using UnityEngine;

public class PhoneController : MonoBehaviour
{
    
    public static PhoneController Instance;
    
    public event Action<List<int>> NumberChanged;
    
    [SerializeField] private List<int> dialedNumbers = new List<int>();
    [SerializeField] private int maxDigits = 3;
    
    public List<int> DialedNumbers => dialedNumbers;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public void Start()
    {
        TransmissionManager.Instance.TransmissionUpdated += Reset;
        Reset();
    }

    private void OnDestroy()
    {
        TransmissionManager.Instance.TransmissionUpdated -= Reset;
    }


    public void DialNumber(int number)
    {
        if (dialedNumbers.Count >= maxDigits)
        {
            return;
        }
        dialedNumbers.Add(number);
        NumberChanged?.Invoke(dialedNumbers);
    }

    public void Reset()
    {
        dialedNumbers.Clear();
        NumberChanged?.Invoke(dialedNumbers);
    }

    public void SubmitTransmission()
    {
        GameManager.Instance.SubmitTransmission();
    }
    
}
