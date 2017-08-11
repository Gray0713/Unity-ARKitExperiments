using UnityEngine;
using System.Collections;

public class DroneTravelDirect : DroneTravelStrategy {

    public override void BeginLateralJourney(FlyingDroneScript scr)
    {
        scr.ResetJourney();
    }
    
	public override Vector3 MoveToTargetLateralPos(Transform xform, Vector3 target, Vector3 eTarget, FlyingDroneScript scr)
	{
		// Calculate the new position.
        scr.journeyFracHoriz += scr.journeyDeltaHoriz;
        Vector3 newPos = Vector3.Lerp(xform.position, eTarget, GeometryClass.MapToCurve(scr.journeyFracHoriz));
        xform.position = newPos;
		// Calculate the new rotation.
		Vector3 angles = xform.localEulerAngles;
		float yAngle = angles.y;
		Transform tempXform = xform;
		tempXform.LookAt(target);
		float yAngleTarget = tempXform.localEulerAngles.y;
        scr.journeyFracRotate += scr.journeyDeltaRotate;
        float yAngleChanged = Mathf.LerpAngle(yAngle, yAngleTarget, GeometryClass.MapToCurve(scr.journeyFracRotate));
        angles.y = yAngleChanged;
		xform.localEulerAngles = angles;
		return xform.position;
	}
}
