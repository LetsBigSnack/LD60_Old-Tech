using System;

namespace Data
{
    
    [Serializable]
    public enum BookChapter
    {
        Intro,
        MorseCode,
        DecypherKeys,
        Disptach,
        Telefon
    }
    
    
    [System.Serializable]
    public class BookPage
    {
        public string content;
        public bool isChapterStart;
        public BookChapter chapter;
    }
}