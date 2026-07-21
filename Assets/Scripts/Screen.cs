using UnityEngine;

public class ScreenController : MonoBehaviour
{

    [SerializeField] private GameObject noPower;
    [SerializeField] private GameObject onScreen;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private UnityEngine.UI.Slider ProgressBar;

    [SerializeField] private PowerCircuit powerCircuit;

    private bool bootScreenPlayed = false;

    private int bootTime = 0;

    private float bootTimer = 0;

    private void Start()
    {
        bootTime = Random.Range(6, 12);
    }
    private void Update()
    {
        if (PowerManager.Instance.IsPowered(powerCircuit) && bootScreenPlayed)
        {
            noPower.SetActive(false);
            startScreen.SetActive(false);
            onScreen.SetActive(true);
        }
        else if (PowerManager.Instance.IsPowered(powerCircuit) && !bootScreenPlayed)
        {
            noPower.SetActive(false);
            onScreen.SetActive(false);
            startScreen.SetActive(true);
            bootTimer += Time.deltaTime;
            ProgressBar.value = bootTimer / bootTime;
            if (bootTimer >= bootTime)
            {
                bootScreenPlayed = true;
            }
        }
        else
        {
            noPower.SetActive(true);
            onScreen.SetActive(false);
            startScreen.SetActive(false);
            bootScreenPlayed = false;
            bootTimer = 0;
        }
    }
}