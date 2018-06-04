using UnityEngine;
using System.Collections;

/*
 * The GeometryClass provides some functions that support the flying drones.
 */

public class GeometryClass : ScriptableObject {

	// Get the closest point on the line segment AB to point P.  If segmentClamp == true
	// the point must be on the line segment.
	// This is from http://www.gamedev.net/topic/444154-closest-point-on-a-line/.
	public static Vector3 ClosestPointOnLine(Vector3 A, Vector3 B, Vector3 P, bool segmentClamp)
	{    
		Vector3 AP = P - A;
		Vector3 AB = B - A;
		float ab2 = AB.x*AB.x + AB.y*AB.y + AB.z*AB.z;
		float ap_ab = AP.x*AB.x + AP.y*AB.y + AP.z*AB.z;
		float t = ap_ab / ab2;
		if (segmentClamp)
		{    
			if (t < 0.0f) t = 0.0f;
			else if (t > 1.0f) t = 1.0f;  
		}    
		Vector3 Closest = A + AB * t;    
		return Closest;
	}

	public static Vector3 TargetSameHeight(Vector3 nominalPos, Vector3 targetPos)
	{
		Vector3 newTarget = targetPos;
		newTarget.y = nominalPos.y;
		return newTarget;
	}

	public static float DotToTarget(Transform thisXform, Vector3 targetPos)
	{
		Vector3 nominalPos = thisXform.position;
		Vector3 DirToTarget = (targetPos - nominalPos).normalized;
		Vector3 ForwardDir = thisXform.forward.normalized;
		return Vector3.Dot(ForwardDir, DirToTarget);
	}

    // In the following, calculate the dot as though all is at the same height.
    public static float DotToTarget2D(Transform thisXform, Vector3 targetPos)
    {
        Vector3 nominalPos = thisXform.position;
        // Take the y of the nominalPos.
        Vector3 localTargetPos = new Vector3(targetPos.x, nominalPos.y, targetPos.z);
        Vector3 DirToTarget = (localTargetPos - nominalPos).normalized;
        Vector3 ForwardDir = thisXform.forward.normalized;
        return Vector3.Dot(ForwardDir, DirToTarget);
    }

    public static float CrossToTarget(Transform thisXform, Vector3 targetPos)
	{
		Vector3 nominalPos = thisXform.position;
		Vector3 DirToTarget = (targetPos - nominalPos).normalized;
		Vector3 ForwardDir = thisXform.forward.normalized;
		return Vector3.Cross(ForwardDir, DirToTarget).y;
	}

    public static float yAngleToTarget(Transform thisXform, Vector3 targetPos)
    {
        // Project all this geometry onto the y=0 axis.
        Vector3 forwardDir = thisXform.forward;
        forwardDir.y = 0;
        forwardDir = forwardDir.normalized;  // necessary?

        Vector3 targetPosProjY = targetPos;
        targetPosProjY.y = 0f;

        Vector3 nominalPosProjY = thisXform.position;
        nominalPosProjY.y = 0f;

        Vector3 dirToTarget = targetPosProjY - nominalPosProjY;
        dirToTarget = dirToTarget.normalized;
        
        return Vector3.Angle(forwardDir, dirToTarget);
    }

    // Map values 0 to 1 to an acceleration / deceleration curve approximated as piecewise linear.
    public static float MapToCurve(float x)
    {
        if (x <= 0f) return 0f;
        if (x > 1f) return 1f;
        // Piecewise linear approximatation.
        if (x <= 0.2f) return 0.5f*x;
        if (x <= 0.4f) return x - 0.1f;
        if (x <= 0.6f) return 2f*x - 0.5f;
        if (x <= 0.8f) return x + 0.1f;
        else return 0.5f*x + 0.5f;  // x <= 1f
    }


	
}
