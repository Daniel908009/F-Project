using UnityEngine;

public class TorpedoScript : FloatingObject
{
    [SerializeField] private GameObject explosionPrefab;
    private float timeUntil = -1f;
    private float distanceTraveled = 0f;
    private float desiredAngle = 0f;
    private GameObject target = null;
    public void Init(float time, float angle)
    {
        timeUntil = time;
        desiredAngle = angle;
    }
    private GameObject FindTarget()
    {
        System.Collections.Generic.List<FloatingObject> floatingObjects = new System.Collections.Generic.List<FloatingObject>(EnemyManager.Instance.GetEnemyShipsFloatingObjects());
        floatingObjects.Add(SubmarineWaves.Instance.GetSubmarine().GetComponent<FloatingObject>());
        System.Collections.Generic.List<GameObject> targetPoints = new System.Collections.Generic.List<GameObject>();
        Transform[] targetPointsTemp;
        foreach (FloatingObject ship in floatingObjects)
        {
            targetPointsTemp = ship.GetTargetPoints();
            foreach (Transform point in targetPointsTemp)
            {
                if (point != null)
                {
                    targetPoints.Add(point.gameObject);
                }
            }
            
        } 
        Transform bestTarget = null;
        float smallestAngle = float.MaxValue;

        foreach (GameObject shipPoint in targetPoints)
        {
            Vector3 directionToShip = (shipPoint.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToShip);

            if (angle < smallestAngle)
            {
                smallestAngle = angle;
                bestTarget = shipPoint.transform;
            }
        }
        return bestTarget != null ? bestTarget.gameObject : null;
    }
    private void FixedUpdate()
    {
        if (distanceTraveled > 5000f)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
            return;
        }
        if (timeUntil != -1f)
        {
            timeUntil -= Time.fixedDeltaTime;
            if (timeUntil < 0f)
            {
                timeUntil = 0f;
            }
        }
        if (target != null)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            desiredAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, desiredAngle));
            if (angleDiff > 120f)
            {
                target = null;
            }
        }
        if (timeUntil == 0f && target == null)
        {
            target = FindTarget();
            //Debug.Log("Torpedo found target: " + (target != null ? target.name : "None"));
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
        distanceTraveled += currentSpeed * Time.fixedDeltaTime;
    }
    protected override void OnCollisionEnter(Collision collision)
    {
        DamageHandler damageHandler =
            collision.gameObject.GetComponentInParent<DamageHandler>();

        if (damageHandler != null)
        {
            damageHandler.Hit(transform.position, 15f, 1f);
        }

        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        explosion.SetActive(true);
        Destroy(gameObject);
    }
}
