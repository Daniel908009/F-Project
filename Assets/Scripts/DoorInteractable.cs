using UnityEngine;

public class DoorInteractable : Interactable
{
    [SerializeField] private Transform door;

    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private Vector3 openOffset = new Vector3(1.5f, 0f, 0f);

    [SerializeField] private float moveSpeed = 2f;

    private Vector3 targetPosition;
    private bool isOpen = false;
    private bool isMoving;

    private void Start()
    {
        targetPosition = closedPosition;

        if (door != null)
            door.localPosition = closedPosition;
    }

    private void Update()
    {
        if (door == null)
            return;

        door.localPosition = Vector3.MoveTowards(
            door.localPosition,
            targetPosition,
            moveSpeed * Time.deltaTime);

        if (Vector3.Distance(door.localPosition, targetPosition) < 0.001f)
        {
            door.localPosition = targetPosition;
            isMoving = false;
        }
    }

    public override void Interact()
    {
        if (isMoving)
            return;

        isOpen = !isOpen;
        isMoving = true;

        if (isOpen)
            targetPosition = closedPosition + openOffset;
        else
            targetPosition = closedPosition;
    }

    public override string GetInteractionPrompt()
    {
        return isOpen ? "E: Close" : "E: Open";
    }
}