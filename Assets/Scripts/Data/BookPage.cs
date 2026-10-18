using System;
using UnityEngine;

namespace Data
{
    public enum BookChapter
    {
        Intro,
        MorseCode,
        DecypherKeys,
        Disptach,
        Telefon
    }

    [Serializable]
    public class BookPage
    {
        public TextAsset content;
        public bool isChapterStart;
        public BookChapter chapter;

        [TextArea(2, 10)] public string generatedText;

        public string Text => generatedText ?? (content != null ? content.text : "");
    }
}