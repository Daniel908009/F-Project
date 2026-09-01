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
    [SerializeField] private float maxWakeHeight = 10f;
    public float MaxWakeHeight => maxWakeHeight;
    public static WaveScript Instance { get; private set; }

    private const int MaxWaveCount = 3;

    private float waveTime = 0f;

    Vector3 FOrigin = Vector3.zero;

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
        waveTime += Time.deltaTime;

        if (waterMaterial != null)
        {
            waterMaterial.SetFloat("_WaveTime", Application.isPlaying ? waveTime : 0f);
            waterMaterial.SetFloat("_MaxWakeHeight", maxWakeHeight);
            waterMaterial.SetVector("_PositionIntOffset", transform.position);    
            //Debug.Log(waterMaterial.GetFloat("_WaveTime"));
            //Debug.Break();
            int waveCount = waves == null ? 0 : waves.Length;
            waterMaterial.SetFloat("_NumberOfWaves", waveCount);
            Vector3 floatingOriginOffset = FloatingOrigin.Instance.GetOffsetPosition();
            //Debug.Log("Floating Origin: OffsetX: " + floatingOriginOffset.x + " OffsetZ: " + floatingOriginOffset.z);
            //Debug.Break();
            //floatingOriginOffset = new Vector3(52762, 0f, 35145);
            floatingOriginOffset = new Vector3(FOrigin.x, 0f, FOrigin.z);
            FOrigin = floatingOriginOffset;
            //floatingOriginOffset += transform.position;
            //floatingOriginOffset = Vector3.zero;
            //Debug.Log("Floating Origin Offset: " + floatingOriginOffset);
            //floatingOriginOffset.x = Mathf.Repeat(floatingOriginOffset.x, 90f);
            waterMaterial.SetVector("_OffsetF", new Vector4(FOrigin.x, 0f, 0f, 0f));
            //Debug.Log("Offset: " + offsets);
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
                    waterMaterial.SetVector("_Direction" + waveIndex, direction);
                }
                else
                {
                    waterMaterial.SetFloat("_Amplitude" + waveIndex, 0f);
                    waterMaterial.SetFloat("_Wavelength" + waveIndex, 1f);
                    waterMaterial.SetFloat("_Speed" + waveIndex, 1f);
                    waterMaterial.SetVector("_Direction" + waveIndex, new Vector2(1f, 0f));
                }
            }
        }
    }
    public Vector3 CalculateWave(Vector3 vertex)
    {
        //Debug.Log("Calculating wave for vertex: " + vertex);
        if (waves == null || waves.Length == 0)
        {
            return Vector3.zero;
        }

        Vector3 displacement = Vector3.zero;
        //Vector3 floatingOriginOffset = FloatingOrigin.Instance.GetOffsetPosition();
        //vertex.x += floatingOriginOffset.x;
        //vertex.z += floatingOriginOffset.z;
        Vector3 pos = vertex + new Vector3(FOrigin.x, 0f, 0f);//+ FloatingOrigin.Instance.GetOffsetPosition();
        //Debug.Log("Vertex after applying floating origin offset: " + pos);
        //Debug.Break();
        for (int i = 0; i < waves.Length; i++)
        {
            //Debug.Log("waves.Length: " + waves.Length);
            //Debug.Log("waves: " + waves);
            //Debug.Break();
            GerstnerWave wave = waves[i];
            //Vector2 direction = wave.direction.normalized;
            //Debug.Log("direction: " + direction);
            if (wave.wavelength <= 0f)
            {
                continue;
            }
            
            Vector2 direction = wave.direction.normalized;
            float waveNumber = 2f * 3.14f / wave.wavelength;
            float phase = waveNumber * Vector2.Dot(direction, new Vector2(pos.x, pos.z)) + waterMaterial.GetFloat("_WaveTime") * wave.speed;
            float cosPhase = Mathf.Cos(phase);
            float sinPhase = Mathf.Sin(phase);

            float amplitude = wave.amplitude / waves.Length;

            displacement.x += direction.x * amplitude * cosPhase;
            displacement.y += amplitude * sinPhase;
            displacement.z += direction.y * amplitude * cosPhase;
            
            //displacement.y = Mathf.Sin(waveTime);
        }
        //Debug.Break();
        //Debug.Log("displacement: " + displacement);
        //Debug.DrawLine(vertex, vertex + displacement, Color.green);
        return displacement;
    }
    public Vector3 CalculateSpecificPosition(
        Vector3 worldPosition,
        int iterations = 10)
    {
        Vector2 target = new Vector2(
            worldPosition.x,
            worldPosition.z
        );

        Vector2 original = target;

        const float epsilon = 0.01f;

        for (int i = 0; i < iterations; i++)
        {
            Vector3 p = new Vector3(
                original.x,
                worldPosition.y,
                original.y
            );

            Vector3 displacement = CalculateWave(p);

            Vector2 current = original + new Vector2(
                displacement.x,
                displacement.z
            );

            Vector2 error = current - target;

            if (error.sqrMagnitude < 0.000001f)
                break;

            Vector3 px = new Vector3(
                original.x + epsilon,
                worldPosition.y,
                original.y
            );

            Vector3 pz = new Vector3(
                original.x,
                worldPosition.y,
                original.y + epsilon
            );

            Vector3 displacementX = CalculateWave(px);
            Vector3 displacementZ = CalculateWave(pz);

            float j11 = 1f + (displacementX.x - displacement.x) / epsilon;
            float j12 = (displacementZ.x - displacement.x) / epsilon;

            float j21 = (displacementX.z - displacement.z) / epsilon;
            float j22 = 1f + (displacementZ.z - displacement.z) / epsilon;

            float determinant =
                j11 * j22 -
                j12 * j21;

            if (Mathf.Abs(determinant) < 0.000001f)
                break;

            float dx =
                (j22 * error.x - j12 * error.y) /
                determinant;

            float dz =
                (-j21 * error.x + j11 * error.y) /
                determinant;

            original.x -= dx;
            original.y -= dz;
        }

        Vector3 finalPosition = new Vector3(
            original.x,
            worldPosition.y,
            original.y
        );

        return CalculateWave(finalPosition);
    }
}
