using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float chunkSize = 50000f;
    [SerializeField] private int numberOfChunksX = 4;
    [SerializeField] private int numberOfChunksZ = 8;
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private TerrainLayer sandLayer;
    [SerializeField] private TerrainLayer snowLayer;
    [SerializeField] private TerrainLayer rockLayer;
    [SerializeField] private TerrainLayer grassLayer;
    [SerializeField] private TerrainLayer grassXrockLayer;
    [SerializeField] private float sandHeight;
    [SerializeField] private float grassHeight;
    [SerializeField] private float grassXrockHeight;
    [SerializeField] private float snowHeight;
    [SerializeField] private float stoneSlopeThreshold;
    [SerializeField] private float equatorSnowTreshold;
    [SerializeField] private Material material;
    [SerializeField] private int resolution = 257;
    [SerializeField] private float chunkHeight = 5000f;
    [SerializeField] private float chunkHeightOffset = -76f;

    private int playerChunkX = int.MaxValue;
    private int playerChunkZ = int.MaxValue;
    private List<Vector2Int> desiredChunksList = new List<Vector2Int>();
    private int chunkOffsetX = 0;
    private int chunkOffsetZ = 0;
    private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
    public static ChunkManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        chunkOffsetX = numberOfChunksX / 2;
        chunkOffsetZ = numberOfChunksZ / 2;
    }
    //private float lowestWorldZ = float.MaxValue;
    //private float maxZ = float.MinValue;
    private void Update()
    {
        int newPlayerChunkX = -Mathf.RoundToInt((FloatingOrigin.Instance.GetOffsetPosition().z + player.position.z) / chunkSize);
        int newPlayerChunkZ = Mathf.RoundToInt((FloatingOrigin.Instance.GetOffsetPosition().x + player.position.x) / chunkSize);
        if (newPlayerChunkX != playerChunkX || newPlayerChunkZ != playerChunkZ)
        {
            playerChunkX = newPlayerChunkX;
            playerChunkZ = newPlayerChunkZ;
            //Debug.Log("Player moved to chunk: (" + playerChunkX + ", " + playerChunkZ + ")");
            desiredChunksList.Clear();
            for (int x = 0; x <= 1; x++)
            {
                for (int z = 0; z <= 1; z++)
                {
                    desiredChunksList.Add(new Vector2Int(playerChunkX + x, playerChunkZ + z));
                }
            }
            //Debug.Log("Desired chunks: " + string.Join(", ", desiredChunksList));
            foreach (var chunk in loadedChunks.Keys.ToList())
            {
                if (!desiredChunksList.Contains(chunk))
                {
                    //Debug.Log("Unloading chunk: " + chunk);
                    Destroy(loadedChunks[chunk]);
                    loadedChunks.Remove(chunk);
                }
            }
            foreach (var chunk in desiredChunksList)
            {
                float realX = chunk.x + chunkOffsetX;
                float realZ = chunk.y + chunkOffsetZ;
                //Debug.Log("Loading chunk: (" + realX + ", " + realZ + ")");
                if (!loadedChunks.ContainsKey(chunk))
                {
                    //Debug.Log("Loading chunk: " + chunk);
                    Vector3 floatingOriginOffset = FloatingOrigin.Instance.GetOffsetPosition();
                    Vector3 chunkPosition = new Vector3(
                        chunk.y * chunkSize - floatingOriginOffset.x - chunkSize,
                        chunkHeightOffset,
                        (-chunk.x + 1) * chunkSize - floatingOriginOffset.z - chunkSize
                    );
                    GameObject chunkObject = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
                    chunkObject.name = "Chunk_" + chunk.x + "_" + chunk.y;
                    Terrain terrain = chunkObject.GetComponent<Terrain>();
                    float x_ = chunk.x + chunkOffsetX;
                    float y_ = chunk.y + chunkOffsetZ;
                    x_ = ((x_ - 1) % numberOfChunksX + numberOfChunksX) % numberOfChunksX + 1;
                    y_ = ((y_ - 1) % numberOfChunksZ + numberOfChunksZ) % numberOfChunksZ + 1;
                    string path = Application.dataPath + "/Map/venusMapMod_" + x_ + "_" + y_ + ".raw";
                    //Debug.Log("Loading heightmap from: " + path);
                    float[,] heights = LoadRawHeightmap(path);
                    //Debug.Log("heights" + heights[0, 0] + " " + heights[heights.GetLength(0) - 1, heights.GetLength(1) - 1]);
                    TerrainData data = new TerrainData
                    {
                        heightmapResolution = resolution,
                        size = new Vector3(chunkSize, chunkHeight, chunkSize),
                    };

                    data.SetHeights(0, 0, heights);
                    data.terrainLayers = new TerrainLayer[]
                    {
                        rockLayer,
                        sandLayer,
                        grassLayer,
                        grassXrockLayer,
                        snowLayer
                    };
                    data.alphamapResolution = resolution;
                    float[,,] splatmap = new float[resolution, resolution, 5];
                    for (int z = 0; z < resolution; z++)
                    {
                        for (int x = 0; x < resolution; x++)
                        {
                            int heightX = Mathf.RoundToInt(
                                (float)x / (resolution - 1) * (heights.GetLength(1) - 1)
                            );

                            int heightZ = Mathf.RoundToInt(
                                (float)z / (resolution - 1) * (heights.GetLength(0) - 1)
                            );

                            float normalizedHeight = heights[heightZ, heightX];

                            float worldHeight = chunkPosition.y + normalizedHeight * chunkHeight;

                            float slope = data.GetSteepness((float)x / (resolution - 1), (float)z / (resolution - 1));
                            if (slope > stoneSlopeThreshold)
                            {
                                splatmap[z, x, 0] = 1f;
                                continue;
                            }
                            float worldZ = chunkPosition.z
                            + floatingOriginOffset.z
                            + (1f - (float)z / (resolution - 1)) * chunkSize;
                            /*if (worldZ < lowestWorldZ)
                            {
                                lowestWorldZ = worldZ;
                            }
                            if (worldZ > maxZ)
                            {
                                maxZ = worldZ;
                            }*/
                            float distanceFromEquator = Mathf.Abs(worldZ);
                            if (worldHeight > 0 && distanceFromEquator > equatorSnowTreshold)
                            {
                                splatmap[z, x, 4] = 1f;
                                continue;
                            }
                            switch (worldHeight)
                            {
                                case float h when h < sandHeight:
                                    splatmap[z, x, 0] = 1f;
                                    break;
                                case float h when h < grassHeight:
                                    splatmap[z, x, 1] = 1f;
                                    break;
                                case float h when h < grassXrockHeight:
                                    splatmap[z, x, 2] = 1f;
                                    break;
                                case float h when h < snowHeight:
                                    splatmap[z, x, 3] = 1f;
                                    break;
                                default:
                                    splatmap[z, x, 4] = 1f;
                                    break;
                            }
                        }
                    }
                    data.SetAlphamaps(0, 0, splatmap);
                    terrain.materialTemplate = material;
                    terrain.terrainData = data;

                    TerrainCollider collider = chunkObject.GetComponent<TerrainCollider>();
                    collider.terrainData = data;
                    loadedChunks[chunk] = chunkObject;
                }
            }
        }
        //Debug.Log("Lowest world Z: " + lowestWorldZ);
        //Debug.Log("Max world Z: " + maxZ);
    }
   // private float maximumHeight = 0f;
    float[,] LoadRawHeightmap(string path)
    {
        float[,] heights = new float[resolution, resolution];
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int index = (z * resolution + x) * 2;
                    if (index + 1 < bytes.Length)
                    {
                        ushort heightValue = (ushort)(bytes[index] | (bytes[index + 1] << 8));
                        heights[resolution - 1 - z, x] = heightValue / 65535f;
                        //maximumHeight = Mathf.Max(maximumHeight, heights[resolution - 1 - z, resolution - 1 - x]);
                    }
                }
            }
        }
        //Debug.Log("Maximum height in loaded heightmap: " + maximumHeight);
        return heights;
    }
    public float GetChunkSize()
    {
        return chunkSize;
    }

    // these two are flipped... fix later
    public int GetNumberOfChunksX()
    {
        return numberOfChunksZ;
    }
    public int GetNumberOfChunksZ()
    {
        return numberOfChunksX;
    }
}
