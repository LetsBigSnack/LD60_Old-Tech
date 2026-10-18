using Data;
using UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIDraggable))]
public class UIPunchcardController : MonoBehaviour
{
    [Header("Punch Card")]
    [SerializeField] private GameObject bigVersion;
    [SerializeField] private PunchCardData punchCardData;

    public PunchCardData PunchCardData => punchCardData;

    public UICardslotHolder CurrentSlot { get; set; }

    private UIDraggable draggable;
    private Button[] buttons;

    private void Awake()
    {
        draggable = GetComponent<UIDraggable>();

        AssignButtons();
    }

    private void OnEnable()
    {
        draggable.DragStarted += OnDragStarted;
    }

    private void OnDisable()
    {
        draggable.DragStarted -= OnDragStarted;
    }

    private void OnDragStarted()
    {
        CurrentSlot?.RemovePunchcard();
    }

    private void AssignButtons()
    {
        buttons = bigVersion.GetComponentsInChildren<Button>(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => PierceHole(index));
        }
    }

    private void PierceHole(int index)
    {
        punchCardData.Punch(index / 6, index % 6);
        buttons[index].interactable = false;
    }
}