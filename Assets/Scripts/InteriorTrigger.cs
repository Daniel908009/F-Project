using UnityEngine;

public class OceanVisibilityTrigger : MonoBehaviour
{
    public Camera playerCamera;

    private int waterLayerMask;
    public static OceanVisibilityTrigger Instance { get; private set; }

    private void Awake()
    {
        waterLayerMask = 1 << LayerMask.NameToLayer("Water");
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OceanVisible(bool isVisible)
    {
        if (playerCamera != null)
        {
            playerCamera.cullingMask = isVisible ? playerCamera.cullingMask | waterLayerMask : playerCamera.cullingMask & ~waterLayerMask;
        }
    }
}