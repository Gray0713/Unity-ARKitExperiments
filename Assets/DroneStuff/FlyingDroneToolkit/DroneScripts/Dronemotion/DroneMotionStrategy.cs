using UnityEngine;
using System.Collections;

public abstract class DroneMotionStrategy : ScriptableObject {

	public abstract Vector3 AddRandomMotion(Vector3 nominalPos, float magnitude, float timeIncrement);

}
