using TMPro;
using UnityEngine;

namespace UI
{
    public class UIEndScreenController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform envelope;
        [SerializeField] private GameObject letterTopClosed;
        [SerializeField] private GameObject letterTopOpen;
        [SerializeField] private GameObject letterBottom;
        [SerializeField] private RectTransform innerLetter;
        [SerializeField] private TextMeshProUGUI letter;
        [SerializeField] private GameObject content;
        
        [Header("Shake")]
        [SerializeField] private float shakeAmount = 3f;
        [SerializeField] private float shakeSpeed = 8f;
        [SerializeField] private float rotationAmount = 1f;

        [Header("Letter Bob")]
        [SerializeField] private float bobAmount = 8f;
        [SerializeField] private float bobSpeed = 2f;

        private enum LetterState
        {
            Closed,
            Opened,
            Finished
        }

        private LetterState state;

        private Vector2 envelopeStartPosition;
        private Quaternion envelopeStartRotation;
        private Vector2 letterStartPosition;

        
        public void DisplayEnd(string letterText)
        {
            content.SetActive(true);
            
            envelopeStartPosition = envelope.anchoredPosition;
            envelopeStartRotation = envelope.localRotation;
            letterStartPosition = innerLetter.anchoredPosition;
            
            
            letter.text = letterText;
            ResetLetter();
        }

        private void Update()
        {
            if (!content.activeSelf)
            {
                return;
            }
            switch (state)
            {
                case LetterState.Closed:
                    AnimateShake();
                    break;

                case LetterState.Opened:
                    AnimateLetterBob();
                    break;
            }
        }

        private void AnimateShake()
        {
            float time = Time.unscaledTime;

            envelope.anchoredPosition = envelopeStartPosition +
                                        new Vector2(
                                            Mathf.Sin(time * shakeSpeed) * shakeAmount,
                                            Mathf.Cos(time * shakeSpeed * 0.7f) * shakeAmount
                                        );

            envelope.localRotation = envelopeStartRotation *
                                     Quaternion.Euler(
                                         0,
                                         0,
                                         Mathf.Sin(time * shakeSpeed) * rotationAmount
                                     );
        }

        private void AnimateLetterBob()
        {
            float offset = Mathf.Sin(Time.unscaledTime * bobSpeed) * bobAmount;

            innerLetter.anchoredPosition =
                letterStartPosition + Vector2.up * offset;
        }

        public void OpenLetter()
        {
            if (state != LetterState.Closed)
                return;
            
            SoundManager.Instance.PlaySFX("Take_Paper");
            
            state = LetterState.Opened;

            envelope.anchoredPosition = envelopeStartPosition;
            envelope.localRotation = envelopeStartRotation;

            letterTopClosed.SetActive(false);
            letterTopOpen.SetActive(true);
            
            innerLetter.gameObject.SetActive(true);
            
            letterBottom.GetComponent<CanvasGroup>().interactable= false;
            letterBottom.GetComponent<CanvasGroup>().blocksRaycasts= false;
            
        }

        public void FinishLetter()
        {
            if (state != LetterState.Opened)
                return;
            
            SoundManager.Instance.PlaySFX("Take_Paper");
            
            state = LetterState.Finished;

            innerLetter.anchoredPosition = letterStartPosition;
            
            letterTopClosed.SetActive(false);
            letterTopOpen.SetActive(false);
            letterBottom.SetActive(false);
            
        }

        public void ResetLetter()
        {
            state = LetterState.Closed;

            envelope.anchoredPosition = envelopeStartPosition;
            envelope.localRotation = envelopeStartRotation;
            innerLetter.anchoredPosition = letterStartPosition;
            
            CanvasGroup group = letterBottom.GetComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            letterTopClosed.SetActive(true);
            letterTopOpen.SetActive(false);
            letterBottom.SetActive(true);
            innerLetter.gameObject.SetActive(false);

        }

        public void Retry()
        {
            SoundManager.Instance.PlaySFX("Take_Paper");
            SceneChangeManager.Instance.ChangeScene(Scenes.Testing_Scene);
        }

        public void MainMenu()
        {
            SoundManager.Instance.PlaySFX("Take_Paper");
            SceneChangeManager.Instance.ChangeScene(Scenes.TitleScreen);
        }
        
        public void Quit()
        {
            SceneChangeManager.Instance.ExitApplication();
        }
        
    }
}
