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
    
    public void GenerateTransmission(int dayOfMonth)
    {
        if (dayOfMonth < 1 || dayOfMonth > 31)
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth));

        int destinationIndex = UnityEngine.Random.Range(0, Destinations.Length);
        int typeIndex = UnityEngine.Random.Range(0, MailTypes.Length);
        int handlingIndex = UnityEngine.Random.Range(0, HandlingOptions.Length);
        int referenceIndex = UnityEngine.Random.Range(0, 6);

        string identifier = new string(new[]
        {
            RandomLetter(), RandomLetter(), RandomLetter()
        });
        string destination = Destinations[destinationIndex];
        string mailType = MailTypes[typeIndex];
        string handling = HandlingOptions[handlingIndex];
        string reference = ((char)('A' + referenceIndex)).ToString();

        string plaintext =
            $"DESTINATION: {destination}\n" +
            $"TYPE: {mailType}\n" +
            $"HANDLING: {handling}\n" +
            $"REFERENCE: {reference}";

        int[] decryptionRows = TransmissionCodec.GetDecryptionRows(identifier, dayOfMonth);
        int[] dispatchRows =
        {
            destinationIndex + 1,
            typeIndex + 1,
            handlingIndex + 1,
            referenceIndex + 1
        };

        string encryptedText = TransmissionCodec.ShiftText(plaintext, decryptionRows, true);
        string morseMessage = TransmissionCodec.ToMorse(identifier + "\n" + encryptedText);

        currentTransmission = new TransmissionData(
            identifier, dayOfMonth, destination, mailType, handling, reference,
            plaintext, encryptedText, morseMessage,
            dispatchRows, PhoneNumbers[destinationIndex]);
        
    }

    public string DecodeWithCard(int[] rows)
    {
        if (currentTransmission == null)
            throw new InvalidOperationException("No active transmission.");
        return TransmissionCodec.ShiftText(currentTransmission.EncryptedText, rows, false);
    }

    public bool ValidateSubmission(int[] dispatchRows, string phoneNumber)
    {
        var t = currentTransmission;
        return t != null &&
               dispatchRows != null &&
               dispatchRows.SequenceEqual(t.ExpectedDispatchRows) &&
               phoneNumber == t.ExpectedPhoneNumber;
    }

    private static char RandomLetter() =>
        (char)('A' + UnityEngine.Random.Range(0, 26));
}
