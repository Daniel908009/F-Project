using UnityEngine;
using UnityEngine.Rendering;

public class Environment : MonoBehaviour
{
    public Transform playerCamera;
    public Volume underwaterVolume;

    public Color surfaceFogColor = Color.white;
    public float surfaceFogStart = 100f;
    public float surfaceFogEnd = 300f;

    public Color underwaterFogColor = new Color(0.1f, 0.3f, 0.5f);
    public float underwaterFogStart = 0f;
    public float underwaterFogEnd = 30f;

    private Vector3 waterLevel = Vector3.zero;

    public bool playerInsideSub = false;

    public static Environment Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void Update()
    {
        if (playerInsideSub)
        {
            SetSurface();
            underwaterVolume.weight = 0f;
            return;
        }
        waterLevel = WaveScript.Instance.CalculateWave(playerCamera.position);
        float depth = waterLevel.y - playerCamera.position.y;

        if (depth > -2f)
        {
            //Debug.Log("Player is underwater. Depth: " + depth + " Water Level: " + waterLevel.y + " Player Y: " + playerCamera.position.y);
            RenderSettings.fogColor = underwaterFogColor;
            RenderSettings.fogStartDistance = underwaterFogStart;
            RenderSettings.fogEndDistance = underwaterFogEnd;

            underwaterVolume.weight = Mathf.Clamp01(depth / 50f);
        }
        else
        {
            SetSurface();
            underwaterVolume.weight = 0f;
        }

        /*Vector3 origin =  Vector3.zero;
        Vector3 displacement = WaveScript.Instance.CalculateWave(origin);

        Vector3 cpuSurface =
            new Vector3(origin.x, 0f, origin.z) + displacement;
        Debug.DrawLine(origin, cpuSurface, Color.green);*/
    }

    void SetSurface()
    {
        RenderSettings.fogColor = surfaceFogColor;
        RenderSettings.fogStartDistance = surfaceFogStart;
        RenderSettings.fogEndDistance = surfaceFogEnd;
    }
}