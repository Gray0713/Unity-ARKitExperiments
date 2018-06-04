using UnityEngine;
using System.Collections;

public abstract class DroneTargetStrategy : ScriptableObject {
    // Any initialization that may be required for effective target calculations.
	public abstract void    DroneTargetInit(FlyingDroneScript scr);
    // Return the "effective target" based on the actual target.  When motion is constrained, the
    // effective target may be different.
	public abstract Vector3 GetEffectiveTarget(Transform xform, Vector3 target, FlyingDroneScript scr);
    // Advance waypoints for patrolling or constrained following.
	public abstract void    AdvanceWaypoints(GameObject waypointReaached, Vector3 target, FlyingDroneScript scr);
}
