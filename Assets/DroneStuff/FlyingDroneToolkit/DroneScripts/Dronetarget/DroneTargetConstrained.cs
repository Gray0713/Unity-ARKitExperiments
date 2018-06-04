using UnityEngine;
using System.Collections;

public class DroneTargetConstrained : DroneTargetStrategy {

	public override void DroneTargetInit(FlyingDroneScript scr)
	{
		scr.UpdateClosestWaypoints();
	}
	
	public override Vector3 GetEffectiveTarget(Transform xform, Vector3 target, FlyingDroneScript scr)
	{
		Vector3 closePnt, closePntNext, closePntPrev, result;
		float delta = 1.0f;  // small distance for comparisons.
		/*
		 * Get the line from prev to next waypoint.
		 * Find the closest point of the target to that line.
		 * If that closest point is one of the endpoints of the line, 
         * that is the effective target.
		 * Otherwise check the current line and each adjacent line 
         * and determine which line is closest to target.
		 * If it is one of the other lines, the effective target is the common waypoint (next or prev).
		 * Otherwise, if the closest line is the current line, the effective target is the 
         * closest point on that line.
		 */
		GameObject nextWP = scr.nextWaypoint;
		GameObject prevWP = scr.prevWaypoint;
		Vector3 Pnext = nextWP.transform.position;
		Vector3 Pprev = prevWP.transform.position;
		DroneWaypointScript nextWPscr = nextWP.GetComponent<DroneWaypointScript>();
		DroneWaypointScript prevWPscr = prevWP.GetComponent<DroneWaypointScript>();

        // Is Pprev the effective target?  If so, return it.
		closePnt = GeometryClass.ClosestPointOnLine(Pprev, Pnext, target, true);
        if (Vector3.Distance(Pprev, target) < delta)
            result = Pprev;

        // Is Pnext the effective target?  If so, return it.
        else if (Vector3.Distance(Pnext, target) < delta)
            result = Pnext;

        else
        {

            // Get the distance of the target point to the current line.
            float distCurLine = Vector3.Distance(closePnt, target);

            // Get the line connected to the next WP that is closest to the target.
            GameObject closestWPtoNextWP = nextWPscr.NearestConstrainedNeighborToPoint(target);
            // Get the closest point on that line.
            closePntNext = GeometryClass.ClosestPointOnLine(Pnext, closestWPtoNextWP.transform.position, target, true);
            // Get the distance from that point to the target.
            float distNextLines = Vector3.Distance(closePntNext, target);

            // Get the line connected to the prev WP that is closest to the target.
            GameObject closestWPtoPrevWP = prevWPscr.NearestConstrainedNeighborToPoint(target);
            // Get the closest point on that line.
            closePntPrev = GeometryClass.ClosestPointOnLine(Pprev, closestWPtoPrevWP.transform.position, target, true);
            // Get the distance from that point to the target.
            float distPrevLines = Vector3.Distance(closePntPrev, target);

            // If the target is closer to a line adjecent to the next waypoint than to a line joining prev and 
            // next waypoints and to a line adjacent to the prev waypoint,
            // return the next waypoint.
            if (distNextLines < distCurLine && distNextLines < distPrevLines)
                result = Pnext;

            // If the target is closer to a line adjecent to the prev waypoint than to a line joining prev and 
            // next waypoints and to a line adjacent to the next waypoint,
            // return the prev waypoint.
            else if (distPrevLines < distCurLine && distPrevLines < distNextLines)
                result = Pprev;

            // If we have gotten this far, return the closest point on the line joining prev and next waypoints.
            else
                result = closePnt;
        }
        // Adjust result by heightAboveWaypoint.
        result.Set(result.x, result.y+scr.heightAboveWaypoint, result.z);
        return result;
	}
	
	public override void AdvanceWaypoints(GameObject waypointReached, Vector3 target, FlyingDroneScript scr)
	{
		scr.prevWaypoint = waypointReached;
		DroneWaypointScript wpScr = scr.prevWaypoint.GetComponent<DroneWaypointScript>();
		scr.nextWaypoint = wpScr.NearestConstrainedNeighborToPoint(target);
	}
	
}


