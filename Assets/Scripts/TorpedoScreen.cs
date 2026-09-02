using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TorpedoScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField TimeInputField;
    [SerializeField] private TMP_InputField AngleInputField;
    [SerializeField] private TMP_InputField TorpedoTypeInputField;
    [SerializeField] private Image[] SelectedTubeImages;
    [SerializeField] private Image SelectedTubePanel;
    private void Start()
    {
        ChangeSelectedTorpedoType(0);
    }
    public void ChangeTime(float change)
    {
        float currentTime;
        if (change == 0f)
        {
            currentTime = 0f;
            TimeInputField.text = currentTime.ToString("F2");
            return;
        }
        if(float.TryParse(TimeInputField.text, out currentTime))
        {
        }
        else
        {
            currentTime = 0;
        }
        currentTime += change;
        if (currentTime < 0f)
        {
            currentTime = 0f;
        }
        TimeInputField.text = currentTime.ToString("F2");
    }
    public void ChangeAngle(float change)
    {
        float currentAngle;
        if (change == 0f)
        {
            currentAngle = SubmarineWaves.Instance.GetCurrentYRotation();
            currentAngle = Mathf.Round(currentAngle);
            AngleInputField.text = currentAngle.ToString("F2");
            return;
        }
        if(float.TryParse(AngleInputField.text, out currentAngle))
        {
        }
        else
        {
            currentAngle = SubmarineWaves.Instance.GetCurrentYRotation();
        }
        currentAngle += change;
        if (currentAngle < 0f)
        {
            currentAngle = 360f;
        }
        else if (currentAngle > 360f)
        {
            currentAngle = 0f;
        }
        currentAngle = Mathf.Round(currentAngle);
        AngleInputField.text = currentAngle.ToString("F2");
    }
    public void UpdateSelectedTubeImages(float selectedTubeIndex)
    {
        for (int i = 0; i < SelectedTubeImages.Length; i++)
        {
            if (i == (int)selectedTubeIndex)
            {
                SelectedTubeImages[i].color = Color.red;
                if (TorpedoLauncher.Instance != null)
                {
                    bool isFlooded = TorpedoLauncher.Instance.TorpedoTubes[i].IsFlooded;
                    bool hasTorpedo = TorpedoLauncher.Instance.TorpedoTubes[i].TorpedoPrefab != null;
                    if (isFlooded && !hasTorpedo)
                    {
                        SelectedTubeImages[i].color = Color.blue;
                    }
                    if (!isFlooded && hasTorpedo)
                    {
                        SelectedTubeImages[i].color = Color.yellow;
                    }
                    if (hasTorpedo && isFlooded)
                    {
                        SelectedTubeImages[i].color = Color.green;
                    }
                }
                RectTransform selectedTubeRect = SelectedTubeImages[i].GetComponent<RectTransform>();
                SelectedTubePanel.rectTransform.position = selectedTubeRect.position;
            }
        }
    }
    public void FloodTube()
    {
        TorpedoLauncher.Instance.FloodSelectedTube(UpdateSelectedTubeImages);
    }
    public void ChangeSelectedTubeIndex(float newIndex)
    {
        TorpedoLauncher.Instance.ChangeSelectedTubeIndex(newIndex);
        UpdateSelectedTubeImages(newIndex);
    }
    public void ChangeSelectedTorpedoType(int change)
    {
        if (change == 0)
        {
            TorpedoTypeInputField.text = TorpedoLauncher.Instance.TorpedoTypes[0].Name;
            return;
        }
        int currentIndex = 0;
        for (int i = 0; i < TorpedoLauncher.Instance.TorpedoTypes.Length; i++)
        {
            if (TorpedoLauncher.Instance.TorpedoTypes[i].Name == TorpedoTypeInputField.text)
            {
                currentIndex = i;
                break;
            }
        }
        currentIndex += change;
        if (currentIndex < 0)
        {
            currentIndex = TorpedoLauncher.Instance.TorpedoTypes.Length - 1;
        }
        else if (currentIndex >= TorpedoLauncher.Instance.TorpedoTypes.Length)
        {
            currentIndex = 0;
        }
        TorpedoTypeInputField.text = TorpedoLauncher.Instance.TorpedoTypes[currentIndex].Name;
    }
    public void LoadTorpedoIntoSelectedTube()
    {
        int dropdownIndex = 0;
        for (int i = 0; i < TorpedoLauncher.Instance.TorpedoTypes.Length; i++)
        {
            if (TorpedoLauncher.Instance.TorpedoTypes[i].Name == TorpedoTypeInputField.text)
            {
                dropdownIndex = i;
                break;
            }
        }
        TorpedoLauncher.Instance.LoadTorpedoIntoSelectedTube(dropdownIndex, UpdateSelectedTubeImages);
    }
    public void FireTorpedoFromSelectedTube()
    {
        float time = 0f;
        float angle = 0f;
        if (!float.TryParse(TimeInputField.text, out time))
        {
            time = 0f;
        }
        if (!float.TryParse(AngleInputField.text, out angle))
        {
            angle = SubmarineWaves.Instance.GetCurrentYRotation();
        }
        TorpedoLauncher.Instance.FireTorpedoFromSelectedTube(time, angle, UpdateSelectedTubeImages);
    }
}
