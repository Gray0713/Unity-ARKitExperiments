using UnityEngine;
using System.Collections;

public class DroneMotionSinusoidal : DroneMotionStrategy {

	private float timeCounter       = 0.0f;
	private float thisDelta         = 0.0f;
	private float lastDelta         = 0.0f;
    private float maxRadians        = 2f * Mathf.PI;

	public override Vector3 AddRandomMotion(Vector3 nominalPos, float magnitude, float timeIncrement)
	{
		float adjust = magnitude * Mathf.Sin (timeCounter);
		thisDelta = adjust - lastDelta;
		Vector3 retVal = nominalPos + Vector3.up*thisDelta;
		lastDelta = thisDelta;
		timeCounter += timeIncrement;
        if (timeCounter > maxRadians)
            timeCounter -= maxRadians;
		return (retVal);
	}

}
