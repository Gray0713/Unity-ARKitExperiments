using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneWaypointScript : MonoBehaviour {

	/*
	 * Each waypoint can have any number of neighbors.
	 * This is the list of the neighboring waypoints.
	 */
	public List<GameObject> waypointList = new List<GameObject>();

	/*
	 * Return a random neighbor of this waypoint.
     * If omit is specified, this one cannot be the one returned.
	 */
	public GameObject randomNeighbor(GameObject omit=null)
	{
        List<GameObject> localList;
        int sz = waypointList.Count; 
        if (omit != null && sz > 1)
        {
            localList = new List<GameObject>(waypointList);
            localList.Remove(omit);
        } else {
            localList = waypointList;
        }
		sz = localList.Count;
		int randomIndex;
		randomIndex = Random.Range (0, sz);
		return localList[randomIndex];
	}

	/*
	 * Search all the waypoints starting with the current one, return the one that 
	 * is closest to the given point.  This assumes that all waypoints are connected in a graph.
	 * The search must begin with one of the waypoints, but it can be any one.
	 */
	public GameObject NearestWaypoint(Vector3 p)
	{
		List<GameObject> visited = new List<GameObject>();
		// Call working function using this waypoint and the (empty) list of visited waypoints.
		return NearestWaypointInternal(p, gameObject, visited);
	}

	/*
	 * This recursive function is called only by NearestWaypoint().
	 * Find the distance to wp and mark wp as visited.
	 * Call the function recursively on each of this wp's neighbors if they have not been visited.
	 * Each of the above calls results in a waypoint being returned.
	 * If that returned waypoint is closer than those examined previously, it is the new 
	 * closest waypoint.
	 */
	private GameObject NearestWaypointInternal(Vector3 p, GameObject wp, List<GameObject> visited)
	{ 
		GameObject closestWP = wp;
		float shortestDistance = Vector3.Distance(p, wp.transform.position);
		visited.Add(wp); // Record that we have visited this waypoint.
		GameObject childWP;
		float      childDist;
        DroneWaypointScript drWPscr = wp.GetComponent<DroneWaypointScript>();
		for (int i=0; i<drWPscr.waypointList.Count; i++)
		{
			childWP = drWPscr.waypointList[i];
			if (!visited.Contains(childWP))
			{
				childWP = NearestWaypointInternal(p, childWP, visited);
				childDist = Vector3.Distance(p, childWP.transform.position);
				if (childDist < shortestDistance)
				{
					closestWP = childWP;
					shortestDistance = childDist;
				}
			}
		}
		return closestWP;
	}

	/*
	 * Return the nearest neighboring waypoint to point p, not including the
	 * current waypoint from which the function is called.  (Thus it is the nearest waypoint
	 * among this waypoint's neighbors.)
	 */
	public GameObject NearestNeighborToPoint(Vector3 p)
	{
		int sz = waypointList.Count;
		// Each waypoint should be connected to at least one other.
		if (sz <= 0) return null;
		int nearestIndex = 0;
		// Find the distance to the first neighbor.
		float nearestDistance = Vector3.Distance(p, waypointList[0].transform.position);
		float dist;
		for (int i=1; i<sz; i++)
		{
			dist = Vector3.Distance(p, waypointList[i].transform.position);
			if (dist < nearestDistance)
			{
				nearestIndex = i;
				nearestDistance = dist;
			}
		}
		return waypointList[nearestIndex];
	}

	/*
	 * Returns the neighbor of this waypoint where the line joining the current 
	 * waypoint with the neighbor has the shortest distance to point p.  This is useful
	 * when the flying drone has its movement constrained to traversing among the 
	 * waypoints.
	 */
	public GameObject NearestConstrainedNeighborToPoint(Vector3 p)
	{
		int sz = waypointList.Count;
		// Each waypoint should be connected to at least one other.
		if (sz <= 0) return null;
		// This should be very fast if the waypoint only has one neighbor.
		if (sz == 1)
		{
			return waypointList[0];
		}
		int nearestIndex = 0;
		// Find the distance to the first neighbor.
		Vector3 neighborPoint = GeometryClass.ClosestPointOnLine(transform.position, waypointList[0].transform.position, p, true);
		float nearestDistance = Vector3.Distance(p, neighborPoint);
		float dist;
		for (int i=1; i<sz; i++)
		{
			neighborPoint = GeometryClass.ClosestPointOnLine(transform.position, waypointList[i].transform.position, p, true);
			dist = Vector3.Distance(p, neighborPoint);
			if (dist < nearestDistance)
			{
				nearestIndex = i;
				nearestDistance = dist;
			}
		}
		return waypointList[nearestIndex];
	}
	
}
