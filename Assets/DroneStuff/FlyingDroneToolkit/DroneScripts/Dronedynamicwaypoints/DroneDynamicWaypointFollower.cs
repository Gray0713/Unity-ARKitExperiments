using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneDynamicWaypointFollower : MonoBehaviour {

    public float castRadius=1f;         // Radius of a sphere surrounding the drone, to test whether the drone has a clear path to its target.
	public bool drawRayCasts = true; // Whether to draw the raycasts in the Scene View, for debugging.
	private RaycastHit hitInfo;

	void Start () {
		if (LayerMask.LayerToName (gameObject.layer) != "Ignore Raycast")
			Debug.LogWarning ("It is recommended that a drone that follows dynamic waypoints should be in the 'Ignore Raycast' layer, but " + gameObject.name + " is not.");
	}


	/*
	 * This function detemines whether one of the dynamic waypoints should be used as the target rather than the true target.
	 */
    public Vector3 GetTargetPos(GameObject realTarget)
    {
		Vector3 realTargetPos = realTarget.transform.position;
        // Cast a sphere from the drone to the target to determine whether there are any occlusions.
        if (TargetOccluded(realTargetPos))
        {
            // Target is occluded.  Examine waypoints, starting with the newest one.
			//Debug.Log ("Target is occluded");
            // Start by getting the list of dynamic waypoints.
			DroneDynamicWaypointEmitter emitter = realTarget.GetComponent<DroneDynamicWaypointEmitter>();
			if (emitter == null) {
				Debug.LogError ("DroneDynamicWaypointFollower.GetTargetPos():  Supplied realTarget is not a waypoint emitter.");
				return realTargetPos;
			}
			List<GameObject> wpList = emitter.GetWaypointList();
 			if (wpList == null) {
				Debug.LogError ("DroneDynamicWaypointFollower.GetTarget():  No waypoint list.");
				return realTargetPos;
			}
            // For each dynamic waypoint, in order of newness...
			for (int i = wpList.Count-1; i>=0; i--) {
				GameObject wp = wpList [i];
				Vector3 wpPos = wp.transform.position;
            	// If this waypoint is not occluded, it will be an intermediate waypoint to help us later get to the real target.  Return it.
				//DroneDynamicWaypoint wpScript = wp.GetComponent<DroneDynamicWaypoint> ();
				if (!TargetOccluded (wpPos)) {
					//Debug.Log("Waypoint " + wpScript.expiration + " is NOT occluded");
					return wpPos;
				} else {
					//Debug.Log ("Waypoint " + wpScript.expiration + " is occluded");
				}
			}
        }
		// At this point, either the target is not occluded (good) or the target and all waypoints are occluded (bad).
		// Either way, return the original target.
		return realTargetPos;
    }
		
    private bool TargetOccluded(Vector3 targetPos)
    {
		// Create a layerMask to exclude the 'Ignore Raycast' layer (Builtin Layer 2).
		int layerMask = 1 << 2;  
		layerMask = ~layerMask;
		// Ray from current position to the target's position (which may be a waypoint or the real target).
        Vector3 fromTo = targetPos - transform.position;
		bool isOccluded = Physics.SphereCast(transform.position, castRadius, fromTo.normalized, out hitInfo, fromTo.magnitude, layerMask);
		if (drawRayCasts) {
			Color lineColor;
			if (isOccluded)
				lineColor = Color.cyan;  // Cyan line shows occluded paths from follower to target or waypoints.  
			else                         // Not all unsuccessful lines are shown, because search stops when a non-occluded path is found.
				lineColor = Color.red;   // Red line shows the direct line from follower to the target or waypoint that is not occluded.
			Debug.DrawLine (transform.position, targetPos, lineColor, 1f);
			// ALso draw a frame to show the far end of the cast sphere.
			Debug.DrawLine(targetPos-(Vector3.forward*castRadius), targetPos+(Vector3.forward*castRadius), lineColor, 1f);
			Debug.DrawLine(targetPos-(Vector3.up     *castRadius), targetPos+(Vector3.up     *castRadius), lineColor, 1f);
			Debug.DrawLine(targetPos-(Vector3.left   *castRadius), targetPos+(Vector3.left   *castRadius), lineColor, 1f);
		}
		return isOccluded;
    }

}
