using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Transform exitTransform;
    [SerializeField] private Transform[] cameraPositions;

    private int currentCameraIndex = 0;
    public static CameraHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public Transform GetCurrentCameraPosition()
    {
        return cameraPositions[currentCameraIndex];
    }
    public Transform GetExitTransform()
    {
        return exitTransform;
    }
    public void SwitchToNextCamera()
    {
        currentCameraIndex = (currentCameraIndex + 1) % cameraPositions.Length;
    }
    public void SwitchToPreviousCamera()
    {
        currentCameraIndex = (currentCameraIndex - 1 + cameraPositions.Length) % cameraPositions.Length;
    }
}
