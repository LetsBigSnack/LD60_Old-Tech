using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Helper
{
    public static class TransmissionCodec
    {
        private static readonly string[] MorseLetters =
        {
            ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---",
            "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-",
            "..-", "...-", ".--", "-..-", "-.--", "--.."
        };

        private static readonly string[] MorseDigits =
        {
            "-----", ".----", "..---", "...--", "....-",
            ".....", "-....", "--...", "---..", "----."
        };
        
        private static int LetterRow(char letter)
        {
            letter = char.ToUpperInvariant(letter);
            if (letter < 'A' || letter > 'Z')
                throw new ArgumentException("Identifier must contain A-Z.");
            return (letter - 'A') % PunchCardData.Rows;
        }

        public static PunchCardData GenerateDecryptionCard(string identifier, int dayOfMonth)
        {
            if (identifier == null || identifier.Length != 3)
                throw new ArgumentException("Identifier must contain three letters.", nameof(identifier));
            if (dayOfMonth < 1 || dayOfMonth > 31)
                throw new ArgumentOutOfRangeException(nameof(dayOfMonth));

            var card = new PunchCardData();
            for (int column = 0; column < 3; column++)
                card.Punch(column, LetterRow(identifier[column]));
            card.Punch(3, (dayOfMonth - 1) % PunchCardData.Rows);
            return card;
        }
        
        public static PunchCardData GenerateDispatchCard(
            int destinationIndex, int mailTypeIndex, int handlingIndex, int referenceIndex)
        {
            var card = new PunchCardData();
            PunchDispatchValue(card, 0, destinationIndex);
            PunchDispatchValue(card, 1, mailTypeIndex);
            PunchDispatchValue(card, 2, handlingIndex);
            PunchDispatchValue(card, 3, referenceIndex);
            return card;
        }

        private static void PunchDispatchValue(PunchCardData card, int column, int row)
        {
            if (row < 0 || row >= PunchCardData.Rows)
                throw new ArgumentOutOfRangeException(nameof(row), "Dispatch index must be 0-5.");

            card.Punch(column, row);
        }

        public static string ShiftText(string text, PunchCardData card, bool encrypt)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            uint cipherState = card.ToKey();
            var result = new StringBuilder(text.Length);

            foreach (char character in text)
            {
                if (!IsEnglishLetter(character))
                {
                    result.Append(character);
                    continue;
                }

                cipherState = GetNextCipherState(cipherState);
                int letterShift = (int)(cipherState % 26);

                if (!encrypt)
                    letterShift = -letterShift;

                result.Append(ShiftLetter(character, letterShift));
            }

            return result.ToString();
        }

        private static bool IsEnglishLetter(char character)
        {
            return (character >= 'A' && character <= 'Z') ||
                   (character >= 'a' && character <= 'z');
        }

        private static uint GetNextCipherState(uint currentState)
        {
            //https://www.numerical.recipes/forumarchive/index.php/t-172.html
            return unchecked(currentState * 1664525u + 1013904223u);
        }

        private static char ShiftLetter(char character, int shift)
        {
            char upperCase = char.ToUpperInvariant(character);
            int letterIndex = upperCase - 'A';
            int shiftedIndex = (letterIndex + shift + 26) % 26;
            return (char)('A' + shiftedIndex);
        }
        
        public static string ToMorse(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            string normalized = text.ToUpperInvariant().Replace("\r\n", "\n").Replace('\r', '\n');
            string[] lines = normalized.Split('\n');
            var encodedLines = new List<string>(lines.Length);

            foreach (string line in lines)
            {
                var tokens = new List<string>();
                foreach (char c in line)
                {
                    if (c >= 'A' && c <= 'Z') tokens.Add(MorseLetters[c - 'A']);
                    else if (c >= '0' && c <= '9') tokens.Add(MorseDigits[c - '0']);
                    else if (c == ' ' || c == '\t')
                    {
                        if (tokens.Count > 0 && tokens[tokens.Count - 1] != "/") tokens.Add("/");
                    }
                    else if (c == ':') tokens.Add("---...");
                    else if (c == '-') tokens.Add("-....-");
                    else throw new ArgumentException($"Unsupported Morse character: {c}");
                }

                while (tokens.Count > 0 && tokens[tokens.Count - 1] == "/")
                    tokens.RemoveAt(tokens.Count - 1);
                encodedLines.Add(string.Join(" ", tokens));
            }

            return string.Join("\n", encodedLines);
        }
    }
}
