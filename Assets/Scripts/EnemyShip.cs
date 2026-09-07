using UnityEngine;

public class EnemyShip : FloatingObject
{
    private void FixedUpdate()
    {

        currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, Time.fixedDeltaTime * speedChangeSpeed);
        currentTurning = Mathf.MoveTowards(currentTurning, desiredTurning, Time.fixedDeltaTime * turningChangeSpeed);

        var (averageWaveHeight, posA, posB, posC, posD) = GetAverageHeight(samplePointA.position, samplePointB.position, samplePointC.position, samplePointD.position);

        Vector3 waterNormal = GetWaterNormal(posA, posB, posC, posD);
        rBody.MoveRotation(RotateFunction(waterNormal));

        Vector3 newPosition = rBody.position;

        float sinkingVariable = GetSinkingVariable();
        //Debug.Log("current depth: " + currentDepth + " sinking variable: " + sinkingVariable);
        currentDepth = Mathf.MoveTowards(currentDepth, desiredDepth, Time.fixedDeltaTime * depthChangeSpeed);
        currentDepth += Time.fixedDeltaTime * sinkingVariable;
        float targetY = Mathf.Lerp(
            rBody.position.y,
            averageWaveHeight - FloatingOffset - currentDepth,
            speedOfTargetYChange * Time.fixedDeltaTime);


        newPosition.y = Mathf.Lerp(
            newPosition.y,
            targetY,
            Time.fixedDeltaTime * 2f);

        newPosition += rBody.rotation * Vector3.forward 
                    * currentSpeed 
                    * Time.fixedDeltaTime;
        rBody.MovePosition(newPosition);
    }
}
