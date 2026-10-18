using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIBookController : MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI leftPageText;
        [SerializeField] private TextMeshProUGUI rightPageText;
        [SerializeField] private GameObject previousButton;
        [SerializeField] private GameObject nextButton;
        
        [SerializeField] private List<UIChapterButtons> chaptersLeft;
        [SerializeField] private List<UIChapterButtons> chaptersRight;
        
        private void Start()
        {
            BookController.Instance.OnPageChanged += DisplayContent;
            BookController.Instance.OnBookmarkChanged += ChangeBoomarks;
            BookController.Instance.Display();
        }

        private void OnDestroy()
        {
            BookController.Instance.OnPageChanged -= DisplayContent;
            BookController.Instance.OnBookmarkChanged -= ChangeBoomarks;
        }

      
        private void DisplayContent(BookPage leftPage, BookPage rightPage, bool displayPrevious,  bool displayNext)
        {
            leftPageText.richText = true;
            rightPageText.richText = true;
            
            leftPageText.text = leftPage != null ? leftPage.Text : "";
            rightPageText.text = rightPage != null ? rightPage.Text : "";
            previousButton.SetActive(displayPrevious);
            nextButton.SetActive(displayNext);
        }
        
        public void NextPage()
        {
            BookController.Instance.NextPage();
        }

        public void PreviousPage()
        {
            BookController.Instance.PreviousPage();
        }

        
        private void ChangeBoomarks(BookChapter chapter, bool isLeft)
        {
            chaptersLeft.FirstOrDefault(item => item.Chapter == chapter)?.gameObject.SetActive(isLeft);
            chaptersRight.FirstOrDefault(item => item.Chapter == chapter)?.gameObject.SetActive(!isLeft);
        }

        
    }
}
