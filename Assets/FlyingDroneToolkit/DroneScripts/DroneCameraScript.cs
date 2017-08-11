using UnityEngine;
using System.Collections;

public class DroneCameraScript : MonoBehaviour {

	public Transform target;
	public float height   = 1.0f;
	public float distance = 3.0f;
	public float damping  = 10.0f;

	void FixedUpdate () {

        transform.position = Vector3.Lerp(transform.position, target.position + height*target.up - distance*target.forward,
                                          damping * Time.deltaTime);

		transform.LookAt(target.position);
	}

}
