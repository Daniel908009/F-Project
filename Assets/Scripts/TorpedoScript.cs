using UnityEngine;

public class TorpedoScript : FloatingObject
{
    [SerializeField] private GameObject explosionPrefab;
    private float timeUntil = 0f;
    private float desiredAngle = 0f;
    private GameObject target;
    public void Init(float time, float angle)
    {
        timeUntil = time;
        desiredAngle = angle;
    }
    private GameObject FindTarget()
    {
        System.Collections.Generic.List<FloatingObject> floatingObjects = new System.Collections.Generic.List<FloatingObject>(EnemyManager.Instance.GetEnemyShipsFloatingObjects());
        floatingObjects.Add(SubmarineWaves.Instance.GetSubmarine().GetComponent<FloatingObject>());
        Transform bestTarget = null;
        float smallestAngle = float.MaxValue;

        foreach (FloatingObject ship in floatingObjects)
        {
            Vector3 directionToShip = (ship.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToShip);

            if (angle < smallestAngle)
            {
                smallestAngle = angle;
                bestTarget = ship.transform;
            }
        }
        return bestTarget != null ? bestTarget.gameObject : null;
    }
    private void FixedUpdate()
    {
        timeUntil -= Time.fixedDeltaTime;
        if (timeUntil <= 0f)
        {
            target = FindTarget();
            if (target != null)
            {
                Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
                desiredAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
                //Debug.Log("Torpedo target acquired: " + target.name + " at angle: " + desiredAngle);
            }
        }
        currentSpeed = desiredSpeed;
        
        float currentAngle = transform.eulerAngles.y;
        float angleDifference = Mathf.DeltaAngle(currentAngle, desiredAngle);
        if (Mathf.Abs(angleDifference) > 0.1f)
        {
            float turnDirection = Mathf.Sign(angleDifference);
            currentTurning = turnDirection * turningChangeSpeed;
        }
        else
        {
            currentTurning = 0f;
        }

        var (averageWaveHeight, posA, posB, posC, posD) = GetAverageHeight(samplePointA.position, samplePointB.position, samplePointC.position, samplePointD.position);

        Vector3 waterNormal = GetWaterNormal(posA, posB, posC, posD);
        rBody.MoveRotation(RotateFunction(waterNormal));

        float offset;
        if (averageWaveHeight < -FloatingOffset)
        {
            offset = averageWaveHeight;
        }
        else
        {
            offset = -FloatingOffset;
        }
        Vector3 newPosition = rBody.position;
        float targetY = Mathf.Lerp(
            rBody.position.y,
            offset,
            0.2f);

        newPosition.y = Mathf.Lerp(
            newPosition.y,
            targetY,
            Time.fixedDeltaTime * 2f);

        newPosition += rBody.rotation * Vector3.forward 
                    * currentSpeed 
                    * Time.fixedDeltaTime;
        rBody.MovePosition(newPosition); 
    }
    protected override void OnCollisionEnter(Collision collision)
    {
        DamageHandler damageHandler =
            collision.gameObject.GetComponentInParent<DamageHandler>();

        if (damageHandler != null)
        {
            damageHandler.Hit(transform, 15f, 1f);
        }

        Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
