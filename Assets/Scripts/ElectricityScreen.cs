using UnityEngine;
using TMPro;

public class ElectricityScreen : MonoBehaviour
{
    
    [SerializeField] private TMP_Text CurrentPowerText;
    [SerializeField] private TMP_Text TotalPowerText;
    private void Update()
    {
        
        CurrentPowerText.text = $"C-P: {PowerManager.Instance.GetCurrentPower():F1}";
        TotalPowerText.text = $"T-P: {PowerManager.Instance.GetTotalPower():F1}";
    }
}
