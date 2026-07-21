using UnityEngine;
using TMPro;

public class SpeedScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text DesiredSpeedText;
    [SerializeField] private TMP_Text CurrentSpeedText;

    [SerializeField] private TMP_Text TurningText;
    [SerializeField] private float speedChangeRate = 1f;
    [SerializeField] private float turningChangeRate = 0.1f;
    private void Update()
    {
        UpdateTexts();
    }
    public void Change(string change)
    {
        if (change == "increaseSpeed")
        {
            SubmarineWaves.Instance.ChangeDesiredSpeed(speedChangeRate);
            //Debug.Log($"Increasing speed. New desired speed: {SubmarineWaves.Instance.GetDesiredSpeed()}");
        }
        else if (change == "decreaseSpeed")
        {
            SubmarineWaves.Instance.ChangeDesiredSpeed(-speedChangeRate);
        }
        else if (change == "turnLeft")
        {
            SubmarineWaves.Instance.ChangeTurning(-turningChangeRate);
        }
        else if (change == "turnRight")
        {
            SubmarineWaves.Instance.ChangeTurning(turningChangeRate);
        }
    }
    private void UpdateTexts()
    {
        DesiredSpeedText.text = $"T-S: {SubmarineWaves.Instance.GetDesiredSpeed():F1}";
        CurrentSpeedText.text = $"C-S: {SubmarineWaves.Instance.GetCurrentSpeed():F1}";
        TurningText.text = $"T: {SubmarineWaves.Instance.GetTurning():F1}";
    }
}
