using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Add this component to any GameObject that should emit dynamic waypoints.
 */
public class DroneDynamicWaypointEmitter : MonoBehaviour {

    public float frequency = 0.5f;          // How often to emit a new waypoint (seconds).
    public float distanceThreshold = 5.0f;  // Don't emit a waypoint unless there is at least this much distance from the last one.
    public bool wpVisible=false;            // Whether or not the waypoints should be visible (for demos or debugging).
    public bool autoDelete = true;          // Whether the emitted waypoints should delete themselves after they expire.
    public float wpLifetime = 5.0f;         // How long emitted waypoints will exist before the delete themselves.
    public GameObject wpPrefab;             // Template prefab from which to create new dynamic waypoints.
	public Vector3 wpOffset = Vector3.zero; // Offset of waypoint from emitter.

    private List<GameObject> wpList;            // List of all the waypoints that have been emitted.
    private float   lastTimeEmitted = -99999f;  // Time when last waypoint was emitted.
    private Vector3 lastPositionEmitted = new Vector3(-99999f, -99999f, -99999f);  // Position of last emitted waypoint.

    void Start () {
		if (wpPrefab == null) {
			Debug.LogError (gameObject.name + " does not have wpPrefab set.");
		}
		wpList = new List<GameObject>();
	}

	void Update () {
		// Determine whether to emit a new waypoint...
		if (Time.time > lastTimeEmitted + frequency)
        {
            // Enough time has passed since previous waypoint.
            if (Vector3.Distance(transform.position, lastPositionEmitted) > distanceThreshold)
            {
                // Enough distance from previous waypoint.
				GameObject wp = (GameObject)Instantiate(wpPrefab, transform.position + wpOffset, transform.rotation);
				DroneDynamicWaypoint wpScript = wp.GetComponent<DroneDynamicWaypoint>();
				Debug.Assert (wpScript != null);
                wpScript.autoDelete = this.autoDelete;
                wpScript.expiration = Time.time + this.wpLifetime;
				wpScript.targetEmitter = this;
				MeshRenderer mr = wp.GetComponent<MeshRenderer>();
				Debug.Assert (mr != null);
				mr.enabled = wpVisible;
                wpList.Add(wp);
				AssertSequence ();
				//Debug.Log (gameObject.name + " emitted waypoint that expires at " + wpScript.expiration);
                lastTimeEmitted = Time.time;
                lastPositionEmitted = transform.position;
            }
        }
	}

	/*
	 * Traverse the waypoint list as a sanity check that they are in order of expiration time.
	 */
	private void AssertSequence() {
		float prevExpiration = 0;
		float thisExpiration;
		foreach (GameObject wp in wpList) {
			DroneDynamicWaypoint wpScript = wp.GetComponent<DroneDynamicWaypoint>();
			thisExpiration = wpScript.expiration;
			Debug.Assert (thisExpiration > prevExpiration);
			prevExpiration = thisExpiration;
		}
	}
		
	public void DeleteWaypointFromList(GameObject wp)
	{
		// We should always be removing the oldest waypoint.  Sanity check to make sure this is true.
		Debug.Assert(wp == wpList[0]);
		wpList.Remove(wp);
	}

	public List<GameObject> GetWaypointList() {
		return wpList;
	}
		
}
