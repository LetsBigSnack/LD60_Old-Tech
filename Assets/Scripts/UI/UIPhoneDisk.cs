
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class RotaryPhoneDial : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Numbers")]
    [SerializeField] private float numberRadius = 100f;
    [SerializeField] private float numberSize = 32f;
    [SerializeField] private float holeRadius = 22f;
    [SerializeField] private float firstNumberAngle = 35f;
    [SerializeField] private float numberSpacing = 30f;

    [Header("Finger Stop")]
    [SerializeField] private float stopAngle = -25f;
    [SerializeField] private float stopLabelSize = 20f;

    [Header("Return")]
    [SerializeField] private float returnSpeed = 300f;

    private RectTransform dial;
    private RectTransform stopMarker;

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
        CreateFingerStop();
    }

    private void CreateNumbers()
    {
        for (int i = 1; i <= 10; i++)
        {
            int number = i % 10;
            string objectName = $"Number_{number}";

            Transform existing = transform.Find(objectName);
            GameObject numberObject;

            if (existing != null)
            {
                numberObject = existing.gameObject;
            }
            else
            {
                numberObject = new GameObject(
                    objectName,
                    typeof(RectTransform),
                    typeof(TextMeshProUGUI)
                );

                numberObject.transform.SetParent(
                    transform,
                    false
                );
            }

            RectTransform rect =
                numberObject.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(50f, 50f);

            float angle =
                GetNumberAngle(number) * Mathf.Deg2Rad;

            rect.anchoredPosition = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * numberRadius;

            TextMeshProUGUI text =
                numberObject.GetComponent<TextMeshProUGUI>();

            text.text = number.ToString();
            text.fontSize = numberSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.raycastTarget = false;
        }
    }

    private void CreateFingerStop()
    {
        if (transform.parent == null)
            return;

        Transform parent = transform.parent;
        Transform existing = parent.Find("FingerStop");

        GameObject stopObject;

        if (existing != null)
        {
            stopObject = existing.gameObject;
        }
        else
        {
            stopObject = new GameObject(
                "FingerStop",
                typeof(RectTransform),
                typeof(TextMeshProUGUI)
            );

            stopObject.transform.SetParent(parent, false);
        }

        stopMarker =
            stopObject.GetComponent<RectTransform>();

        stopMarker.anchorMin = new Vector2(0.5f, 0.5f);
        stopMarker.anchorMax = new Vector2(0.5f, 0.5f);
        stopMarker.pivot = new Vector2(0.5f, 0.5f);
        stopMarker.sizeDelta = new Vector2(70f, 40f);

        TextMeshProUGUI text =
            stopObject.GetComponent<TextMeshProUGUI>();

        text.text = "STOP";
        text.fontSize = stopLabelSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.red;
        text.raycastTarget = false;
        
        stopMarker.SetAsLastSibling();

        UpdateFingerStopPosition();
    }

    private void UpdateFingerStopPosition()
    {
        if (stopMarker == null)
            return;

        float angle = stopAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * numberRadius,
            Mathf.Sin(angle) * numberRadius,
            0f
        );
        
        stopMarker.localPosition = dial.localPosition + offset;
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

        Vector2 direction =
            eventData.position - center;

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
            PhoneController.Instance.DialNumber(selectedNumber);
            Debug.Log($"Dialed: {selectedNumber}");
        }
    }

    private void Update()
    {
        if (!returning)
            return;

        currentRotation = Mathf.MoveTowards(
            currentRotation,
            0f,
            returnSpeed * Time.deltaTime
        );

        ApplyRotation();

        if (currentRotation <= 0f)
        {
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
