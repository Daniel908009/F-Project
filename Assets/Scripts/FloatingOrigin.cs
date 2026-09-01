using UnityEngine;
using UnityEngine.SceneManagement;
public class FloatingOrigin : MonoBehaviour
{
    public Transform player;
    [SerializeField] private float threshold = 3000f;
    [SerializeField] private Vector3 initialOffset = Vector3.zero;
    private Vector3 offsetPosition = Vector3.zero;
    public static FloatingOrigin Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        offsetPosition = initialOffset;
        SubmarineWaves.Instance.ChangePositionByOffset(initialOffset);
    }
    //private bool test = false;
    private void FixedUpdate()
    {
        if (player.position.magnitude > threshold)
        {
            //if (test)
            //{
             //  return;
            //}
            //test = true;
            //Debug.Log("offsetting world by: " + player.position);
            Vector3 offset = new Vector3(player.position.x, 0f, player.position.z);
            CharacterController controller = player.GetComponent<CharacterController>();
            controller.enabled = false;
            //Debug.Log("Player position before offset: " + player.position);
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if(root.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody rb = root.GetComponent<Rigidbody>();
                    //Debug.Log("Applying offset to Rigidbody: " + root.name);
                    rb.position -= offset;
                }else{
                    root.transform.position -= offset;
                }
            }
            //Debug.Log("Floating origin applied.");
            controller.enabled = true;
            SubmarineWaves.Instance.ChangePositionByOffset(offset);
            //Debug.Log("offset " + offset);
            offsetPosition += new Vector3(offset.x, 0f, offset.z);
            Debug.Log("New offset position: " + offsetPosition);
            //Debug.Break();
            //bool test = false;
            if (offsetPosition.x > ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksX() / 2)
            {
                offsetPosition.x -= ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksX();
                //test = true;
            }
            else if (offsetPosition.x < -ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksX() / 2)
            {
                offsetPosition.x += ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksX();
                //test = true;
            }
            if (offsetPosition.z > ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksZ() / 2)
            {
                offsetPosition.z -= ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksZ();
                //test = true;
            }
            else if (offsetPosition.z < -ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksZ() / 2)
            {
                offsetPosition.z += ChunkManager.Instance.GetChunkSize() * ChunkManager.Instance.GetNumberOfChunksZ();
                //test = true;
            }
            //if (test){
              //  Debug.Log("New offset position after wrapping: " + offsetPosition);
            //}
            //Debug.Break();
        }
    }
    public Vector3 GetOffsetPosition()
    {
        return offsetPosition;
    }
}