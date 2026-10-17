using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Helper;
using UnityEngine;

public sealed class TransmissionManager : MonoBehaviour
{
    
    #region Dictionaries
    private static readonly string[] Destinations =
    {
        "CIVILIAN", "GOVERNMENT", "MILITARY", "INDUSTRY", "INTELLIGENCE", "INTERNATIONAL"
    };

    private static readonly string[] MailTypes =
    {
        "PERSONAL", "OFFICIAL", "CLASSIFIED", "COMMERCIAL", "DIPLOMATIC", "RESTRICTED"
    };

    private static readonly string[] HandlingOptions =
    {
        "DELIVER", "HOLD", "INSPECT", "RETURN", "FORWARD", "DESTROY"
    };

    private static readonly string[] PhoneNumbers =
    {
        "101", "204", "315", "426", "537", "648"
    };

    #endregion


    public static TransmissionManager Instance;
    
    [Header("Transmissions")]
    [SerializeField] private TransmissionData currentTransmission = null;
    [SerializeField] private List<TransmissionData> pastTransmissions;

    public TransmissionData CurrentTransmission
    {
        get => currentTransmission;
    }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        pastTransmissions =  new List<TransmissionData>();
    }
    
    public TransmissionData GenerateTransmission(int dayOfMonth)
    {
        if (dayOfMonth < 1 || dayOfMonth > 31)
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth));

        int destinationIndex = UnityEngine.Random.Range(0, Destinations.Length);
        int typeIndex = UnityEngine.Random.Range(0, MailTypes.Length);
        int handlingIndex = UnityEngine.Random.Range(0, HandlingOptions.Length);
        int referenceIndex = UnityEngine.Random.Range(0, 6);

        string identifier = new string(new[] { RandomLetter(), RandomLetter(), RandomLetter() });
        string destination = Destinations[destinationIndex];
        string mailType = MailTypes[typeIndex];
        string handling = HandlingOptions[handlingIndex];
        string reference = ((char)('A' + referenceIndex)).ToString();

        string plaintext =
            $"DESTINATION: {destination}\n" +
            $"TYPE: {mailType}\n" +
            $"HANDLING: {handling}\n" +
            $"REFERENCE: {reference}";

        PunchCardData decryptionCard = TransmissionCodec.GenerateDecryptionCard(identifier, dayOfMonth);
        PunchCardData dispatchCard = TransmissionCodec.GenerateDispatchCard(
            destinationIndex, typeIndex, handlingIndex, referenceIndex);

        string encryptedText = TransmissionCodec.ShiftText(plaintext, decryptionCard, true);
        // Identifier remains unencrypted, but is converted to Morse.
        string morseMessage = TransmissionCodec.ToMorse(identifier + "\n" + encryptedText);

        if (currentTransmission != null)
            pastTransmissions.Add(currentTransmission);

        currentTransmission = new TransmissionData(
            identifier, dayOfMonth, destination, mailType, handling, reference,
            plaintext, encryptedText, morseMessage, dispatchCard, PhoneNumbers[destinationIndex]);

        return currentTransmission;
    }

    public string DecodeWithCard(PunchCardData card)
    {
        if (currentTransmission == null)
            throw new InvalidOperationException("No active transmission.");
        return TransmissionCodec.ShiftText(currentTransmission.EncryptedText, card, false);
    }

    public bool ValidateSubmission(
        PunchCardData decryptionCard,
        PunchCardData dispatchCard,
        string phoneNumber)
    {
        if (currentTransmission == null || decryptionCard == null || dispatchCard == null)
            return false;

        
        bool dispatchCorrect = dispatchCard.Matches(currentTransmission.ExpectedDispatchCard);
        bool phoneCorrect = phoneNumber == currentTransmission.ExpectedPhoneNumber;

        return dispatchCorrect && phoneCorrect;
    }

    private static char RandomLetter() =>
        (char)('A' + UnityEngine.Random.Range(0, 26));
    
}
