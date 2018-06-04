using UnityEngine;
using System.Collections;

public class DroneTargetWaypoint : DroneTargetStrategy {

	public override void DroneTargetInit(FlyingDroneScript scr)
	{
	}
	
	public override Vector3 GetEffectiveTarget(Transform xform, Vector3 target, FlyingDroneScript scr)
	{
        Vector3 eTarget = scr.nextWaypoint.transform.position;
        eTarget.y += scr.heightAboveWaypoint;
		return eTarget;
	}
	
	public override void AdvanceWaypoints(GameObject waypointReached, Vector3 target, FlyingDroneScript scr)
	{
		scr.RandomNextWaypoint();
	}
	
}
