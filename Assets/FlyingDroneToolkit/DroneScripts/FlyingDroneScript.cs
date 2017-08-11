using UnityEngine;
using System.Collections;


public class FlyingDroneScript : MonoBehaviour {

    /*
     * The following definitions, variables, and functions implement the flying drone's different modes,
     * where the mode is a goal-directed behavior.
     */
    public enum DroneMode
    {
        Maintain,
        Hover,
        Manual,
        Land,
        Patrol,
        Follow
    }
    private DroneMode curMode;
    public DroneMode userMode = DroneMode.Hover;

    public enum DroneTravelMode
    {
        Direct,
        TurnThenMove,
        Incremental
    }
    private DroneTravelMode curTravelMode;
    public DroneTravelMode userTravelMode = DroneTravelMode.Incremental;

    public enum DroneFollowMode
    {
        Constrained,
        Direct
    }
    private DroneFollowMode curFollowMode;
    public DroneFollowMode userFollowMode = DroneFollowMode.Constrained;

    public enum DroneMotionMode
    {
        None,
        Sine,
        Simple
    }
    private DroneMotionMode curMotionMode;
    public DroneMotionMode userMotionMode = DroneMotionMode.Simple;

	public enum DroneMaintainHeightMode
	{
		ConstantHeight,
		VariableHeight
	}
	public DroneMaintainHeightMode userMaintainHeightMode = DroneMaintainHeightMode.ConstantHeight;

	/*
	 * Parameters that can be adjusted to tune the behavior.
	 */

    // References to other game objects.
    public GameObject followee;             // For following behavior.
    public GameObject nextWaypoint;
    public GameObject prevWaypoint;

    // Parameters for manually-controlled motion.
	public float forceMultiplier  = 20.0f;  // Controls forward / backward force in manual mode.
    public float forceFactor      = 0.5f;   // Controls how much to increase force based on force control axis.
    public float upDownFactor     = 0.75f;  // Up / down force relative to forward / back.
    public float leftRightFactor  = 0.5f;   // Left / right force relative to forward / back.
    public float rotateFactor     = 0.1f;   // Rotatation force relative to forward / back.

    // Parameters for height, distance, and rotations.
    public float hoverHeight      = 10.0f;  // Height at which to hover.
	public float distTol          = 1.0f;   // how close for drone to be considered "at" destination; should be larger than "drifting" motion
    public float wpDistTol        = 2.0f;   // How close to a waypoint to be "at" the waypoint.
    public float dotTolerance     = 0.998f; // How close rotation must be to face in desired direction.
    public float heightAboveWaypoint = 0f;  // When patrolling or following, drone will be this height above waypoints.

    // Parameters to control simulated "drifting" motion.
    private float motionMag = 0.0f;  // The amount of random "drift" motion set frame-by-frame by the other parameters.
    public float motionMagMax     = 0.03f;  // Max magnitude of "drifting" motion.
	public float motionTimeIncr   = 0.001f; // Controls speed of random motion.  
	public float motionMagHeight  = 5.0f;   // Height above ground for maximum magnitude of random motion.
	public float motionMagDelta   = 0.1f;   // If the drone is this distance from the ground, treat it as zero from ground.

    // Boolean flags.
    public  bool patrolNewNext = true;      // If true, patrolling will not return to previous waypoint unless it is the only choice.
	private bool manualEnabled = true;      // Disables manual mode if axes not properly configured.

    // Script variables.
	private FlyingDroneScript         thisScript;
	private FlyingDroneSpecificScript droneScr;

    // The effective target may be the same as the target, or, if the movement of the drone is constrained,
    // the effective target may be a point within the constraints of the drone's movement.
	private Vector3 target;
	private Vector3 effectiveTarget;

    // Parameters for moving and rotating.  These are used for Direct and TurnThenMove travel.
    [System.NonSerialized]
    public float journeyFracVert    = 0f; // Fraction along vertical journey.
    [System.NonSerialized]
    public float journeyFracHoriz = 0f;   // Fraction along horizontal journey.
    [System.NonSerialized]
    public float journeyFracRotate = 0f;  // Fraction along rotational journey.
    public float journeyDeltaVert   = 0.0001f;
    public float journeyDeltaHoriz  = 0.0001f;
    public float journeyDeltaRotate = 0.001f;

    // Parameters for SmoothDamp() and SmoothDampAngles().  These are used for Incremental travel.
    [System.NonSerialized]
    public Vector3 smoothVelocity;
    [System.NonSerialized]
    public float smoothVelocityf;
    public float smoothTime           = 2.5f;
    public float smoothTimeRotate     = 2.5f;
    public float smoothMaxSpeed       = 10f;
    public float smoothMaxSpeedRotate = 1000f;
    private float   smoothSpeed;
	private float   smoothTimeStart;

	// Strategy pattern to configure behavior.
	private DroneMotionStrategy motionBehavior;  // Seemingly random motion to simulate flight aerodynamics.
    private DroneMotionStrategy motionBehaviorSine;
    private DroneMotionStrategy motionBehaviorSimple;
    private DroneMotionStrategy motionBehaviorNone;
	private DroneTravelStrategy travelBehavior;  // How drone travels to its destination.
    private DroneTravelStrategy travelBehaviorDirect;
    private DroneTravelStrategy travelBehaviorTurnThenMove;
    private DroneTravelStrategy travelBehaviorIncremental;
    private DroneTargetStrategy targetBehavior;  // How drone sets its next target.
    private DroneTargetStrategy targetDirectBehavior;  
	private DroneTargetStrategy targetWaypointBehavior;
	private DroneTargetStrategy targetConstrainedBehavior;

	/*
	 * The following definitions, variables, and functions implement the flying drone's finite state machine.
	 */
	
	private bool  debugFSM = false;
	
	private enum FSMDroneState
	{
		Start,
		OnGround,
		FreeMovement,
		AdjustHeight,
		AtHeight,
		AdjustLateral,
		AtDestination,
		Landing
	}
	private FSMDroneState curState;
	
	private void FSMUpdate()
	{
		switch (curState)
		{
		case FSMDroneState.Start:         UpdateStartState();         break;
		case FSMDroneState.OnGround:      UpdateOnGroundState();      break;
		case FSMDroneState.FreeMovement:  UpdateFreeMovementState();  break;
		case FSMDroneState.AdjustHeight:  UpdateAdjustHeightState();  break;
		case FSMDroneState.AtHeight:      UpdateAtHeightState();      break;
		case FSMDroneState.AdjustLateral: UpdateAdjustLateralState(); break;
		case FSMDroneState.AtDestination: UpdateAtDestinationState(); break;
		case FSMDroneState.Landing:       UpdateLandingState();       break;
		}
	}
	
	// Every state XXX need functions EnterXXXState(), UpdateXXXState, and a case in LeaveState().
	// By convention, EnterXXXState() should start with a call to LeaveState().
	
	private void EnterStartState()
	{
		//LeaveState() is not called for the initial state.
		curState = FSMDroneState.Start;
		if (debugFSM) Debug.Log (string.Format ("{0}: Entering ", curState));
	}
	
	private void UpdateStartState()
	{
		EnterAtDestinationState();
	}
	
	private void EnterOnGroundState()
	{
		LeaveState();
		curState = FSMDroneState.OnGround;
		if (debugFSM) Debug.Log (string.Format ("{0}: Entering ", curState));
        ResetJourney();
	}
	
	private void UpdateOnGroundState()
	{
		if (!droneScr.OnGround())
		{
			MoveToTargetHeight();
		}
		if (curMode != DroneMode.Land && curMode != DroneMode.Maintain)
			EnterAdjustHeightState();
	}
	
	private void EnterFreeMovementState()
	{
		LeaveState();
		curState = FSMDroneState.FreeMovement;
		if (debugFSM) Debug.Log (string.Format ("{0}: Entering ", curState));
	}
	
	private void UpdateFreeMovementState()
	{
		if (!manualEnabled)
			return;
		// Set Speed and Direction according to inputs.
		// Input axes need to be configured to have these labels.
		try {
			float dr  = Input.GetAxis ("DroneRotate");
 			float dfb = Input.GetAxis ("DroneForwardBack");
			float dud = Input.GetAxis ("DroneUpDown");
			float drl = Input.GetAxis ("DroneRightLeft");
            // Adjustable force.
            float localForce = forceMultiplier * (1f + forceFactor * Input.GetAxis("DroneForce"));
			GetComponent<Rigidbody>().AddForce(dfb * transform.forward * localForce);
			GetComponent<Rigidbody>().AddForce(dud * transform.up      * localForce * upDownFactor);
			GetComponent<Rigidbody>().AddForce(drl * transform.right   * localForce * leftRightFactor);
			GetComponent<Rigidbody>().AddTorque(dr * transform.up      * localForce * rotateFactor);
		} catch (UnityException exc) {
			Debug.LogException(exc);
            manualEnabled = false;
			// Go to "Maintain" mode rather than trying over and over.
            SetMaintainMode();
		}
	}
	
	private void EnterAdjustHeightState()
	{
		LeaveState();
		curState = FSMDroneState.AdjustHeight;
		if (debugFSM) Debug.Log (string.Format ("{0}: Entering ", curState));
        ResetJourney();
	}
	
	private void UpdateAdjustHeightState()
	{
		MoveToTargetHeight();
		if (AtTargetHeight())
			EnterAtHeightState();
	}
	
	private void EnterLandingState()
	{
		LeaveState();
		curState = FSMDroneState.Landing;
		if (debugFSM) Debug.Log (string.Format ("{0}: Entering ", curState));
        ResetJourney();
	}
	
	private void UpdateLandingState()
	{
		if (droneScr.OnGround())
			EnterOnGroundState();
		else
		{
			MoveToTargetHeight();
		}
	}
	
	private void EnterAtHeightState()
	{
		LeaveState();
		curState = FSMDroneState.AtHeight;
		if (debugFSM) Debug.Log (string.Format ("{0}: Entering ", curState));
	}
	
	private void UpdateAtHeightState()
	{
		// If the mode is one that does not move laterally, drone is at destination.
		// Otherwise, FSM to AtDestinationState
		if (curMode == DroneMode.Hover || curMode == DroneMode.Land || curMode == DroneMode.Maintain)
			EnterAtDestinationState();
		else
			EnterAdjustLateralState();
	}
	
	private void EnterAdjustLateralState()
	{
		LeaveState();
		curState = FSMDroneState.AdjustLateral;
		if (debugFSM) Debug.Log (string.Format ("{0}: Entering ", curState));
        travelBehavior.BeginLateralJourney(thisScript);
	}

	private void UpdateAdjustLateralState()
	{
		if (AtTargetLateralPos())
			EnterAtDestinationState();
		else
			transform.position = travelBehavior.MoveToTargetLateralPos(transform, target, effectiveTarget, this);
	}
	
	private void EnterAtDestinationState()
	{
		LeaveState();
		curState = FSMDroneState.AtDestination;
		if (debugFSM) Debug.Log (string.Format ("{0}: Entering", curState));
        // If the user has changed the travel or motion mode, make the switch here.
        SetUserTravelAndMotion();
		if (curMode == DroneMode.Maintain)
		{
			// In Maintain mode, do not do anything.
			return;
		}
        if (curMode == DroneMode.Hover)
        {
            effectiveTarget = targetBehavior.GetEffectiveTarget(transform, target, thisScript);
            EnterAdjustHeightState();
        }
        if (curMode == DroneMode.Patrol || curMode == DroneMode.Follow)
		{
            if (!nextWaypoint)
            {
                Debug.LogError("Error:  nextWaypoint must be set in the Inspector.");
                SetMaintainMode();
                return;
            }
        }
        if (curMode == DroneMode.Patrol)
        {
            // If patrolling and waypoint reached, identify the next waypoint.
            targetBehavior.AdvanceWaypoints(nextWaypoint, target, thisScript);
                EnterAdjustLateralState();
		}
		if (curMode == DroneMode.Follow)
		{
			// If following, constrained to waypoints, and waypoint has been reached, 
			// identify the next waypoint.
			if (CloseToNextWaypoint())
			{
				targetBehavior.AdvanceWaypoints(nextWaypoint, target, thisScript);
				effectiveTarget = targetBehavior.GetEffectiveTarget(transform, target, thisScript);
				
			}
			if (CloseToPrevWaypoint())
			{
				targetBehavior.AdvanceWaypoints(prevWaypoint, target, thisScript);
				effectiveTarget = targetBehavior.GetEffectiveTarget(transform, target, thisScript);
				
			}
			if (!AtTargetLateralPos())
				EnterAdjustLateralState();
		}
	}
	
	private void UpdateAtDestinationState()
	{
		if (!AtTargetLateralPos() && curMode != DroneMode.Maintain && curMode != DroneMode.Hover)
				EnterAdjustLateralState();
	}

	private void LeaveState()
	{
		switch (curState)
		{
		case FSMDroneState.Start:
		{
			break;
		}
		case FSMDroneState.OnGround:
		{
			break;
		}
		case FSMDroneState.FreeMovement:
		{
			break;
		}
		case FSMDroneState.AdjustHeight:
		{
			break;
		}
		case FSMDroneState.AtHeight:
		{
			break;
		}
		case FSMDroneState.AdjustLateral:
		{
			break;
		}
		case FSMDroneState.AtDestination:
		{
			break;
		}
		case FSMDroneState.Landing:
		{
			break;
		}
		}
		if (debugFSM) Debug.Log (string.Format ("{0}: Leaving ", curState));
	}
	
	
	public void SetMaintainMode()   
	{ 
		curMode = userMode = DroneMode.Maintain;
		targetBehavior = targetDirectBehavior;
		EnterAtDestinationState();
	}
	public void SetHoverMode()      
	{ 
		curMode = userMode = DroneMode.Hover;      
		targetBehavior = targetDirectBehavior;
		target = transform.position;
		target.y = hoverHeight;
		effectiveTarget = targetBehavior.GetEffectiveTarget(transform, target, thisScript);
		EnterAdjustHeightState();
	}
	public void SetLandMode()       
	{ 
		curMode = userMode = DroneMode.Land;
		targetBehavior = targetDirectBehavior;
		target = transform.position;
		target.y = droneScr.HeightOfGround();
		effectiveTarget = targetBehavior.GetEffectiveTarget(transform, target, thisScript);
		EnterLandingState();
	}
	public void SetManualMode()     
	{
		curMode = userMode = DroneMode.Manual;  
		targetBehavior = targetDirectBehavior;
		EnterFreeMovementState();
	}
	public void SetPatrolMode()     
	{
        if (!nextWaypoint)
        {
            Debug.LogError("Error:  nextWaypoint must be set in the Inspector.");
            SetMaintainMode();
        }
        else
        {
            curMode = userMode = DroneMode.Patrol;
            targetBehavior = targetWaypointBehavior;
            targetBehavior.DroneTargetInit(thisScript);
            // Find the closest waypoint.  This may be any of the waypoints if the drone has been controlled manually.
            DroneWaypointScript wpScript = nextWaypoint.GetComponent<DroneWaypointScript>();
            nextWaypoint = wpScript.NearestWaypoint(transform.position);
            //prevWaypoint = null;
            target = nextWaypoint.transform.position;
            effectiveTarget = targetBehavior.GetEffectiveTarget(transform, target, thisScript);
            EnterAdjustHeightState();
        }
	}
	public void SetFollowMode()     
	{
        if (!followee)
        {
            Debug.LogError("ERROR:  Followee needs to be set in the inspector.");
            SetMaintainMode();
        }
        else if (curFollowMode == DroneFollowMode.Constrained && !nextWaypoint) {
			Debug.LogError("Error:  nextWaypoint must be set in the Inspector.");
			SetMaintainMode();
		}
		else
        {
            curMode = userMode = DroneMode.Follow;
            if (userFollowMode == DroneFollowMode.Constrained)
                targetBehavior = targetConstrainedBehavior;
            else // (userFollowMode == DroneFollowMode.Direct)
                targetBehavior = targetDirectBehavior;
            targetBehavior.DroneTargetInit(thisScript);
            // Set the target to be the followee (XZ), but at the same height as the drone (Y).
            target = GeometryClass.TargetSameHeight(transform.position, followee.transform.position);
            effectiveTarget = targetBehavior.GetEffectiveTarget(transform, target, thisScript);
            EnterAdjustLateralState();
        }
	}

	/*
	 * Functions that implement the Unity callbacks Start() and FixedUpdate().
	 */

	void Start () {
		// Get the specific script for this drone.
		thisScript = gameObject.GetComponent<FlyingDroneScript>();
		droneScr   = gameObject.GetComponent<FlyingDroneSpecificScript>();

        InitializeModes();

        // Force initial state to be what user has selected in the Inspector.
        SetUserTravelAndMotion(true);
        SetUserMode(true);

        InvokeRepeating("CheckFolloweeMoved", 0.5f, 1.0f);
		
	}

    // Using FixedUpdate() because there are some physics calculations.
    void FixedUpdate()
    {
        FSMUpdate();                     // Update finite state machine (FSM).
        AdjustMotionMag();               // Adjust magnitude of random "drifting" motion based on distance from ground.
        AddRandomMotion();               // Add the random "drifting" motion to the drone's height.
        smoothSpeed = smoothVelocity.magnitude;  // Helpful for debugging and setting parameters.
        SetUserMode();
    }
    
    private void InitializeModes()
    {
        // Get all travel scripts.
        travelBehaviorDirect = ScriptableObject.CreateInstance<DroneTravelDirect>();
        travelBehaviorTurnThenMove = ScriptableObject.CreateInstance<DroneTravelTurnThenMove>();
        travelBehaviorIncremental = ScriptableObject.CreateInstance<DroneTravelIncremental>();

        // Get all motion scripts.
        motionBehaviorSine = ScriptableObject.CreateInstance<DroneMotionSinusoidal>();
        motionBehaviorSimple = ScriptableObject.CreateInstance<DroneMotionSimple>();
        motionBehaviorNone = ScriptableObject.CreateInstance<DroneMotionNone>();

        // Get all targeting scripts.
        targetDirectBehavior = ScriptableObject.CreateInstance<DroneTargetDirect>();
        targetWaypointBehavior = ScriptableObject.CreateInstance<DroneTargetWaypoint>();
        targetConstrainedBehavior = ScriptableObject.CreateInstance<DroneTargetConstrained>();
        targetBehavior = targetDirectBehavior;  // Set initial targeting to direct.

    }

    // This is called each time a destination is reached, for possible update.
    private void SetUserTravelAndMotion(bool force=false)
    {
        if (force || (userTravelMode != curTravelMode))
        {
            switch (userTravelMode)
            {
                case DroneTravelMode.TurnThenMove:
                    travelBehavior = travelBehaviorTurnThenMove;
                    curTravelMode = DroneTravelMode.TurnThenMove;
                    break;
                case DroneTravelMode.Incremental:
                    travelBehavior = travelBehaviorIncremental;
                    curTravelMode = DroneTravelMode.Incremental;
                    break;
                default:
                    travelBehavior = travelBehaviorDirect;
                    curTravelMode = DroneTravelMode.Direct;
                    break;
            }
        }

        if (force || (userMotionMode != curMotionMode))
        {
            switch (userMotionMode)
            {
                case DroneMotionMode.Sine:
                    motionBehavior = motionBehaviorSine;
                    curMotionMode = DroneMotionMode.Sine;
                    break;
                case DroneMotionMode.Simple:
                    motionBehavior = motionBehaviorSimple;
                    curMotionMode = DroneMotionMode.Simple;
                    break;
                default:
                    motionBehavior = motionBehaviorNone;
                    curMotionMode = DroneMotionMode.None;
                    break;
            }
        }

    }

    // This is called by FixedUpdate(), but with a quick exit if there is no change.
    private void SetUserMode(bool force=false)
    {
        if (force || (userMode != curMode))
        {
            switch (userMode)
            {
                case DroneMode.Follow:
                    SetFollowMode();
                    break;
                case DroneMode.Hover:
                    SetHoverMode();
                    break;
                case DroneMode.Land:
                    SetLandMode();
                    break;
                case DroneMode.Maintain:
                    SetMaintainMode();
                    break;
                case DroneMode.Manual:
                    SetManualMode();
                    break;
                case DroneMode.Patrol:
                    SetPatrolMode();
                    break;
                default:
                    SetMaintainMode();
                    break;
            }
        }

        // Adjust the targetBehavior if necessary.
        if (userMode == DroneMode.Follow && (force || userFollowMode != curFollowMode))
        {
            switch (userFollowMode)
            {
                case DroneFollowMode.Constrained:
                    targetBehavior = targetConstrainedBehavior;
                    curFollowMode = DroneFollowMode.Constrained;
                    break;
                default:
                    targetBehavior = targetDirectBehavior;
                    curFollowMode = DroneFollowMode.Direct;
                    break;
            }
        }

    }

	/*
	 * Internal functions for flying drone movement.
	 */

    public void ResetJourney()
    {
        journeyFracVert    = 0f;
        journeyFracHoriz   = 0f;
        journeyFracRotate  = 0f;
    }

    private void MoveToTargetHeight()
	{
		Vector3 targetPos = transform.position;
		targetPos.y = effectiveTarget.y;
        journeyFracVert += journeyDeltaVert;
        transform.position = Vector3.Lerp(transform.position, targetPos, GeometryClass.MapToCurve(journeyFracVert));

	}
	
	private bool AtTargetHeight()
	{
		if (Mathf.Abs (transform.position.y - effectiveTarget.y) < distTol) 
			return true;
		else
			return false;
	}
	
	// Tests within XZ only, assumes height is the same
	private bool AtTargetLateralPos()
	{
		Vector3 tempPos = transform.position;
		tempPos.y = effectiveTarget.y;
        float distToTarget = Vector3.Distance(tempPos, effectiveTarget);
		return distToTarget < distTol;
	}

	public void AdjustMotionMag()
	{
		motionMag = motionMagMax * Mathf.Clamp(droneScr.DistanceFromGround() / motionMagHeight, 0f, 1f);
		if (motionMag < motionMagDelta)
			motionMag = 0f;
	}
	
	/*
	 * Functions for moving among waypoints.
	 */

	private bool CloseToNextWaypoint()
	{
		Vector3 pos = transform.position;
        // Adjust for same height; determine if close to waypoint based on XZ lateral position.
        pos.y = nextWaypoint.transform.position.y; 
		float nextDist = Vector3.Distance(pos, nextWaypoint.transform.position);
		return (nextDist < wpDistTol);
	}
	
	private bool CloseToPrevWaypoint()
	{
		Vector3 pos = transform.position;
        // Adjust for same height; determine if close to waypoint based on XZ lateral position.
        pos.y = prevWaypoint.transform.position.y;
        float prevDist = Vector3.Distance(pos, prevWaypoint.transform.position);
		return (prevDist < wpDistTol);
	}

	// Set prevWaypoint to the nextWaypoint, and nextWaypoint to be a random neighbor.
	// This assumes that the set of waypoints are connected in a graph and that
	// nextWaypoint is set to one of them.
	// The height of the target is the height of the next waypoint.
	public void RandomNextWaypoint()
	{
		DroneWaypointScript scr = nextWaypoint.GetComponent<DroneWaypointScript>();
        GameObject savePrev = prevWaypoint;
		prevWaypoint = nextWaypoint;
        if (patrolNewNext)
    		nextWaypoint = scr.randomNeighbor(savePrev);
        else
            nextWaypoint = scr.randomNeighbor();
		target = nextWaypoint.transform.position;
		effectiveTarget = targetBehavior.GetEffectiveTarget(transform, target, thisScript);
	}

	// Set prevWaypoint to be the closest one and nextWaypoint to be the closest neightbor to 
	// prevWaypoint.  This assumes that the set of waypoints are connected in a graph and that
	// nextWaypoint is set to one of them.
	public void UpdateClosestWaypoints()
	{
		DroneWaypointScript nextScr = nextWaypoint.GetComponent<DroneWaypointScript>();
		GameObject closeWP = nextScr.NearestWaypoint(transform.position);
		DroneWaypointScript closeScr = closeWP.GetComponent<DroneWaypointScript>(); 
		GameObject nextCloseWP = closeScr.NearestNeighborToPoint(transform.position);
		nextWaypoint = closeWP;
		prevWaypoint = nextCloseWP;
	}
	
	/*
	 * When the flying drone is in follow mode, check periodically to determine whether the followee has moved.
	 * Also, if the drone is a dynamic waypoint follower, determine whether there is a waypoint that should be the 
	 * interim target, if the true target is occluded.
	 */
	
	private void CheckFolloweeMoved()
	{
		if (curMode == DroneMode.Follow)
		{
			// See if this drone has a waypont follower component.
			DroneDynamicWaypointFollower followComponent = GetComponent<DroneDynamicWaypointFollower>();
			if (followComponent) {
				// Call this function if the drone is a dynamic waypoint follower.
				target = followComponent.GetTargetPos(followee);
			} else {
				// This is the default if the drone is not a dynamic waypoint follower.
				target = followee.transform.position;
			}
			// When following, the drone should optionallly maintain its altitude, presumably above the target.
			if (userMaintainHeightMode == DroneMaintainHeightMode.ConstantHeight) {
				target = GeometryClass.TargetSameHeight (transform.position, target);
			}
			// The effecitve target may, for example, be constrained to the paths between static waypoints.
			effectiveTarget = targetBehavior.GetEffectiveTarget(transform, target, thisScript);
		}
	}
	

	
	/*
	 * Add a random drifting motion to the flying drone.
	 */

	private void AddRandomMotion()
	{
		transform.position = motionBehavior.AddRandomMotion(transform.position, motionMag, motionTimeIncr);
	}

}
