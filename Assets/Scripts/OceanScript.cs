using UnityEngine;

public class OceanScript : MonoBehaviour
{    
    [SerializeField] private GameObject FollowObject;
    //[SerializeField] private float tileSize = 500f;
   // private int playerTileX = int.MaxValue;
   // private int playerTileZ = int.MaxValue;
    public static OceanScript Instance { get; private set; }
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
        /*int newPlayerTileX = Mathf.RoundToInt(player.position.x / tileSize);
        int newPlayerTileZ = Mathf.RoundToInt(player.position.z / tileSize);
        if (newPlayerTileX != playerTileX || newPlayerTileZ != playerTileZ)
        {
            transform.position = new Vector3(
                newPlayerTileX * tileSize,
                0f,
                newPlayerTileZ * tileSize
            );
            playerTileX = newPlayerTileX;
            playerTileZ = newPlayerTileZ; 
        }*/
        transform.position = new Vector3(FollowObject.transform.position.x, 0f, FollowObject.transform.position.z);
        //Debug.DrawLine(new Vector3(player.position.x, 0f, player.position.z), new Vector3(player.position.x, WaveScript.Instance.CalculateWave(player.position).y, player.position.z), Color.red);
    }
}
