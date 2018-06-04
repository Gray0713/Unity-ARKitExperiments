using UnityEngine;
using System.Collections;

public abstract class DroneTravelStrategy : ScriptableObject {

    public abstract void BeginLateralJourney(FlyingDroneScript scr);
    
	public abstract Vector3 MoveToTargetLateralPos(Transform xform, Vector3 target, Vector3 eTarget, FlyingDroneScript scr);

}
