using UnityEngine;
using UnityEngine.Rendering;

public class Environment : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Volume underwaterVolume;

    [SerializeField] private Color surfaceFogColor = Color.white;
    [SerializeField] private float surfaceFogStart = 100f;
    [SerializeField] private float surfaceFogEnd = 300f;

    [SerializeField] private Color surfaceSkyColor = new Color(0.5f, 0.7f, 1f);
    [SerializeField] private Color underwaterFogColor = new Color(0.1f, 0.3f, 0.5f);
    [SerializeField] private float underwaterFogStart = 0f;
    [SerializeField] private float underwaterFogEnd = 30f;

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
            playerCamera.backgroundColor = surfaceSkyColor;
            return;
        }
        waterLevel = WaveScript.Instance.CalculateSpecificPosition(playerCamera.transform.position);
        float depth = waterLevel.y - playerCamera.transform.position.y;
        //Vector3 cwaveres = WaveScript.Instance.CalculateWave(playerCamera.transform.position);
        //Vector3 cheight = WaveScript.Instance.CalculateSpecificPosition(playerCamera.transform.position);
        //Debug.Log(cwaveres + " " + cheight);
        //waterLevel.x += playerCamera.transform.position.x;
        //waterLevel.z += playerCamera.transform.position.z;
        //Debug.Log("Water Level: " + waterLevel);
        //Debug.DrawLine(playerCamera.transform.position, waterLevel, Color.red);
        //Mesh oceanMesh = GameObject.Find("OceanTile").GetComponent<MeshFilter>().mesh;
        //Vector3[] vertices = oceanMesh.vertices;
        //Vector3 positionOfVertice = vertices[0];
        //Vector3 waveLevel = WaveScript.Instance.CalculateWave(positionOfVertice + FloatingOrigin.Instance.GetOffsetPosition());
        //Debug.DrawLine(new Vector3(positionOfVertice.x, 0, positionOfVertice.z), waveLevel, Color.red);
        if (depth > -0.5f)
        {
            //Debug.Log("Player is underwater. Depth: " + depth + " Water Level: " + waterLevel.y + " Player Y: " + playerCamera.position.y);
            RenderSettings.fogColor = underwaterFogColor;
            RenderSettings.fogStartDistance = underwaterFogStart;
            RenderSettings.fogEndDistance = underwaterFogEnd;
            playerCamera.clearFlags = CameraClearFlags.SolidColor;
            playerCamera.backgroundColor = underwaterFogColor;
            underwaterVolume.weight = Mathf.Clamp01(depth / 50f);
        }
        else
        {
            SetSurface();
            underwaterVolume.weight = 0f;
            playerCamera.clearFlags = CameraClearFlags.Skybox;
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