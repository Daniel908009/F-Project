using UnityEngine;

public class PlayerCameras : MonoBehaviour
{
    [SerializeField] private float turningChangeRate = 0.1f;
    private bool isCameraMode = false;
    public void OnChangeNext()
    {
        CameraHandler.Instance.SwitchToNextCamera();
        PlayerModeController.Instance.EnterCamera(CameraHandler.Instance.GetCurrentCameraPosition(), CameraHandler.Instance.GetExitTransform());
    }
    public void OnChangePrevious()
    {
        CameraHandler.Instance.SwitchToPreviousCamera();
        PlayerModeController.Instance.EnterCamera(CameraHandler.Instance.GetCurrentCameraPosition(), CameraHandler.Instance.GetExitTransform());
    }
    public void OnSpeedUP()
    {
        if(!isCameraMode) return;
        SubmarineWaves.Instance.ChangeDesiredSpeed(1f);
    }
    public void OnSpeedDOWN()
    {
        if(!isCameraMode) return;
        SubmarineWaves.Instance.ChangeDesiredSpeed(-1f);
    }
    public void OnTurnLeft()
    {
        if(!isCameraMode) return;
        SubmarineWaves.Instance.ChangeTurning(-turningChangeRate);
    }
    public void OnTurnRight()
    {
        if(!isCameraMode) return;
        SubmarineWaves.Instance.ChangeTurning(turningChangeRate);
    }
    public void OnDepthUP()
    {
        if(!isCameraMode) return;
        SubmarineWaves.Instance.ChangeDesiredDepth(1f);
    }
    public void OnDepthDOWN()
    {
        if(!isCameraMode) return;
        SubmarineWaves.Instance.ChangeDesiredDepth(-1f);
    }
    public void OnControls()
    {
        isCameraMode = !isCameraMode;
    }
}
