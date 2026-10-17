using System;

namespace Data
{
    [System.Serializable]
    public class TransmissionData
    {
        public string Identifier;
        public int DayOfMonth;
        public string Destination;
        public string MailType;
        public string Handling;
        public string Reference;

        public string Plaintext;
        public string EncryptedText;
        public string MorseMessage;
        
        public int[] ExpectedDispatchRows;
        public string ExpectedPhoneNumber;

        public TransmissionData(string identifier, int dayOfMonth, string destination,
            string mailType, string handling, string reference, string plaintext,
            string encryptedText, string morseMessage,
            int[] dispatchRows, string phoneNumber)
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
            ExpectedDispatchRows = (int[])dispatchRows.Clone();
            ExpectedPhoneNumber = phoneNumber;
        }
    }
}
