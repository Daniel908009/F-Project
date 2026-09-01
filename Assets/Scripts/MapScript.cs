using UnityEngine;
using System.IO;
using System;

public class MapScript : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private GameObject oceanObject;
    [SerializeField] private GameObject submarineMapObject;
    [SerializeField] private GameObject submarineObject;
    [SerializeField] private GameObject TerrainObject;
    [SerializeField] private Transform MapBound1;
    [SerializeField] private Transform MapBound2;

    private bool isOceanVisible = true;
    public static MapScript Instance { get; private set; }
    private const int sourceWidth = 2049;
    private const int sourceHeight = 1025;
    [SerializeField] private int targetWidth = 257;
    [SerializeField] private int targetHeight = 129;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        float[,] heights = LoadMapData("Assets/Map/map.raw");

        Mesh terrainMesh = CreateTerrainMesh(
            heights,
            10f,  // width
            10f,   // depth
            0.005f    // height scale
        );
        meshFilter.mesh = terrainMesh;
    }
    private void Update()
    {
        MoveSubmarineMapObject();
    }
    private void MoveSubmarineMapObject()
    {
        //Debug.Log("mapbound1: " + MapBound1.localPosition);
        //Debug.Log("mapbound2: " + MapBound2.localPosition);
        //Debug.Log("diff: " + -MapBound1.localPosition.y + " - " + MapBound1.localPosition.y);
        //Debug.Break();
        float submarineX = Remap(
            submarineObject.transform.position.x + FloatingOrigin.Instance.GetOffsetPosition().x,
            -ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksX() / 2f,
            ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksX() / 2f,
            -MapBound1.localPosition.y, MapBound1.localPosition.y
        );
        //Debug.Log("submarineX: " + submarineX);
        //Debug.Log("position: " + submarineObject.transform.position.x + ", offset: " + FloatingOrigin.Instance.GetOffsetPosition().x + " = " + (submarineObject.transform.position.x + FloatingOrigin.Instance.GetOffsetPosition().x));
        //Debug.Log("chunkX" + ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksX() / 2f + ", chunk negative: " + -ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksX() / 2f);
        //Debug.Log("mapbound2: " + MapBound2.localPosition.x + ", mapbound2 negative: " + -MapBound2.localPosition.x);
        float submarineZ = Remap(
            submarineObject.transform.position.z + FloatingOrigin.Instance.GetOffsetPosition().z,
            -ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksZ() / 2f,
            ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksZ() / 2f,
            -MapBound2.localPosition.x, MapBound2.localPosition.x
        );
        //Debug.Log("submarineX: " + submarineX + ", submarineZ: " + submarineZ);
        //Debug.Log("position: " + submarineObject.transform.position.z + ", offset: " + FloatingOrigin.Instance.GetOffsetPosition().z + " = " + (submarineObject.transform.position.z + FloatingOrigin.Instance.GetOffsetPosition().z));
        //Debug.Log("chunk positive: " + ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksZ() / 2f + ", chunk negative: " + -ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksZ() / 2f);
        //Debug.Log("mapbound1: " + MapBound1.localPosition.y + ", mapbound1 negative: " + -MapBound1.localPosition.y);
        //Debug.Break();
        submarineMapObject.transform.localPosition = new Vector3(submarineZ, submarineX, submarineMapObject.transform.localPosition.z);
        Quaternion rotation = submarineObject.transform.rotation;
        rotation *= Quaternion.Euler(submarineObject.transform.rotation.eulerAngles);
        rotation *= Quaternion.Euler(0, -90, 0);
        submarineMapObject.transform.rotation = rotation;
    }
    float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        if (Mathf.Approximately(fromMax - fromMin, 0f))
        {
            Debug.LogWarning("Remap: fromMax and fromMin are equal. Returning toMin.");
            return toMin;
        }

        float normalizedValue = Mathf.InverseLerp(fromMin, fromMax, value);
        return Mathf.Lerp(toMin, toMax, normalizedValue);
    }
    private float[,] LoadMapData(string path)
    {
        float[,] heights = new float[targetHeight, targetWidth];

        if (!File.Exists(path))
        {
            Debug.LogError($"Map data file not found: {path}");
            return heights;
        }

        byte[] bytes = File.ReadAllBytes(path);

        // Use every 8th pixel instead of averaging
        const int pixelStep = 8;

        for (int targetZ = 0; targetZ < targetHeight; targetZ++)
        {
            // Calculate source Z coordinate (every 8th pixel)
            int sourceZ = targetZ * pixelStep;

            for (int targetX = 0; targetX < targetWidth; targetX++)
            {
                // Calculate source X coordinate (every 8th pixel)
                int sourceX = targetX * pixelStep;

                // Read single pixel height value
                int index = (sourceZ * sourceWidth + sourceX) * 2;

                if (index + 1 < bytes.Length)
                {
                    ushort heightValue = (ushort)(bytes[index] | (bytes[index + 1] << 8));
                    int flippedTargetZ = targetHeight - 1 - targetZ;
                    heights[flippedTargetZ, targetX] = heightValue / 65535f;
                }
                
                // Commented out averaging logic:
                /*
                int sourceZStart = Mathf.FloorToInt(
                    targetZ * sourceHeight / (float)targetHeight
                );

                int sourceZEnd = Mathf.FloorToInt(
                    (targetZ + 1) * sourceHeight / (float)targetHeight
                );

                int sourceXStart = Mathf.FloorToInt(
                    targetX * sourceWidth / (float)targetWidth
                );

                int sourceXEnd = Mathf.FloorToInt(
                    (targetX + 1) * sourceWidth / (float)targetWidth
                );

                float sum = 0f;
                int count = 0;

                for (int sourceZLoop = sourceZStart; sourceZLoop < sourceZEnd; sourceZLoop++)
                {
                    for (int sourceXLoop = sourceXStart; sourceXLoop < sourceXEnd; sourceXLoop++)
                    {
                        int avgIndex = (sourceZLoop * sourceWidth + sourceXLoop) * 2;

                        if (avgIndex + 1 < bytes.Length)
                        {
                            ushort heightValue = BitConverter.ToUInt16(bytes, avgIndex);

                            sum += heightValue / 65535f;
                            count++;
                        }
                    }
                }

                if (count > 0)
                {
                    int flippedTargetZ = targetHeight - 1 - targetZ;
                    heights[flippedTargetZ, targetX] = sum / count;
                }
                */
            }
        }

        return heights;
    }
    private Mesh CreateTerrainMesh(float[,] heights, float width, float depth, float heightScale)
    {
        int heightCount = heights.GetLength(0);
        int widthCount = heights.GetLength(1);

        int vertexCount = widthCount * heightCount;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];

        int[] triangles = new int[
            (widthCount - 1) * (heightCount - 1) * 6
        ];

        for (int z = 0; z < heightCount; z++)
        {
            for (int x = 0; x < widthCount; x++)
            {
                int index = z * widthCount + x;

                float normalizedX = x / (float)(widthCount - 1);
                float normalizedZ = z / (float)(heightCount - 1);

                float posX = normalizedX * width - width / 2f;
                float posZ = normalizedZ * depth - depth / 2f;

                float posY = heights[z, x] * heightScale;

                vertices[index] = new Vector3(
                    posX,
                    posY,
                    posZ
                );

                uvs[index] = new Vector2(
                    normalizedX,
                    normalizedZ
                );
            }
        }

        int triangleIndex = 0;

        for (int z = 0; z < heightCount - 1; z++)
        {
            for (int x = 0; x < widthCount - 1; x++)
            {
                int bottomLeft = z * widthCount + x;
                int bottomRight = bottomLeft + 1;

                int topLeft = (z + 1) * widthCount + x;
                int topRight = topLeft + 1;

                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = bottomRight;

                triangles[triangleIndex++] = bottomRight;
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = topRight;
            }
        }

        Mesh mesh = new Mesh();

        mesh.name = "Heightmap Terrain";

        if (vertices.Length > 65535)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
    public void ChangeOcean()
    {
        if (isOceanVisible)
        {
            oceanObject.SetActive(false);
            isOceanVisible = false;
        }
        else
        {
            oceanObject.SetActive(true);
            isOceanVisible = true;
        }
    }
}
