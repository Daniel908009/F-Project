using UnityEngine;

public class CameraEntrypointInteractable : Interactable
{
    [SerializeField] private PowerCircuit powerCircuit;
    public override void Interact()
    {
        if (!PowerManager.Instance.IsPowered(powerCircuit))
        {
            return;
        }
        PlayerModeController.Instance.EnterCamera(CameraHandler.Instance.GetCurrentCameraPosition(), CameraHandler.Instance.GetExitTransform());
    }
    public override string GetInteractionPrompt()
    {
        if (!PowerManager.Instance.IsPowered(powerCircuit))
        {
            return "No power";
        }
        return "Enter Cameras";
    }
}
