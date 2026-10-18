using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance;

    [Header("Date / Time")]
    [SerializeField] private int currentDay = 0;
    [SerializeField] private int maxDays = 5;
    [SerializeField] private float currenDayTime = 0;
    [SerializeField] private float timePerDay = 300;
    [SerializeField] private bool isDayRunning = true;
    
    [Header("Money")]
    [SerializeField] private int currentMoney = 0;
    [SerializeField] private int rewardMoney = 20;
    [SerializeField] private int penaltyMoney = -10;

    [Header("Story-Events")] 
    [SerializeField] private int moneyThresholdGoodEnding = 300;
    [SerializeField] private int moneyThresholdBadEnding = 100;
    
 
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void Start()
    {
        StartDay();
    }
    
    private void StartDay()
    {
        
        currentDay++;

        if (currentDay > maxDays)
        {
            EndGame();
        }
        
        Debug.Log("Starting Day");
        currenDayTime = 0;
        isDayRunning =  true;
        TransmissionManager.Instance.GenerateTransmission(currentDay);

    }
    
    public void SubmitTransmission()
    {
        PunchCardData dispatchDate = DispatchController.Instance.InsertedKeycard;
        string phoneNumber = string.Join("", PhoneController.Instance.DialedNumbers);

        if (TransmissionManager.Instance.ValidateSubmission(dispatchDate, phoneNumber))
        {
            currentMoney += rewardMoney;
        }
        else
        {
            currentMoney -= penaltyMoney;
        }
        
        TransmissionManager.Instance.GenerateTransmission(currentDay);
        
    }

    public void EndGame()
    {
        
    }
    
    private void Update()
    {
        if (isDayRunning)
        {
            currenDayTime += Time.deltaTime;
        }

        if (currenDayTime >= timePerDay)
        {
            StartDay();
        }
    }
}
