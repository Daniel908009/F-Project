using UnityEngine;
[System.Serializable] public struct TorpedoType
{
    public string Name;
    public GameObject Prefab;
}
public class TorpedoLauncher : MonoBehaviour
{
    [SerializeField] private TorpedoTube[] torpedoTubes;
    [SerializeField] private TorpedoType[] torpedoTypes;
    public TorpedoType[] TorpedoTypes
    {
        get { return torpedoTypes; }
        set { torpedoTypes = value; }
    }
    public TorpedoTube[] TorpedoTubes
    {
        get { return torpedoTubes; }
        set { torpedoTubes = value; }
    }
    private float selectedTubeIndex = 0;
    public static TorpedoLauncher Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void ChangeSelectedTubeIndex(float newIndex)
    {
        if (newIndex >= 0 && newIndex < torpedoTubes.Length)
        {
            selectedTubeIndex = newIndex;
        }
        //Debug.Log("Selected Tube Index: " + selectedTubeIndex);
    }
    public void FloodSelectedTube(System.Action<float> updateSelectedTubeImagesCallback)
    {
        if (!PowerManager.Instance.IsPowered(PowerCircuit.TorpedoSystem))
        {
            return;
        }
        bool flood = !torpedoTubes[(int)selectedTubeIndex].IsFlooded;
        torpedoTubes[(int)selectedTubeIndex].IsFlooded = flood;
        //Debug.Log("Tube " + selectedTubeIndex + " flooded: " + flood);
        updateSelectedTubeImagesCallback(selectedTubeIndex);
    }
    public void LoadTorpedoIntoSelectedTube(int torpedoTypeIndex, System.Action<float> updateSelectedTubeImagesCallback)
    {
        if (torpedoTubes[(int)selectedTubeIndex].IsFlooded || !PowerManager.Instance.IsPowered(PowerCircuit.TorpedoSystem))
        {
            //Debug.Log("Cannot load torpedo into tube " + selectedTubeIndex + " because it is flooded.");
            return;
        }
        GameObject torpedoPrefab = null;
        if (torpedoTypeIndex >= 0 && torpedoTypeIndex < torpedoTypes.Length)
        {
            torpedoPrefab = torpedoTypes[torpedoTypeIndex].Prefab;
        }
        torpedoTubes[(int)selectedTubeIndex].TorpedoPrefab = torpedoPrefab;
        //Debug.Log("Loaded torpedo into tube " + selectedTubeIndex);
        updateSelectedTubeImagesCallback(selectedTubeIndex);
    }
    public void FireTorpedoFromSelectedTube(float time, float angle, System.Action<float> updateSelectedTubeImagesCallback)
    {
        TorpedoTube selectedTube = torpedoTubes[(int)selectedTubeIndex];
        if (selectedTube.IsFlooded && selectedTube.TorpedoPrefab != null && PowerManager.Instance.IsPowered(PowerCircuit.TorpedoSystem))
        {
            GameObject torpedoInstance = Instantiate(selectedTube.TorpedoPrefab, selectedTube.transform.position, selectedTube.transform.rotation);
            torpedoInstance.GetComponent<TorpedoScript>().Init(time, angle);
            selectedTube.TorpedoPrefab = null;
            //Debug.Log("Fired torpedo from tube " + selectedTubeIndex);
        }
        updateSelectedTubeImagesCallback(selectedTubeIndex);
    }
}
