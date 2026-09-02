using UnityEngine;
using System.Collections.Generic;

public enum ConnectionType
{
    EqualHeight,
    DownConnection,
    UpConnection
}
[System.Serializable]
public struct RoomConnection
{
    [SerializeField] public RoomScript room;
    [SerializeField] public DoorInteractable door;
    [SerializeField] public HatchInteractable hatch;
    [SerializeField] public bool hole;
    [SerializeField] public ConnectionType connectionType;
}

public class RoomScript : MonoBehaviour
{
    [SerializeField] private float floodLevel = 0f;
    [SerializeField] private bool hasWaterPlane = false;
    [SerializeField] private float damageLevel = 0f;
    private float floodingSpeed = 0f;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private RoomConnection[] roomConnections = null;
    public RoomConnection[] RoomConnections { get { return roomConnections; } }
    private float floodingValue = 0.1f;
    private BoxCollider roomCollider = null;
    public BoxCollider RoomCollider { get { return roomCollider; } } 
    private GameObject waterPlane = null;
    private void Awake()
    {
        roomCollider = GetComponent<BoxCollider>();

        if (!hasWaterPlane)
        {
            return;
        }    
        waterPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Destroy(waterPlane.GetComponent<MeshCollider>());
        waterPlane.name = "WaterPlane";
        waterPlane.GetComponent<Renderer>().material = blueMaterial;
        waterPlane.transform.SetParent(transform);

        waterPlane.transform.localPosition = Vector3.zero;
        waterPlane.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        waterPlane.transform.localScale = new Vector3(
            roomCollider.size.x / 10f,
            1f,
            roomCollider.size.z / 10f
        );
        waterPlane.SetActive(false);
    }

    private void Update()
    {
        floodingSpeed = damageLevel * floodingValue;
        floodingSpeed += SpeedFromConnections();
        floodingSpeed -= WaterPumps.Instance.PumpingSpeedInRoom();
        floodLevel += floodingSpeed * Time.deltaTime;
        floodLevel = Mathf.Clamp(floodLevel, 0f, 1f);
        if (!hasWaterPlane)
        {
            return;
        }
        if (floodLevel > 0 && Environment.Instance.playerInsideSub)
        {
            waterPlane.SetActive(true);
        }else
        {
            waterPlane.SetActive(false);
        }
        float top = roomCollider.size.y / 2f;
        float bottom = -roomCollider.size.y / 2f;
        float y = Mathf.Lerp(bottom, top, floodLevel);
        //Debug.Log("Top: " + top + " Bottom: " + bottom + " FloodLevel: " + floodLevel + " Y: " + y);
        //Debug.Break();
        waterPlane.transform.localPosition = new Vector3(0, y, 0);
    }
    private bool HoleInConnectionLine(RoomScript otherRoom, List<RoomScript> checkedRooms = null)
    {
        //Debug.Log("Checked room:" +checkedRooms[0].name);
        if (otherRoom.floodingSpeed > 0f)
        {
            //Debug.Log("this ran");
            return true;
        }
        List<RoomScript> cRooms = checkedRooms ?? new List<RoomScript>();
        if(!cRooms.Contains(this))
        {
            cRooms.Add(this);
        }
        //Debug.Log("Checking room: " + otherRoom.name);
        //return false;
        foreach (RoomConnection connection in otherRoom.RoomConnections)
        {
            if (connection.hatch != null && connection.hatch.IsHatchOpen() || connection.door != null && connection.door.IsDoorOpen() || connection.hole)
            {
                if (cRooms.Contains(connection.room))
                {
                    continue;
                }
                if (connection.room.HoleInConnectionLine(connection.room, cRooms))
                {
                    return true;
                }
            }
        }
        //Debug.Log("Checked rooms: " + cRooms.Count);
        //foreach (RoomScript room in cRooms)
        //{
        //    Debug.Log("Checked room: " + room.name);
        //}
        return false;
    }
    private float SpeedFromConnections()
    {
        float speed = 0f;
        foreach (RoomConnection connection in roomConnections)
        {
            if (connection.room == null)
            {
                continue;
            }
            float otherFloodLevel = connection.room.floodLevel;
            float otherFloodSpeed = connection.room.floodingSpeed;
            //Debug.Log("From: " + name + " Connection to room: " + connection.room.name + " FloodLevel: " + otherFloodLevel + " FloodSpeed: " + otherFloodSpeed);
            if (connection.hatch != null && connection.hatch.IsHatchOpen())
            {
                if (connection.connectionType == ConnectionType.UpConnection && otherFloodLevel > 0 && floodLevel < 1)
                {
                    speed += 1.2f *Time.deltaTime;
                }else if (connection.connectionType == ConnectionType.DownConnection && otherFloodLevel < 0.99)
                {
                    speed -= 1.2f *Time.deltaTime;
                }else if (connection.connectionType == ConnectionType.DownConnection && otherFloodLevel > 0.99 && HoleInConnectionLine(connection.room, new List<RoomScript>() {this, connection.room}))
                {
                    //Debug.Log("this ran");
                    speed += 1.2f *Time.deltaTime;
                }
            }
            else if (connection.door != null && connection.door.IsDoorOpen())
            {
                if (connection.connectionType == ConnectionType.EqualHeight)
                {
                    float floodDifference = otherFloodLevel - floodLevel;
                    speed += floodDifference * 0.5f;
                }
            }else if (connection.hole)
            {
                if (connection.connectionType == ConnectionType.DownConnection && otherFloodLevel < 0.99)
                {
                    speed -= 1.2f *Time.deltaTime;
                }else if (connection.connectionType == ConnectionType.DownConnection && otherFloodLevel > 0.99 && HoleInConnectionLine(connection.room, new List<RoomScript>() {this, connection.room}))
                {
                    speed += 1.2f *Time.deltaTime;
                }else if (connection.connectionType == ConnectionType.UpConnection && otherFloodLevel > 0 && floodLevel < 1)
                {
                    speed += 1.2f *Time.deltaTime;
                }
            }
        }
        return speed;
    }
    public void SetDamageLevel(float damageChange)
    {
        damageLevel += damageChange;
        if (damageLevel < 0f)
        {
            damageLevel = 0f;
        } else if (damageLevel > 1f)
        {
            damageLevel = 1f;
        }
    }
}
