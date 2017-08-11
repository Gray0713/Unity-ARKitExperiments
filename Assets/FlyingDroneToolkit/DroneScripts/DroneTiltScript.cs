using UnityEngine;
using System.Collections;

public class DroneTiltScript : MonoBehaviour {

    public enum TiltMode
    {
        Acceleration,
        Velocity,
        Bank
    }
    public TiltMode tiltMode = TiltMode.Acceleration;
    public float tiltAccelFactor = 5000f, tiltVelocFactor = 50f, bankFactor = 25f;
    public float timeFactor = 0.5f;
    public GameObject assembly;

    private Vector3 p1, p2, p3, p2projected;
    private float yaw1, yaw2, yaw3;
    private bool p1valid, p2valid, p3valid;
    private Vector3 accelDir, accelDirWorld, tiltAxis;
    private float accelMag, velocMag, bankMag;
    private float tiltAngle;
    private Vector3 saveRot;
    private Quaternion fromQ, toQ;
   

	void Start () {
        p1valid = p2valid = p3valid = false;
        yaw1 = yaw2 = 0f;
        accelDir = Vector3.zero;
	}

    void RotatePoints ()
    {
        if (!p3valid) p3valid = true;
        else if (!p2valid) p2valid = true;
        else if (!p1valid) p1valid = true;
        if (p2valid)
        {
            p1 = p2;
            yaw1 = yaw2;
        }
        if (p3valid)
        {
            p2 = p3;
            yaw2 = yaw3;
        }
        p3 = transform.position;
        p3.y = 0f;  // Project all points onto the same horizontal plane.
        yaw3 = transform.localEulerAngles.y;
    }

    bool PointsValid()
    {
        return (p1valid && p2valid && p3valid);
    }

    void FixedUpdate () {
        RotatePoints();
        if (PointsValid())
        {
            // Calculate the acceleration direction.
            accelDirWorld = p3 - p1;
            accelDirWorld.y = 0f;
            accelDir = (transform.InverseTransformVector(accelDirWorld));
            accelDir.y = 0f;
            accelDir.Normalize();
            // Calculate the acceleration magnitude.
            p2projected = GeometryClass.ClosestPointOnLine(p1, p3, p2, true);
            accelMag = Vector3.Distance(p2projected, p3) - Vector3.Distance(p1, p2projected);
            velocMag = Vector3.Distance(p3, p1);
            bankMag = Mathf.DeltaAngle(yaw2, yaw1);
            // Calculate the tilt axis.
            if (tiltMode == TiltMode.Bank)
                tiltAxis = Vector3.forward;
            else // tiltMode == TiltMode.Acceleration || tiltMode == TiltMode.Velocity
                tiltAxis = Vector3.Cross(Vector3.up, accelDir);
            if (tiltMode == TiltMode.Acceleration)
                tiltAngle = accelMag * tiltAccelFactor;
            else if (tiltMode == TiltMode.Velocity)
                tiltAngle = velocMag * tiltVelocFactor;
            else if (tiltMode == TiltMode.Bank)
                tiltAngle = bankMag * bankFactor;
            // Tilt as appropriate.
            fromQ = assembly.transform.localRotation;
            toQ = Quaternion.AngleAxis(tiltAngle, tiltAxis);
            assembly.transform.localRotation = Quaternion.Slerp(fromQ, toQ, Time.deltaTime * timeFactor);
        }
    }

    void LateUpdate () {
        // The main transform should rotate only in yaw.
        saveRot = transform.localEulerAngles;
        saveRot.x = saveRot.z = 0f;
        transform.localEulerAngles = saveRot;
        // The tilt transform should not have any yaw.
        saveRot = assembly.transform.localEulerAngles;
        saveRot.y = 0f;
        // Also not pitch in bank mode.
        if (tiltMode == TiltMode.Bank) saveRot.x = 0f;
        assembly.transform.localEulerAngles = saveRot;
    }
}
