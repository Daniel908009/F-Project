using UnityEngine;
using TMPro;

public class DepthScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text DesiredDepthText;
    [SerializeField] private TMP_Text CurrentDepthText;

    [SerializeField] private float depthChangeAmount = 1f;
    [SerializeField] private float periscopeDepth = 10f;
    private void Update()
    {
        UpdateDepthText();
    }
    public void ChangeDepth(string change)
    {
        float changeAmount = 0f;
        if(change == "up")
        {
            changeAmount = -depthChangeAmount;
        }
        else if(change == "down")
        {
            changeAmount = depthChangeAmount;
        }
        else if(change == "surface")
        {
            changeAmount = -SubmarineWaves.Instance.GetDesiredDepth();
        }
        else if (change == "periscopeDepth")
        {
            changeAmount = periscopeDepth - SubmarineWaves.Instance.GetDesiredDepth();
        }
        else
        {
            return;
        }
        SubmarineWaves.Instance.ChangeDesiredDepth(changeAmount);
    }
    private void UpdateDepthText()
    {
        DesiredDepthText.text = $"T-Depth: {SubmarineWaves.Instance.GetDesiredDepth():F1} m";
        CurrentDepthText.text = $"C-Depth: {SubmarineWaves.Instance.GetCurrentDepth():F1} m";
    }
}
