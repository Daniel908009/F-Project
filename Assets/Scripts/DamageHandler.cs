using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private RoomScript[] rooms = null;
    public RoomScript[] Rooms { get { return rooms; } }
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float timeToHullDestruction = 15f;
    private void Awake()
    {
        rooms = GetComponentsInChildren<RoomScript>();
    }

    public void Hit(Vector3 vector, float damageDistance, float damageAmountAtCenter)
    {
        //Debug.Log("DamageHandler Hit called");
        //Debug.Log("Vector: " + vector);
        foreach (RoomScript room in rooms)
        {
            //Debug.Log("Checking room: " + room.name + " roomCollider: " + room.RoomCollider);
            Vector3 closest = room.RoomCollider.ClosestPoint(vector);
            float distance = Vector3.Distance(vector, closest);
            //Debug.Log("Room " + room.name + " closest point: " + closest + ", distance: " + distance);
            if (distance < damageDistance)
            {
                float damageAmount = Mathf.Lerp(damageAmountAtCenter, 0f, distance / damageDistance);
                room.SetDamageLevel(damageAmount);
                //Debug.Log("Room " + room.name + " damaged by " + damageAmount + " at distance " + distance);
            }
        }
    }
    private void Update()
    {
        float currentFlooding = 0f;
        foreach (RoomScript room in rooms)
        {
            currentFlooding += room.FloodLevel;
        }
        float floodingFraction = currentFlooding / rooms.Length;

        float damagePerSecond = 100f / timeToHullDestruction * floodingFraction;

        float damageThisUpdate = damagePerSecond * Time.deltaTime;

        FloatingObject floatingObject = GetComponent<FloatingObject>();
        if (floatingObject != null)
        {
            floatingObject.HullHealth -= damageThisUpdate;
            //Debug.Log("Hull health: " + floatingObject.HullHealth);
            if (floatingObject.CurrentDepth > floatingObject.MaxSafeDepth)
            {
                float depthExcess = floatingObject.CurrentDepth - floatingObject.MaxSafeDepth;
                float damageFromDepth = depthExcess * 0.01f * Time.deltaTime;
                foreach (RoomScript room in rooms)
                {
                    room.SetDamageLevel(damageFromDepth);
                }
            }
            if (floatingObject.HullHealth <= 0f)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
                explosion.SetActive(true);
                if (floatingObject is SubmarineWaves)
                {
                    SubmarineWaves.Instance.enabled = false;
                }
                else if (floatingObject is EnemyShip)
                {
                    EnemyShip enemyShip = floatingObject as EnemyShip;
                    EnemyManager.Instance.RemoveEnemyShip(enemyShip);
                }
                Destroy(gameObject);
            }
        }
    }
}
