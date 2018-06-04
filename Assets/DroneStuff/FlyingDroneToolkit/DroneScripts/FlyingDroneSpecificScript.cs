using UnityEngine;
using System.Collections;

public abstract class FlyingDroneSpecificScript : MonoBehaviour {

    // Each specific flying drone must provide implementations of these functions.
    // See the scripts included in the Flying Drone Toolkit for examples.
	public abstract bool OnGround();
	public abstract float DistanceFromGround();
	public abstract float HeightOfGround();

	public void CastRayToGround(float rayStartInt, float rayLengthInt, float yScaleInt, 
        out bool isOnGroundInt, out float distFromGroundInt, out float heightOfGroundInt)
	{
		Vector3 origin = transform.position - transform.up*rayStartInt*yScaleInt;
		Vector3 dir    = -transform.up;
		float   dist   = rayLengthInt*yScaleInt;
		RaycastHit hit;
		bool hasGroundBeneath;
		Debug.DrawRay(origin, dist*dir, Color.green);
		isOnGroundInt    = Physics.Raycast(origin, dir, dist);
		hasGroundBeneath = Physics.Raycast(origin, dir, out hit);
		if (!hasGroundBeneath)
		{
			distFromGroundInt = Mathf.Infinity;
			heightOfGroundInt = Mathf.Infinity;
		}
		else
		{
			distFromGroundInt = hit.distance;
			heightOfGroundInt = hit.point.y;
		}
	}
	
}
