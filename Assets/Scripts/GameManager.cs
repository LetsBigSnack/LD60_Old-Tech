using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int TotalTransmission
    {
        get => totalTransmission;
        set => totalTransmission = value;
    }

    public int TotalCorrectTransmission
    {
        get => totalCorrectTransmission;
        set => totalCorrectTransmission = value;
    }

    public int TotalIncorrectTransmission
    {
        get => totalIncorrectTransmission;
        set => totalIncorrectTransmission = value;
    }

    public int DayTransmission
    {
        get => dayTransmission;
        set => dayTransmission = value;
    }

    public int DayCorrectTransmission
    {
        get => dayCorrectTransmission;
        set => dayCorrectTransmission = value;
    }

    public int DayIncorrectTransmission
    {
        get => dayIncorrectTransmission;
        set => dayIncorrectTransmission = value;
    }

    public static GameManager Instance;

    [Header("Date / Time")]
    [SerializeField] private int startDay;
    [SerializeField] private int currentDay = 0;
    [SerializeField] private int maxDays = 5;
    [SerializeField] private float currenDayTime = 0;
    [SerializeField] private float timePerDay = 300;
    [SerializeField] private bool isDayRunning = false;
    
    public bool IsDayRunning => isDayRunning;
    
    [Header("Money")]
    [SerializeField] private int currentMoney = 0;
    [SerializeField] private int rewardMoney = 20;
    [SerializeField] private int penaltyMoney = -10;

    [Header("Story-Events")] 
    [SerializeField] private int moneyThresholdGoodEnding = 300;
    [SerializeField] private int moneyThresholdBadEnding = 100;
    
    [Header("UI")]
    [SerializeField] private UIPopUpController popUpController;
    [SerializeField] private UIEndScreenController endScreenController;
    
    [Header("Stats")]
    [SerializeField] private int totalTransmission = 0;
    [SerializeField] private int totalCorrectTransmission = 0;
    [SerializeField] private int totalIncorrectTransmission = 0;
    [SerializeField] private int dayTransmission = 0;
    [SerializeField] private int dayCorrectTransmission = 0;
    [SerializeField] private int dayIncorrectTransmission = 0;


    
    public event Action<int> OnDayChange;
    
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        currentDay = startDay-1;
        Instance = this;
    }
    public void StartDay()
    {
        
        totalTransmission += dayTransmission;
        totalCorrectTransmission += dayCorrectTransmission;
        totalIncorrectTransmission += dayIncorrectTransmission;
        dayTransmission = 0;
        dayCorrectTransmission = 0;
        dayIncorrectTransmission = 0;
        
        currentDay++;
        OnDayChange?.Invoke(currentDay);
        
        Debug.Log("Starting Day");
        currenDayTime = 0;
        
    }

    public void StartWorkDay()
    {
        if (isDayRunning)
        {
            return;
        }
        isDayRunning =  true;
        TransmissionManager.Instance.GenerateTransmission(currentDay);
    }
    
    public void SubmitTransmission()
    {
        dayTransmission++;
        PunchCardData dispatchDate = DispatchController.Instance.InsertedKeycard;
        string phoneNumber = string.Join("", PhoneController.Instance.DialedNumbers);

        if (TransmissionManager.Instance.ValidateSubmission(dispatchDate, phoneNumber))
        {
            currentMoney += rewardMoney;
            dayCorrectTransmission++;
        }
        else
        {
            currentMoney += penaltyMoney;
            dayIncorrectTransmission++;
        }
        
        TransmissionManager.Instance.GenerateTransmission(currentDay);
        
    }

    public void EndDay()
    {
        
        SoundManager.Instance.StopLoopSFX("Clock_Tick");
        SoundManager.Instance.PlaySFX("Clock_Alarm");
        
        isDayRunning =  false;
        if (currentDay - startDay + 1 >= maxDays)
        {
            EndGame();
            return;
        }
        
        popUpController.SetPopUp(
            $"<size=130%><b>END OF DAY {currentDay}</b></size>\n" +
            $"Transmissions Processed: {dayTransmission}\n" +
            $"Correct Transmissions: {dayCorrectTransmission}\n" +
            $"Incorrect Transmissions: {dayIncorrectTransmission}\n\n" +
            $"Daily Earnings: <b>{dayCorrectTransmission * rewardMoney + dayIncorrectTransmission * penaltyMoney:+#;-#;0}</b>\n" +
            $"Total Balance: <b>{currentMoney}</b>\n" +
            "Unfinished transmissions have been transferred to the night shift."
        );
    }
    
    public void EndGame()
    {
        totalTransmission += dayTransmission;
        totalCorrectTransmission += dayCorrectTransmission;
        totalIncorrectTransmission += dayIncorrectTransmission;

        if (currentMoney >= moneyThresholdGoodEnding)
            GoodEnding();
        else if (currentMoney < moneyThresholdBadEnding)
            BadEnding();
        else
            NeutralEnding();
    }
    
    private void GoodEnding()
    {
        endScreenController.DisplayEnd(
            "<size=125%><b>NOTICE OF PROMOTION</b></size>\n" +
            "Congratulations, Comrade!\n" +
            "Your exceptional accuracy and dedication\n" +
            "have earned you a promotion to " +
            "<b>Senior Communications Officer.</b>\n\n" +
            $"Transmissions Processed: {totalTransmission}\n" +
            $"Correct Transmissions: {totalCorrectTransmission}\n" +
            $"Final Balance: {currentMoney}\n\n"
        );
    }

    private void NeutralEnding()
    {
        endScreenController.DisplayEnd(
            "<size=125%><b>PERFORMANCE EVALUATION</b></size>\n" +
            "Comrade,\n" +
            "Your performance has been deemed satisfactory.\n" +
            "You are to retain your current position.\n\n" +
            $"Transmissions Processed: {totalTransmission}\n" +
            $"Correct Transmissions: {totalCorrectTransmission}\n" +
            $"Final Balance: {currentMoney}\n\n"
        );
    }

    private void BadEnding()
    {
        endScreenController.DisplayEnd(
            "<size=125%><b>NOTICE OF TERMINATION</b></size>\n" +
            "Comrade,\n" +
            "Your repeated errors have failed to meet\n" +
            "the Ministry's standards.\n" +
            "Your employment is terminated immediately.\n\n" +
            $"Transmissions Processed: {totalTransmission}\n" +
            $"Correct Transmissions: {totalCorrectTransmission}\n" +
            $"Final Balance: {currentMoney}\n\n"
        );
    }
    
    private void Update()
    {

        if (!isDayRunning)
        {
            return;
        }
        
        currenDayTime += Time.deltaTime;

        if (currenDayTime >= timePerDay)
        {
            EndDay();
        }
    }

    public void SetPassTime(bool b)
    {
        isDayRunning = b;
    }
    
    public string GetCurrentTime()
    {
        const float startHour = 8f;
        const float endHour = 17f;

        float progress = Mathf.Clamp01(currenDayTime / timePerDay);

        float totalMinutes = Mathf.Lerp(startHour * 60f, endHour * 60f, progress);

        int hours = Mathf.FloorToInt(totalMinutes / 60f);
        int minutes = Mathf.FloorToInt(totalMinutes % 60f);

        string period = hours >= 12 ? "PM" : "AM";
        int displayHour = hours % 12;

        if (displayHour == 0)
            displayHour = 12;

        return $"{displayHour:00}:{minutes:00} {period}";
    }


    public string GetRemainingTime()
    {
        return (currentDay - startDay+1) + "/" + maxDays;
    }
}
