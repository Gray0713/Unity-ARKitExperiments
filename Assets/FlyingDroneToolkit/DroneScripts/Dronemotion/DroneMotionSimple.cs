using UnityEngine;
using System.Collections;

public class DroneMotionSimple : DroneMotionStrategy {
    private float thisDelta = 0.0f;
    private float incrSign = 1f;

    public override Vector3 AddRandomMotion(Vector3 nominalPos, float magnitude, float timeIncrement)
    {
        thisDelta += incrSign * timeIncrement;
        Vector3 retVal = nominalPos + Vector3.up * thisDelta;
        if ((incrSign*thisDelta) > magnitude)
            incrSign *= -1f;
        return (retVal);
    }
}
