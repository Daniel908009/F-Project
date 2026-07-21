using UnityEngine;

public class PeriscopeController : MonoBehaviour
{
    [SerializeField] private Transform MovingPart;
    [SerializeField] public float moveSpeed = 1f;
    [SerializeField] public Transform maxYTransform;
    [SerializeField] public Transform minYTransform;

    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float maxZoom = 30;

    private float maxY;
    private float minY;
    private void Start()
    {
        maxY = maxYTransform.localPosition.y;
        minY = minYTransform.localPosition.y;
    }
    public bool MovePeriscope(float verticalInput)
    {
        Vector3 newPosition = MovingPart.localPosition;
        newPosition.y += verticalInput * moveSpeed * Time.deltaTime;
        //newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        if(newPosition.y > maxY)
        {
            newPosition.y = maxY;
            return false;
        }
        else if(newPosition.y < minY)
        {
            newPosition.y = minY;
            return false;
        }
        MovingPart.localPosition = newPosition;
        return true;
    }
    public void Zoom(float zoomInput)
    {
        float targetFOV = CameraController.Instance.targetFOV - zoomInput * zoomSpeed;
        targetFOV = Mathf.Clamp(targetFOV, maxZoom, CameraController.Instance.normalFOV);
        CameraController.Instance.ZoomCamera(targetFOV);
    }
}
