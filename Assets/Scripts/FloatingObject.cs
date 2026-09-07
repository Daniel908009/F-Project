using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [SerializeField] protected Rigidbody rBody;
    [SerializeField] protected Transform samplePointA; 
    [SerializeField] protected Transform samplePointB;
    [SerializeField] protected Transform samplePointC;
    [SerializeField] protected Transform samplePointD;

    [SerializeField] protected Transform[] targetPoints;

    [SerializeField] protected float desiredSpeed = 0f;
    [SerializeField] protected float currentSpeed = 0f;
    public float CurrentSpeed => currentSpeed;
    [SerializeField] protected float maximumSpeed = 10f;
    public float MaximumSpeed => maximumSpeed;
    [SerializeField] protected float maximumReverseSpeed = 5f;
    [SerializeField] protected float speedChangeSpeed = 1f;
    [SerializeField] protected float currentDepth = 0f;
    public float CurrentDepth => currentDepth;
    [SerializeField] protected float desiredDepth = 0f;
    [SerializeField] protected float depthChangeSpeed = 1f;
    [SerializeField] protected float speedOfTargetYChange = 10f;
    [SerializeField] protected float maxSafeDepth = 1000f;
    public float MaxSafeDepth => maxSafeDepth;

    [SerializeField] protected float desiredTurning = 0f;
    [SerializeField] protected float currentTurning = 0f;
    [SerializeField] protected float maximumTurning = 1f;
    [SerializeField] protected float turningChangeSpeed = 1f;

    [SerializeField] protected float FloatingOffset = 3.5f;
    [SerializeField] protected float tiltStrength = 0.4f;

    [SerializeField] protected float InteractionWidthValue = 0.0125f;
    public float InteractionWidth => InteractionWidthValue;
    [SerializeField] protected float InteractionLengthValue = 0.05f;
    public float InteractionLength => InteractionLengthValue;
    [SerializeField] protected float WakeLengthValue = 0.3f;
    public float WakeLength => WakeLengthValue;
    [SerializeField] protected float WakeAngleValue = 0.5f;
    public float WakeAngle => WakeAngleValue;
    [SerializeField] protected float WakeEdgeSharpnessValue = 5f;
    public float WakeEdgeSharpness => WakeEdgeSharpnessValue;
    [SerializeField] protected float MaxWakeValue = 10f;
    public float MaxWake => MaxWakeValue;
    [SerializeField] protected Transform hullValue;
    public Transform Hull => hullValue;
    [SerializeField] protected Transform WakePositionValue;
    public Transform WakePosition => WakePositionValue;
    [SerializeField] protected float numberOfRoomsToSinking = 3f;

    protected float hullHealth = 100f;
    public float HullHealth
    {
        get { return hullHealth; }
        set { hullHealth = value; }
    }

    protected (float, Vector3, Vector3, Vector3, Vector3) GetAverageHeight(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD)
    {
        float averageWaveHeight = 0f;
        Vector3 posA = samplePointA.position;
        Vector3 posB = samplePointB.position;
        Vector3 posC = samplePointC.position;
        Vector3 posD = samplePointD.position;
        posA.y = WaveScript.Instance.CalculateWave(posA).y;
        posB.y = WaveScript.Instance.CalculateWave(posB).y;
        posC.y = WaveScript.Instance.CalculateWave(posC).y;
        posD.y = WaveScript.Instance.CalculateWave(posD).y;
        averageWaveHeight += posA.y + posB.y + posC.y + posD.y;
        averageWaveHeight /= 4f;
        return (averageWaveHeight, posA, posB, posC, posD);
    }

    protected Vector3 GetWaterNormal(Vector3 posA, Vector3 posB, Vector3 posC, Vector3 posD, float waveInfluence = 1f)
    {
        Vector3 front = (posA + posB) * 0.5f;
        Vector3 back = (posC + posD) * 0.5f;
        Vector3 forward = (front - back).normalized;

        Vector3 right = (posB + posC) * 0.5f;
        Vector3 left = (posA + posD) * 0.5f;
        Vector3 rightDir = (right - left).normalized;

        Vector3 waterNormal = Vector3.Cross(forward, rightDir).normalized;
        waterNormal = Vector3.Slerp(Vector3.up, waterNormal, tiltStrength * waveInfluence).normalized;
        return waterNormal;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Terrain>() != null)
        {
            //Debug.Log("Hit terrain!");
            desiredSpeed = 0f;
            desiredTurning = 0f;
            currentSpeed = 0f;
            currentTurning = 0f;
            desiredDepth = currentDepth-5;
            if (desiredDepth < 0f)
            {
                desiredDepth = 0f;
            }
            this.GetComponent<DamageHandler>()?.Hit(collision.contacts[0].point, 20f, 1f);
        }
    }

    protected Quaternion RotateFunction(Vector3 waterNormal)
    {
        float speedMultiplier = Mathf.Lerp(
        0.2f,
        1f,
        Mathf.InverseLerp(0f, maximumSpeed, Mathf.Abs(currentSpeed)));
        float turnThisFrame = currentTurning * speedMultiplier * Time.fixedDeltaTime;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, waterNormal) * rBody.rotation;
        Quaternion turnRotation = Quaternion.AngleAxis(turnThisFrame * Mathf.Rad2Deg, Vector3.up);
        targetRotation = turnRotation * targetRotation;
        return Quaternion.Slerp(rBody.rotation, targetRotation, Time.fixedDeltaTime * 2f);
    }
    public Transform[] GetTargetPoints()
    {
        return targetPoints;
    }
    public float GetSinkingVariable()
    {
        DamageHandler damageHandler = GetComponent<DamageHandler>();
        if (damageHandler == null)
        {
            return 0f;
        }
        RoomScript[] rooms = damageHandler.Rooms;
        if (rooms == null || rooms.Length == 0){
            return 0f;
        }
        float currentFlooding = 0f;
        foreach (RoomScript room in rooms)
        {
            currentFlooding += room.FloodLevel;
        }
        float totalFlood = rooms.Length;

        float sinkingVariable = 1f - (currentFlooding / numberOfRoomsToSinking);

        if (sinkingVariable < 0f)
        {
            sinkingVariable =
                -(currentFlooding - numberOfRoomsToSinking) /
                (totalFlood - numberOfRoomsToSinking);
        }

        //Debug.Log("Sinking variable: " + sinkingVariable + " currentFlooding: " + currentFlooding + " numberOfRoomsToSinking: " + numberOfRoomsToSinking + " totalFlood: " + totalFlood);
        return Mathf.Clamp(1f - sinkingVariable, 0f, 2f);
    }
}
