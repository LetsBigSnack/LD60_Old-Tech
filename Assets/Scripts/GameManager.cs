using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance;

    [Header("Date / Time")]
    [SerializeField] private int currentDay = 0;
    [SerializeField] private int maxDays = 5;
    [SerializeField] private float timePerDay;
    
    
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
        Debug.Log("Starting Day");
        currentDay++;
        TransmissionManager.Instance.GenerateTransmission(currentDay);

    }

    public void EndDay()
    {
        
    }

    private void GenerateTransmissions()
    {
        
    }
    
    public void SubmitTransmission()
    {
        
    }

    public void EndGame()
    {
        
    }
    
    private void Update()
    {
        //TODO: Pass Time
    }
}
