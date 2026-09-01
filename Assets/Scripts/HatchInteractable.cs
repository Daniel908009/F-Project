using UnityEngine;

public class HatchInteractable : Interactable
{
    [Header("Hatch Parts")]
    [SerializeField] private Transform hatchPivot;

    [Header("Rotation")]
    [SerializeField] private Vector3 closedRotation;
    [SerializeField] private Vector3 openRotation = new Vector3(0f, 90f, 0f);

    [Header("Animation")]
    [SerializeField] private float openSpeed = 2f;

    private bool isOpen = false;
    private bool isMoving = false;
    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = Quaternion.Euler(closedRotation);

        if (hatchPivot != null)
        {
            hatchPivot.localRotation = targetRotation;
        }
    }

    private void Update()
    {
        if (hatchPivot == null) return;

        hatchPivot.localRotation = Quaternion.Lerp(
            hatchPivot.localRotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );

        if (Quaternion.Angle(hatchPivot.localRotation, targetRotation) < 0.1f)
        {
            hatchPivot.localRotation = targetRotation;
            isMoving = false;
        }
    }

    public override void Interact()
    {
        if (hatchPivot == null) return;
        if (isMoving) return;

        isOpen = !isOpen;
        isMoving = true;

        if (isOpen)
        {
            targetRotation = Quaternion.Euler(openRotation);
        }
        else
        {
            targetRotation = Quaternion.Euler(closedRotation);
        }
    }

    public override string GetInteractionPrompt()
    {
        return isOpen ? "Close" : "Open";
    }
    public bool IsHatchOpen()
    {
        return isOpen;
    }
}