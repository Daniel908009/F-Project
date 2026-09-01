using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light sun;
    [SerializeField] private float dayLengthMinutes = 10f;

    public static DayNightCycle Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        float degreesPerSecond = 360f / (dayLengthMinutes * 60f);

        sun.transform.Rotate(
            degreesPerSecond * Time.deltaTime,
            0f,
            0f,
            Space.Self
        );
    }
}

