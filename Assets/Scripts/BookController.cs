using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class BookController : MonoBehaviour
{
    public static BookController Instance { get; private set; }

    [Header("Source files (one per original section)")]
    [SerializeField] private List<BookPage> sourcePages = new List<BookPage>();

    [Header("Automatic pagination")]
    [SerializeField] private string pageBreakMarker = "[PAGE]";

    [Header("Generated pages (read-only at runtime)")]
    [SerializeField] private List<BookPage> bookPages = new List<BookPage>();
    [SerializeField] private int currentPage;
    [SerializeField] private BookPage displayedLeft;
    [SerializeField] private BookPage displayedRight;

    public event Action<BookPage, BookPage, bool, bool> OnPageChanged;
    public event Action<BookChapter, bool> OnBookmarkChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        BuildPages();
    }
    
    [ContextMenu("Rebuild Pages")]
    public void BuildPages()
    {
        bookPages.Clear();

        foreach (BookPage source in sourcePages)
        {
            if (source == null || source.content == null)
                continue;

            string text = source.content.text
                .Replace("\r\n", "\n")
                .Replace('\r', '\n');

            string[] pages = text.Split(
                new[] { pageBreakMarker },
                StringSplitOptions.None
            );

            bool firstPage = true;

            foreach (string page in pages)
            {
                string pageText = page.Trim('\n', '\r');

                if (string.IsNullOrWhiteSpace(pageText))
                    continue;

                bookPages.Add(new BookPage
                {
                    chapter = source.chapter,
                    isChapterStart = source.isChapterStart && firstPage,
                    generatedText = pageText
                });

                firstPage = false;
            }
        }

        currentPage = 0;
    }

    public void NextPage()
    {
        if (currentPage + 2 >= bookPages.Count) return;
        currentPage += 2;
        ChangeContent();
    }

    public void PreviousPage()
    {
        if (currentPage < 2) return;
        currentPage -= 2;
        ChangeContent();
    }

    public void JumpToChapter(BookChapter chapter)
    {
        int index = bookPages.FindIndex(p => p.isChapterStart && p.chapter == chapter);
        if (index < 0)
        {
            Debug.LogWarning($"Chapter start not found: {chapter}", this);
            return;
        }
        currentPage = index - index % 2;
        ChangeContent();
    }

    public void Display() => ChangeContent();

    private void ChangeContent()
    {
        displayedLeft = bookPages.ElementAtOrDefault(currentPage);
        displayedRight = bookPages.ElementAtOrDefault(currentPage + 1);
        OnPageChanged?.Invoke(displayedLeft, displayedRight,
            currentPage > 0, currentPage + 2 < bookPages.Count);

        foreach (BookChapter chapter in Enum.GetValues(typeof(BookChapter)))
        {
            int start = bookPages.FindIndex(p => p.isChapterStart && p.chapter == chapter);
            if (start >= 0)
                OnBookmarkChanged?.Invoke(chapter, start < currentPage);
        }
    }
}
