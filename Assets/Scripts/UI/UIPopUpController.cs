using TMPro;
using UnityEngine;

namespace UI
{
    public class UIPopUpController : MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private GameObject content;
        public void Procced()
        {
            SoundManager.Instance.PlaySFX("Take_Paper");
            content.SetActive(false);
            GameManager.Instance.StartDay();
        }

        public void SetPopUp(string message)
        {
            content.SetActive(true);
            text.text = message;
        }
    }
}
