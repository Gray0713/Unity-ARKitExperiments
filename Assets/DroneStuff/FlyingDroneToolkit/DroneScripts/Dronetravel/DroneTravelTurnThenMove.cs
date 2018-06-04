using UnityEngine;
using System.Collections;

public class DroneTravelTurnThenMove : DroneTravelStrategy {
    
    private enum MoveState
    {
        turn,
        move
    } 
    private MoveState mvState;

    public override void BeginLateralJourney(FlyingDroneScript scr)
    {
        mvState = MoveState.turn;
        scr.ResetJourney();
    }
	
	public override Vector3 MoveToTargetLateralPos(Transform xform, Vector3 target, Vector3 eTarget, FlyingDroneScript scr)
	{
        if (mvState == MoveState.turn)
        {
		    // Calculate the new rotation.
		    float dot   = GeometryClass.DotToTarget2D(xform, target);
            if (dot < scr.dotTolerance)
            {
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
                
            }
            else
            {
                mvState = MoveState.move;
                scr.ResetJourney();
            }
		} else { // mvState == MoveState.move
			// Calculate new position only if rotation is already correct.
            scr.journeyFracHoriz += scr.journeyDeltaHoriz;
            Vector3 newPos = Vector3.Lerp(xform.transform.position, eTarget, GeometryClass.MapToCurve(scr.journeyFracHoriz));
            xform.position = newPos;
		}
		return xform.position;
	}
	
}
