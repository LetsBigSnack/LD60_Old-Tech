
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UIDraggable))]
public class UIZoneResizer : MonoBehaviour, IDragHandler
{
    [Header("Versions")]
    [SerializeField] private GameObject bigVersion;
    [SerializeField] private GameObject smallVersion;

    [Header("Zone")]
    [SerializeField] private RectTransform deskZone;

    public GameObject ActiveVersion { get; private set; }

    private Canvas canvas;
    private UIDraggable draggable;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>().rootCanvas;
        draggable = GetComponent<UIDraggable>();

        if (deskZone == null)
        {
            deskZone = GameObject
                .FindGameObjectWithTag("BigArea")
                .GetComponent<RectTransform>();
        }

        ActiveVersion = bigVersion.activeSelf
            ? bigVersion
            : smallVersion;

        bigVersion.SetActive(ActiveVersion == bigVersion);
        smallVersion.SetActive(ActiveVersion == smallVersion);
    }

    private void OnEnable()
    {
        draggable.DragEnded += OnDragEnded;
    }

    private void OnDisable()
    {
        draggable.DragEnded -= OnDragEnded;
    }

    private void Start()
    {
        UpdateVersion();
    }

    private Camera GetUICamera()
    {
        return canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateVersion(eventData);
    }

    private void OnDragEnded()
    {
        // Recalculate using the actual final position.
        UpdateVersion();
    }

    public void UpdateVersion(PointerEventData eventData = null)
    {
        Camera camera = GetUICamera();
        Vector2 screenPoint;

        if (eventData != null)
        {
            screenPoint = eventData.position;
        }
        else
        {
            RectTransform activeRect =
                ActiveVersion.GetComponent<RectTransform>();

            Vector3 worldCenter =
                activeRect.TransformPoint(activeRect.rect.center);

            screenPoint = RectTransformUtility.WorldToScreenPoint(
                camera,
                worldCenter
            );
        }

        bool onDesk =
            RectTransformUtility.RectangleContainsScreenPoint(
                deskZone,
                screenPoint,
                camera
            );

        SetVersion(onDesk);
    }

    private void SetVersion(bool big)
    {
        GameObject newVersion = big ? bigVersion : smallVersion;

        if (ActiveVersion == newVersion)
            return;

        ActiveVersion.SetActive(false);
        newVersion.SetActive(true);

        ActiveVersion = newVersion;
    }
}
