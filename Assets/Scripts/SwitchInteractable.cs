using UnityEngine;

public class SwitchInteractable : Interactable
{
    [SerializeField] private Transform switchTransform;
    [SerializeField] private Vector3 offRotation;
    [SerializeField] private Vector3 onRotation;

    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;

    [SerializeField] private MeshRenderer switchRenderer;

    [SerializeField] private PowerCircuit powerCircuit;

    private bool isOn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switchTransform.rotation = Quaternion.Euler(offRotation);
        switchRenderer.material = offMaterial;
    }

    public override void Interact()
    {
        isOn = !isOn;

        if (isOn)
        {
            switchTransform.rotation = Quaternion.Euler(onRotation);
            switchRenderer.material = onMaterial;
            PowerManager.Instance.SetPower(powerCircuit, true);
        }
        else
        {
            switchTransform.rotation = Quaternion.Euler(offRotation);
            switchRenderer.material = offMaterial;
            PowerManager.Instance.SetPower(powerCircuit, false);
        }
    }
    public override string GetInteractionPrompt()
    {
        return powerCircuit.ToString() + (isOn ? " (ON)" : " (OFF)");
    }
}
