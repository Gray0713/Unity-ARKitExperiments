using UnityEngine;
using System.Collections;

public class DroneTargetDirect : DroneTargetStrategy {

	public override void DroneTargetInit(FlyingDroneScript scr)
	{
	}

	public override Vector3 GetEffectiveTarget(Transform xform, Vector3 target, FlyingDroneScript scr)
	{
		return target;
	}

	public override void AdvanceWaypoints(GameObject waypointReached, Vector3 target, FlyingDroneScript scr)
	{
	}

}
