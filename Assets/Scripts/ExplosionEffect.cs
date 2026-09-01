using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    private ParticleSystem ps = null;
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
}
