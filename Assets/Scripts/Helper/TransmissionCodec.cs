using System;
using System.Collections.Generic;
using System.Text;

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

        private static readonly Dictionary<string, char> ReverseMorse = BuildReverseMorse();

        public static int LetterToRow(char letter)
        {
            char upper = char.ToUpperInvariant(letter);
            if (upper < 'A' || upper > 'Z')
                throw new ArgumentException("Identifier must contain A-Z letters.");
            return (upper - 'A') % 6 + 1;
        }

        public static int DayToRow(int dayOfMonth)
        {
            if (dayOfMonth < 1 || dayOfMonth > 31)
                throw new ArgumentOutOfRangeException(nameof(dayOfMonth));
            return (dayOfMonth - 1) % 6 + 1;
        }

        public static int[] GetDecryptionRows(string identifier, int dayOfMonth)
        {
            if (identifier == null || identifier.Length != 3)
                throw new ArgumentException("Identifier must contain exactly three letters.", nameof(identifier));

            return new[]
            {
                LetterToRow(identifier[0]),
                LetterToRow(identifier[1]),
                LetterToRow(identifier[2]),
                DayToRow(dayOfMonth)
            };
        }

        // A card's four rows repeat across letters only; spaces/newlines do not advance the index.
        public static string ShiftText(string text, IReadOnlyList<int> rows, bool encrypt)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (rows == null || rows.Count != 4)
                throw new ArgumentException("Exactly four punched rows are required.", nameof(rows));
            for (int i = 0; i < 4; i++)
                if (rows[i] < 1 || rows[i] > 6)
                    throw new ArgumentException("Every column must have a row from 1 to 6.", nameof(rows));

            var result = new StringBuilder(text.Length);
            int letterIndex = 0;
            foreach (char c in text)
            {
                char upper = char.ToUpperInvariant(c);
                if (upper >= 'A' && upper <= 'Z')
                {
                    int shift = rows[letterIndex % 4] * (encrypt ? 1 : -1);
                    int index = (upper - 'A' + shift + 26) % 26;
                    result.Append((char)('A' + index));
                    letterIndex++;
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        public static string ToMorse(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var lines = text.ToUpper()
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Split('\n');

            var encodedLines = new List<string>();

            foreach (string line in lines)
            {
                var tokens = new List<string>();

                foreach (char c in line)
                {
                    if (c >= 'A' && c <= 'Z')
                        tokens.Add(MorseLetters[c - 'A']);
                    else if (c >= '0' && c <= '9')
                        tokens.Add(MorseDigits[c - '0']);
                    else if (c == ' ' || c == '\t')
                    {
                        if (tokens.Count > 0 && tokens[^1] != "/")
                            tokens.Add("/");
                    }
                    else if (c == ':')
                        tokens.Add("---...");
                    else if (c == '-')
                        tokens.Add("-....-");
                    else
                        throw new ArgumentException(
                            $"Unsupported Morse character: {c}");
                }

                while (tokens.Count > 0 && tokens[^1] == "/")
                    tokens.RemoveAt(tokens.Count - 1);

                encodedLines.Add(string.Join(" ", tokens));
            }

            return string.Join("\n", encodedLines);
        }
        
        private static Dictionary<string, char> BuildReverseMorse()
        {
            var map = new Dictionary<string, char>();
            for (int i = 0; i < 26; i++) map.Add(MorseLetters[i], (char)('A' + i));
            for (int i = 0; i < 10; i++) map.Add(MorseDigits[i], (char)('0' + i));
            map.Add("---...", ':');
            map.Add("-....-", '-');
            return map;
        }
    }
}
