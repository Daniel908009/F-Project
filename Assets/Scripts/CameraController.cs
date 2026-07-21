using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public float targetFOV;
    public float normalFOV = 60f;
    public float zoomedFOV = 30f;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        targetFOV = normalFOV;
    }
    public void ZoomCamera(float targetFOV)
    {
        this.targetFOV = targetFOV;
    }
    private void Update()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * 5f);
        //Debug.Log($"Current FOV: {Camera.main.fieldOfView}, Target FOV: {targetFOV}");
    }
}
