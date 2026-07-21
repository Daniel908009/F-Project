using UnityEngine;
[System.Serializable]
public struct GerstnerWave
{
    public Vector2 direction;
    public float amplitude;
    public float wavelength;
    public float speed;
}
public class WaveScript : MonoBehaviour
{
    [SerializeField] private GerstnerWave[] waves;
    [SerializeField] private Material waterMaterial;
    public static WaveScript Instance { get; private set; }

    private const int MaxWaveCount = 3;

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
        if (waterMaterial != null)
        {
            waterMaterial.SetFloat("_WaveTime", Application.isPlaying ? Time.time : 0f);
            //Debug.Log("Wave Time: " + (Application.isPlaying ? Time.time : 0f));

            int waveCount = waves == null ? 0 : waves.Length;
            waterMaterial.SetFloat("_NumberOfWaves", waveCount);
            //Debug.Log("Wave Count: " + waveCount);
            for (int i = 0; i < MaxWaveCount; i++)
            {
                int waveIndex = i + 1;

                if (i < waveCount)
                {
                    GerstnerWave wave = waves[i];
                    Vector2 direction = wave.direction.normalized;

                    waterMaterial.SetFloat("_Amplitude" + waveIndex, wave.amplitude);
                    waterMaterial.SetFloat("_Wavelength" + waveIndex, wave.wavelength);
                    waterMaterial.SetFloat("_Speed" + waveIndex, wave.speed);
                    waterMaterial.SetVector("_Direction" + waveIndex, new Vector4(direction.x, direction.y, 0f, 0f));
                }
                else
                {
                    waterMaterial.SetFloat("_Amplitude" + waveIndex, 0f);
                    waterMaterial.SetFloat("_Wavelength" + waveIndex, 1f);
                    waterMaterial.SetFloat("_Speed" + waveIndex, 1f);
                    waterMaterial.SetVector("_Direction" + waveIndex, new Vector4(1f, 0f, 0f, 0f));
                }
            }
        }
    }
    public Vector3 CalculateWave(Vector3 vertex)
    {
        if (waves == null || waves.Length == 0)
        {
            return Vector3.zero;
        }

        Vector3 displacement = Vector3.zero;

        for (int i = 0; i < waves.Length; i++)
        {
            GerstnerWave wave = waves[i];

            if (wave.wavelength <= 0f)
            {
                continue;
            }

            Vector2 direction = wave.direction.normalized;
            float waveNumber = 2f * Mathf.PI / wave.wavelength;
            float phase = waveNumber * Vector2.Dot(direction, new Vector2(vertex.x, vertex.z)) + Time.time * wave.speed;
            float cosPhase = Mathf.Cos(phase);
            float sinPhase = Mathf.Sin(phase);

            wave.amplitude = wave.amplitude / waves.Length;
            //Debug.Log("wavecount" + waveCount);
            displacement.x += direction.x * wave.amplitude * cosPhase;
            displacement.y += wave.amplitude * sinPhase;
            displacement.z += direction.y * wave.amplitude * cosPhase;

            //Debug.Log("/////////////////");
            //Debug.Log("direction: " + direction);
            //Debug.Log("waveNumber: " + waveNumber);
            //Debug.Log("phase: " + phase);
            //Debug.Log("sin(phase): " + Mathf.Sin(phase));
            //Debug.Log("displacement.y: " + displacement.y);
        }

        return displacement;
    }
}
