
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class RotaryPhoneDial : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Numbers")]
    [SerializeField] private TMP_FontAsset numberFont;
    [SerializeField] private float numberRadius = 100f;
    [SerializeField] private float numberSize = 32f;
    [SerializeField] private float holeRadius = 22f;
    [SerializeField] private float firstNumberAngle = 35f;
    [SerializeField] private float numberSpacing = 30f;
    [SerializeField] private Color numberColor;
    
    
    [Header("Finger Stop")]
    [SerializeField] private float stopAngle = -25f;

    [Header("Return")]
    [SerializeField] private float returnSpeed = 300f;

    private RectTransform dial;
    private RectTransform numbersContainer;

    private Quaternion restingRotation;
    private float currentRotation;
    private float maximumRotation;
    private float previousMouseAngle;

    private int selectedNumber = -1;
    private bool dragging;
    private bool returning;

    private void Awake()
    {
        dial = GetComponent<RectTransform>();
        restingRotation = dial.localRotation;

        CreateNumbers();
    }

    private void CreateNumbers()
    {
        if (transform.parent == null)
            return;

        Transform parent = transform.parent;
        Transform existing = parent.Find("DialNumbers");

        GameObject containerObject;

        if (existing != null)
        {
            containerObject = existing.gameObject;
        }
        else
        {
            containerObject = new GameObject(
                "DialNumbers",
                typeof(RectTransform)
            );

            containerObject.transform.SetParent(parent, false);
        }

        numbersContainer = containerObject.GetComponent<RectTransform>();

        numbersContainer.anchorMin = dial.anchorMin;
        numbersContainer.anchorMax = dial.anchorMax;
        numbersContainer.pivot = dial.pivot;
        numbersContainer.sizeDelta = dial.sizeDelta;
        numbersContainer.anchoredPosition = dial.anchoredPosition;

        // Keep the numbers above the rotating disk.
        numbersContainer.SetAsLastSibling();

        for (int i = 1; i <= 10; i++)
        {
            int number = i % 10;
            string objectName = $"Number_{number}";

            Transform numberExisting = numbersContainer.Find(objectName);
            GameObject numberObject;

            if (numberExisting != null)
            {
                numberObject = numberExisting.gameObject;
            }
            else
            {
                numberObject = new GameObject(
                    objectName,
                    typeof(RectTransform),
                    typeof(TextMeshProUGUI)
                );

                numberObject.transform.SetParent(
                    numbersContainer,
                    false
                );
            }

            RectTransform rect =
                numberObject.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(50f, 50f);

            float angle = GetNumberAngle(number) * Mathf.Deg2Rad;

            rect.anchoredPosition = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * numberRadius;

            TextMeshProUGUI text =
                numberObject.GetComponent<TextMeshProUGUI>();

            text.text = number.ToString();

            if (numberFont != null)
                text.font = numberFont;

            text.fontSize = numberSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = numberColor;
            text.raycastTarget = false;
        }
    }

    private float GetNumberAngle(int number)
    {
        int index = number == 0 ? 9 : number - 1;
        return firstNumberAngle + index * numberSpacing;
    }

    private Vector2 GetLocalMousePosition(
        Vector2 screenPosition,
        Camera eventCamera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dial,
            screenPosition,
            eventCamera,
            out Vector2 localPosition
        );

        return localPosition;
    }

    private int FindNumber(PointerEventData eventData)
    {
        Vector2 mouse = GetLocalMousePosition(
            eventData.pressPosition,
            eventData.pressEventCamera
        );

        for (int i = 1; i <= 10; i++)
        {
            int number = i % 10;

            float angle =
                GetNumberAngle(number) * Mathf.Deg2Rad;

            Vector2 position = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * numberRadius;

            if (Vector2.Distance(mouse, position) <= holeRadius)
                return number;
        }

        return -1;
    }

    private float GetMouseAngle(PointerEventData eventData)
    {
        Vector2 center =
            RectTransformUtility.WorldToScreenPoint(
                eventData.pressEventCamera,
                dial.position
            );

        Vector2 direction = eventData.position - center;

        return Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (returning)
            return;

        selectedNumber = FindNumber(eventData);

        if (selectedNumber < 0)
            return;
        
        float numberAngle = GetNumberAngle(selectedNumber);

        maximumRotation = Mathf.Repeat(
            numberAngle - stopAngle,
            360f
        );

        currentRotation = 0f;
        previousMouseAngle = GetMouseAngle(eventData);

        dragging = true;
        
        SoundManager.Instance.PlaySFX("Phone_Drag");

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging)
            return;

        float mouseAngle = GetMouseAngle(eventData);

        float delta = Mathf.DeltaAngle(
            previousMouseAngle,
            mouseAngle
        );

        previousMouseAngle = mouseAngle;

        currentRotation = Mathf.Clamp(
            currentRotation - delta,
            0f,
            maximumRotation
        );

        ApplyRotation();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging)
            return;
        
        dragging = false;
        returning = true;

        if (currentRotation >= maximumRotation - 2f)
        {
            SoundManager.Instance.PlaySFX("Phone_Dial");
            PhoneController.Instance.DialNumber(selectedNumber);
            Debug.Log($"Dialed: {selectedNumber}");
        }
    }

    private void Update()
    {
        if (!returning)
            return;

        if (!SoundManager.Instance.IsLoopSFXPlaying("Phone_Return"))
        {
            SoundManager.Instance.StartLoopSFX("Phone_Return");
        }
        
        currentRotation = Mathf.MoveTowards(
            currentRotation,
            0f,
            returnSpeed * Time.deltaTime
        );

        ApplyRotation();

        if (currentRotation <= 0f)
        {
            SoundManager.Instance.StopLoopSFX("Phone_Return");
            returning = false;
            selectedNumber = -1;
        }
    }

    private void ApplyRotation()
    {
        dial.localRotation = restingRotation *
            Quaternion.Euler(0f, 0f, -currentRotation);
    }
}
