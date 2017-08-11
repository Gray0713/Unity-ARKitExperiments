using UnityEngine;
using System.Collections;

public class DroneTravelIncremental : DroneTravelStrategy
{
    public override void BeginLateralJourney(FlyingDroneScript scr)
    {
        // No special setup needed.
    }

    public override Vector3 MoveToTargetLateralPos(Transform xform, Vector3 target, Vector3 eTarget, FlyingDroneScript scr)
    {
        // Calculate the new position.
        Vector3 newPos = Vector3.SmoothDamp(xform.position, eTarget, ref scr.smoothVelocity, scr.smoothTime, scr.smoothMaxSpeed);
        xform.position = newPos;
        // Calculate the new rotation.
        Vector3 angles = xform.localEulerAngles;
        float yAngle = angles.y;
        Transform tempXform = xform;
        tempXform.LookAt(target);
        float yAngleTarget = tempXform.localEulerAngles.y;
        float yAngleChanged = Mathf.SmoothDampAngle(yAngle, yAngleTarget, ref scr.smoothVelocityf, scr.smoothTimeRotate, scr.smoothMaxSpeedRotate);
        angles.y = yAngleChanged;
        xform.localEulerAngles = angles;
        return xform.position;
    }

}
