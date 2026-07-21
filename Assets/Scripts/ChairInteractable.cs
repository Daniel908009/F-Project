using UnityEngine;

public class ChairInteractable : Interactable
{
    [SerializeField] private Transform seatPosition;
    [SerializeField] private Transform exitPosition;

    public override void Interact()
    {
        PlayerModeController.Instance.EnterChair(seatPosition, exitPosition, GetComponentInChildren<Collider>());
    }

    public override string GetInteractionPrompt()
    {
        return "Sit";
    }
}