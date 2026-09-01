using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private RoomScript[] rooms = null;
    private void Awake()
    {
        rooms = GetComponentsInChildren<RoomScript>();
    }

    public void Hit(Transform tra, float damageDistance, float damageAmountAtCenter)
    {
        Debug.Log("DamageHandler Hit called");
        Debug.Log("Rooms found: " + rooms.Length);
        foreach (RoomScript room in rooms)
        {
            Vector3 closest = room.RoomCollider.ClosestPoint(tra.position);
            float distance = Vector3.Distance(tra.position, closest);
            Debug.Log("Room " + room.name + " closest point: " + closest + ", distance: " + distance);
            if (distance < damageDistance)
            {
                float damageAmount = Mathf.Lerp(damageAmountAtCenter, 0f, distance / damageDistance);
                room.SetDamageLevel(damageAmount);
                Debug.Log("Room " + room.name + " damaged by " + damageAmount + " at distance " + distance);
            }
        }
    }
}
