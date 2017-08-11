using UnityEngine;
using System.Collections;

public class DroneMotionNone : DroneMotionStrategy {

	public override Vector3 AddRandomMotion(Vector3 nominalPos, float magnitude, float timeIncrement)
	{
		return nominalPos;
	}

}
