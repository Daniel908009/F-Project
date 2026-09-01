using UnityEngine;

public class TorpedoTube : MonoBehaviour
{
    private bool isFlooded = false;
    public bool IsFlooded
    {
        get { return isFlooded; }
        set { isFlooded = value; }
    }
    private GameObject torpedoPrefab;
    public GameObject TorpedoPrefab
    {
        get { return torpedoPrefab; }
        set { torpedoPrefab = value; }
    }
}
