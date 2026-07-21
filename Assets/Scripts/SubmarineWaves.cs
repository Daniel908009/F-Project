using UnityEngine;

public class SubmarineWaves : MonoBehaviour
{
    [SerializeField] private Transform samplePointA;
    [SerializeField] private Transform samplePointB;
    [SerializeField] private Transform samplePointC;
    [SerializeField] private Transform samplePointD;
    private Vector3 lastTransform;
    [SerializeField] private Rigidbody submarineRigidbody;
    [SerializeField] private float SubOffset = 3.5f;
    [SerializeField] private float tiltStrength = 0.4f;
    [SerializeField] private float fadeStartDistance = 0f;
    [SerializeField] private float fadeEndDistance = -10f;

    [SerializeField] private float desiredDepth = 0f;
    [SerializeField] private float currentDepth = 0f;
    [SerializeField] private float depthChangeSpeed = 1f;

    [SerializeField] private float desiredSpeed = 0f;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float maximumSpeed = 10f;
    [SerializeField] private float maximumReverseSpeed = 5f;
    [SerializeField] private float speedChangeSpeed = 1f;

    [SerializeField] private float desiredTurning = 0f;
    [SerializeField] private float currentTurning = 0f;
    [SerializeField] private float maximumTurning = 1f;
    [SerializeField] private float turningChangeSpeed = 1f;
    
    private Quaternion lastRotation;

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
        lastTransform = transform.position;
        lastRotation = transform.rotation;
    }
    private void Update()
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

        
        //Debug.Log($"Wave Influence: {waveInfluence}, Fade Start: {fadeStartDistance}, Fade End: {fadeEndDistance}, Submarine Y: {transform.position.y}");

        currentDepth = Mathf.MoveTowards(currentDepth, desiredDepth, Time.deltaTime * depthChangeSpeed);
        currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, Time.deltaTime * speedChangeSpeed);

        float waveInfluence = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, -currentDepth);
        waveInfluence = Mathf.SmoothStep(0f, 1f, waveInfluence);
        //Debug.Log("waveInfluence: " + waveInfluence);
        
        Vector3 newPosition = transform.position;
        float targetY = Mathf.Lerp(
                        transform.position.y,
                        averageWaveHeight - SubOffset - currentDepth,
                        waveInfluence);
        newPosition.y = Mathf.Lerp(newPosition.y, targetY, Time.deltaTime * 2f);
        newPosition += submarineRigidbody.rotation * Vector3.forward * currentSpeed * Time.deltaTime;
        submarineRigidbody.MovePosition(newPosition);

        Vector3 front = (posA + posB) * 0.5f;
        Vector3 back = (posC + posD) * 0.5f;
        Vector3 forward = (front - back).normalized;

        Vector3 right = (posB + posC) * 0.5f;
        Vector3 left = (posA + posD) * 0.5f;
        Vector3 rightDir = (right - left).normalized;

        Vector3 waterNormal = Vector3.Cross(forward, rightDir).normalized;

        waterNormal = Vector3.Slerp(Vector3.up, waterNormal, tiltStrength * waveInfluence).normalized;
        //Debug.Log($"Water Normal: {waterNormal}, Forward: {forward}, RightDir: {rightDir}");
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, waterNormal) * transform.rotation;
        submarineRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f));
        Quaternion currentRotation = submarineRigidbody.rotation;
        Quaternion rotationDelta = currentRotation * Quaternion.Inverse(lastRotation);

        PlayerMovement.Instance.ApplySubRotation(rotationDelta, transform.position);

        lastRotation = currentRotation;
        Vector3 difference = transform.position - lastTransform;
        PlayerMovement.Instance.MoveWithSub(difference);
        //Debug.Log($"Sub Movement from sub script: {difference}");
        lastTransform = transform.position;
    }
    public void ChangeDesiredDepth(float change)
    {
        desiredDepth += change;
        desiredDepth = Mathf.Clamp(desiredDepth, 0f, float.PositiveInfinity);
    }
    public float GetDesiredDepth()
    {
        return desiredDepth;
    }
    public float GetCurrentDepth()
    {
        return currentDepth;
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
}
