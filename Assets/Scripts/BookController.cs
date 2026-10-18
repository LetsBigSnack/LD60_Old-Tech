using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.Events;


public class BookController : MonoBehaviour
{   
    
    public static BookController Instance;
    
    [SerializeField] private List<BookPage> bookPages;
    [SerializeField] private int currentPage = 0;
    [SerializeField] private BookPage displayedLeft;
    [SerializeField] private BookPage displayedRight;

    public event Action<BookPage,BookPage, bool, bool> OnPageChanged;
    public event Action<BookChapter, bool> OnBookmarkChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }


    public void NextPage()
    {
        if (currentPage +2 >= bookPages.Count)
        {
            return;
        }
        currentPage += 2;
        ChangeContent();
    }

    public void PreviousPage()
    {
        if (currentPage -2 < 0)
        {
            return;
        }
        currentPage -= 2;
        ChangeContent();
    }

    private void ChangeContent()
    {
        displayedLeft = bookPages.ElementAtOrDefault(currentPage);
        displayedRight = bookPages.ElementAtOrDefault(currentPage+1);
        
        OnPageChanged?.Invoke(displayedLeft, displayedRight, currentPage != 0, currentPage +2 < bookPages.Count);

        foreach (BookChapter chapter in Enum.GetValues(typeof(BookChapter)))
        {  
            BookPage neededBookPage = bookPages.FirstOrDefault(b => b.isChapterStart && b.chapter == chapter);
            if (neededBookPage == null)
            {
                throw new Exception("Not found chapter");
            }
        
            int index = bookPages.IndexOf(neededBookPage);
            OnBookmarkChanged?.Invoke(chapter, index < currentPage);
        }
        
    }

    public void JumpToChapter(BookChapter chapter)
    {
        BookPage neededBookPage = bookPages.FirstOrDefault(b => b.isChapterStart && b.chapter == chapter);
        if (neededBookPage == null)
        {
            throw new Exception("Not found chapter");
        }
        
        int index = bookPages.IndexOf(neededBookPage);
        currentPage = index % 2 == 0? index : index-1;
        ChangeContent();

    }

    public void Display()
    {
        ChangeContent();
    }
}
