using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject normalUI;
    [SerializeField] private GameObject periscopeUI;
    [SerializeField] private GameObject cameraUI;
    public static PlayerUIController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void ShowNormalUI()
    {
        normalUI.SetActive(true);
        periscopeUI.SetActive(false);
        cameraUI.SetActive(false);
    }
    public void ShowPeriscopeUI()
    {
        normalUI.SetActive(false);
        periscopeUI.SetActive(true);
        cameraUI.SetActive(false);
    }
    public void ShowCameraUI()
    {
        normalUI.SetActive(false);
        periscopeUI.SetActive(false);
        cameraUI.SetActive(true);
    }
}
