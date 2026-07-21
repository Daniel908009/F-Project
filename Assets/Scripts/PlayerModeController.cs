using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerModeController : MonoBehaviour
{
    public static PlayerModeController Instance { get; private set; }
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform periscopeCamera;
    [SerializeField] private Transform Submarine;

    private Transform currentExitPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void EnterChair(Transform seatPosition, Transform exitPosition, Collider chairCollider)
    {
        characterController.enabled = false;
        currentExitPoint = exitPosition;
        playerMovement.CanMove = false;
        transform.position = seatPosition.position;
        playerInput.SwitchCurrentActionMap("Chair");
        characterController.enabled = true;
    }
    public void EnterPeriscope(Transform periscopeCamera, Transform exitPoint)
    {
        //Debug.Log("Entering Periscope");
        characterController.enabled = false;
        transform.position = periscopeCamera.position - new Vector3(0, characterController.height / 2f, 0);
        transform.rotation = periscopeCamera.rotation;
        playerMovement.CanMove = false;
        currentExitPoint = exitPoint;
        playerInput.SwitchCurrentActionMap("Periscope");
        playerMovement.MinVerticalAngle = -30f;
        playerMovement.MaxVerticalAngle = 30f;
        PlayerUIController.Instance.ShowPeriscopeUI();
        CameraController.Instance.ZoomCamera(CameraController.Instance.normalFOV);
        Environment.Instance.playerInsideSub = false;
        OceanVisibilityTrigger.Instance.OceanVisible(true);
    }
    public void OnExit()
    {
        ExitStation();
    }
    public void ExitStation()
    {
        characterController.enabled = false;
        transform.position = currentExitPoint.position;
        transform.rotation = currentExitPoint.rotation;
        playerInput.SwitchCurrentActionMap("Player");
        characterController.enabled = true;
        playerMovement.CanMove = true;
        playerMovement.resetAngleLimits();
        PlayerUIController.Instance.ShowNormalUI();
        CameraController.Instance.ZoomCamera(CameraController.Instance.normalFOV);
        Environment.Instance.playerInsideSub = true;
        OceanVisibilityTrigger.Instance.OceanVisible(false);
    }
}