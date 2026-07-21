using UnityEngine;

public class PeriscopeInteractable : Interactable
{
    [SerializeField] private GameObject periscopeCamera;
    [SerializeField] private GameObject ExitPoint;

    public override void Interact()
    {
        PlayerModeController.Instance.EnterPeriscope(periscopeCamera.transform, ExitPoint.transform);
    }
    public override string GetInteractionPrompt()
    {
        return "Use Periscope";
    }
}
