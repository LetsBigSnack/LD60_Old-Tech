using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public sealed class TransmissionData
    {
        [SerializeField] public string Identifier ;
        [SerializeField] public int DayOfMonth ;
        [SerializeField] public string Destination ;
        [SerializeField] public string MailType ;
        [SerializeField] public string Handling ;
        [SerializeField] public string Reference ;
        [SerializeField] public string Plaintext ;
        [SerializeField] public string EncryptedText ;
        [SerializeField] public string MorseMessage ;
        [SerializeField] public PunchCardData ExpectedDispatchCard ;
        [SerializeField] public string ExpectedPhoneNumber ;

        public TransmissionData(
            string identifier, int dayOfMonth, string destination,
            string mailType, string handling, string reference,
            string plaintext, string encryptedText, string morseMessage,
            PunchCardData dispatchCard,
            string phoneNumber)
        {
            Identifier = identifier;
            DayOfMonth = dayOfMonth;
            Destination = destination;
            MailType = mailType;
            Handling = handling;
            Reference = reference;
            Plaintext = plaintext;
            EncryptedText = encryptedText;
            MorseMessage = morseMessage;
            ExpectedDispatchCard = dispatchCard.Clone();
            ExpectedPhoneNumber = phoneNumber;
        }
    }
}