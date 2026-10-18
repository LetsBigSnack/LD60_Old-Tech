using System;
using Data;
using UnityEngine;

public class MorseController : MonoBehaviour, IPunchcardReceiver
{
    [SerializeField] private UIMorseMachineController ui;
    
    [SerializeReference] private PunchCardData insertedKeycard;

    //TODO: rework and Split UI form Controller Controller
    private void Start()
    {
        TransmissionManager.Instance.TransmissionUpdated += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        TransmissionManager.Instance.TransmissionUpdated -= UpdateDisplay;
    }


    public bool InsertPunchcard(PunchCardData card)
    {
        if (card == null || insertedKeycard != null)
            return false;

        insertedKeycard = card;
        UpdateDisplay();

        return true;
    }

    public void RemovePunchcard()
    {
        insertedKeycard = null;
        UpdateDisplay();
    }
    
    private void UpdateDisplay()
    {
        if (insertedKeycard == null)
        {
            ui.SetDisplayText(TransmissionManager.Instance.CurrentTransmission.MorseMessage);
            return;
        }
        
        ui.SetDisplayText(DecodeWithCard(insertedKeycard));
    }

    private string DecodeWithCard(PunchCardData card)
    {
        return TransmissionManager.Instance.DecodeWithCard(card);
    }
}
