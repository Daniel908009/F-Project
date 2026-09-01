using UnityEngine;

public class WaterPumps : MonoBehaviour
{
    public static WaterPumps Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public float PumpingSpeedInRoom() // later
    {
        return 0f;
    }
}
