using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private RoomScript[] rooms = null;
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
}
