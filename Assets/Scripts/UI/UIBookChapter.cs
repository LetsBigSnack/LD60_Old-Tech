using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIChapterButtons : MonoBehaviour
    {
        [SerializeField]
        private BookChapter chapter;
        
        public BookChapter Chapter => chapter;
        
        private void OnEnable()
        {
            GetComponentInChildren<TextMeshProUGUI>().text = chapter.ToString();
            GetComponent<Button>().onClick.RemoveAllListeners();
            Debug.Log("Added ActionListener for "+ chapter);
            GetComponent<Button>().onClick.AddListener(() => BookController.Instance.JumpToChapter(chapter));
        }
    }
}