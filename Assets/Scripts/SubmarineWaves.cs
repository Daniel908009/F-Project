using UnityEngine;

public class SubmarineWaves : FloatingObject
{
    private Vector3 lastTransform;
    
    private Quaternion lastRotation;

    [SerializeField] protected float fadeStartDistance = 0f;
    [SerializeField] protected float fadeEndDistance = -10f;

    public static SubmarineWaves Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        lastTransform = rBody.position;
        lastRotation = rBody.rotation;
    }
    private void FixedUpdate()
    {
        var (averageWaveHeight, posA, posB, posC, posD) = GetAverageHeight(samplePointA.position, samplePointB.position, samplePointC.position, samplePointD.position);

        if (PowerManager.Instance.IsPowered(PowerCircuit.EngineRoom))
        {
            currentDepth = Mathf.MoveTowards(currentDepth, desiredDepth, Time.fixedDeltaTime * depthChangeSpeed);
            currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, Time.fixedDeltaTime * speedChangeSpeed);
            currentTurning = Mathf.MoveTowards(currentTurning, desiredTurning, Time.fixedDeltaTime * turningChangeSpeed);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, Time.fixedDeltaTime * speedChangeSpeed);
            currentTurning = Mathf.MoveTowards(currentTurning, 0f, Time.fixedDeltaTime * turningChangeSpeed);
        }
        //Debug.Log("sinking variable: " + GetSinkingVariable());
        float sinkingVariable = GetSinkingVariable();
        if (!PowerManager.Instance.IsPowered(PowerCircuit.EngineRoom))
        {
            sinkingVariable -= 1f;
            if (sinkingVariable < 0f)
            {
                sinkingVariable = 0f;
            }
        }
        currentDepth += Time.fixedDeltaTime * sinkingVariable;

        float waveInfluence = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, -currentDepth);
        waveInfluence = Mathf.SmoothStep(0f, 1f, waveInfluence);
        
        Vector3 waterNormal = GetWaterNormal(posA, posB, posC, posD, waveInfluence);

        rBody.MoveRotation(RotateFunction(waterNormal));
        Quaternion currentRotation = rBody.rotation;
        Quaternion rotationDelta = currentRotation * Quaternion.Inverse(lastRotation);

        Vector3 newPosition = rBody.position;

        float targetY = Mathf.Lerp(
            rBody.position.y,
            averageWaveHeight * waveInfluence - FloatingOffset - currentDepth,
            speedOfTargetYChange * Time.fixedDeltaTime);
        //Debug.Log("submarine current depth: " + currentDepth + " sinking variable: " + sinkingVariable);

        newPosition.y = Mathf.Lerp(
            newPosition.y,
            targetY,
            Time.fixedDeltaTime * 2f);

        newPosition += rBody.rotation * Vector3.forward 
                    * currentSpeed 
                    * Time.fixedDeltaTime;
        //Debug.Log("new Position: " + newPosition);
        //Debug.Log("Submarine Position: " + rBody.position);
        rBody.MovePosition(newPosition); 

        PlayerMovement.Instance.ApplySubRotation(rotationDelta, lastTransform);

        lastRotation = currentRotation;
        Vector3 difference = rBody.position - lastTransform;
        //Debug.Log($"Submarine Position: {rBody.position}, Last Position: {lastTransform}, Difference: {difference}");
        PlayerMovement.Instance.MoveWithSub(difference);
        //Debug.Log($"Sub Movement from sub script: {difference}");
        lastTransform = rBody.position;
    }
    public void ChangePositionByOffset(Vector3 offset)
    {
        lastTransform -= offset;
    }
    public void ChangeDesiredDepth(float change)
    {
        desiredDepth += change;
        if (desiredDepth < 0f)
        {
            desiredDepth = 0f;
        }
    }
    public float GetDesiredDepth()
    {
        return desiredDepth;
    }
    public float GetCurrentDepth()
    {
        return currentDepth;
    }
    public float GetCurrentYRotation()
    {
        return rBody.rotation.eulerAngles.y;
    }
    public void ChangeDesiredSpeed(float change)
    {
        desiredSpeed += change;
        desiredSpeed = Mathf.Clamp(desiredSpeed, -maximumReverseSpeed, maximumSpeed);
    }
    public void ChangeTurning(float change)
    {
        desiredTurning += change;
        desiredTurning = Mathf.Clamp(desiredTurning, -maximumTurning, maximumTurning);
        desiredTurning = Mathf.Round(desiredTurning * 10f) / 10f;
    }
    public float GetDesiredSpeed()
    {
        return desiredSpeed;
    }
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    public float GetTurning()
    {
        return desiredTurning;
    }
    public GameObject GetSubmarine()
    {
        return this.gameObject;
    }
}
