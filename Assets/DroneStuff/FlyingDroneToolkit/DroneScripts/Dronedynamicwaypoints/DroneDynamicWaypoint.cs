using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneDynamicWaypoint : MonoBehaviour {
	
    // These variables are public so that they can be set by the emitter.  They should not be changed by the user. 
    public bool autoDelete;    // Whether the waypoint should automatically delete after it expires.
    public float expiration;   // The time at which the waypoint automatically deletes, in seconds since game start-up.
    public DroneDynamicWaypointEmitter targetEmitter; // The target's waypoint emitter component.

	void Start() {
		Debug.Assert (targetEmitter != null);
	}

	void Update () {
	    if (autoDelete && Time.time > expiration)
        {
			targetEmitter.DeleteWaypointFromList(gameObject);
			Destroy (gameObject);
        }
    }

}
