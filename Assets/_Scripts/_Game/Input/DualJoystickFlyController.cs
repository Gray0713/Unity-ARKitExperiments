using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DualJoystickFlyController : MonoBehaviour 
{
    [SerializeField] private LeftJoystick leftJoystick;
    [SerializeField] private RightJoystick rightJoystick;
    [SerializeField] private float axisThreshold;
    [SerializeField] private float rotationThreshold;
    [SerializeField] private bool invertJoysticks = true;
    private new bool enabled;
    private DroneMovementScript droneMovement;
	private Vector3 leftJoystickInput; // Holds the input of the Left Joystick
	private Vector3 rightJoystickInput; // Holds the input of the Right Joystick
	// DEBUG Texts
    [Header("Debug Canvas")]
    [SerializeField] private Text leftXText;
	[SerializeField] private Text leftYText;
	[SerializeField] private Text rightXText;
	[SerializeField] private Text rightYText;
	// NOTIFIER
	private Notifier notifier;

	// Use this for initialization
	void Start () 
    {
		// NOTIFIER
		notifier = new Notifier();
		notifier.Subscribe(GameManager.ON_START_FLY, HandleOnStartFly);
        notifier.Subscribe(GameManager.ON_START_PLAY, HandleOnStartFly);
		if (leftJoystick == null)
		{
			Debug.LogError("The Left Joystick is not attached.");
		}

		if (rightJoystick == null)
		{
			Debug.LogError("The Right Joystick is not attached.");
		}
        enabled = false;
	}
    private void HandleOnStartFly(params object[] args)
    {
		droneMovement = PlayerManager.Instance.Player.GetComponent<DroneMovementScript>();
        enabled = true;
	}
    private void FixedUpdate()
    {
        if (enabled)
        {
			leftJoystickInput = leftJoystick.GetInputDirection();
			rightJoystickInput = rightJoystick.GetInputDirection();
			float xLeftJoystick = leftJoystickInput.x;
			float yLeftJoystick = leftJoystickInput.y;
			float xRightJoystick = rightJoystickInput.x;
			float yRightJoystick = rightJoystickInput.y;
			// Left Joystick buttons
			bool W = yLeftJoystick > axisThreshold;
			bool A = xLeftJoystick < -rotationThreshold;
			bool S = yLeftJoystick < -axisThreshold;
			bool D = xLeftJoystick > rotationThreshold;
			// Right Joystick buttons
			bool I = yRightJoystick > axisThreshold;
			bool J = xRightJoystick < -axisThreshold;
			bool K = yRightJoystick < -axisThreshold;
			bool L = xRightJoystick > axisThreshold;
			// Connection to DroneMovementScript
			droneMovement.W = invertJoysticks ? I : W;
			droneMovement.A = invertJoysticks ? J : A;
			droneMovement.S = invertJoysticks ? K : S;
			droneMovement.D = invertJoysticks ? L : D;
			droneMovement.I = invertJoysticks ? W : I;
			droneMovement.J = invertJoysticks ? A : J;
			droneMovement.K = invertJoysticks ? S : K;
			droneMovement.L = invertJoysticks ? D : L;
			// DEBUG Canvas
			leftXText.text = xLeftJoystick.ToString();
			leftYText.text = yLeftJoystick.ToString();
			rightXText.text = xRightJoystick.ToString();
			rightYText.text = yRightJoystick.ToString();
			leftXText.color = (A || D) ? Color.green : Color.red;
			leftYText.color = (W || S) ? Color.green : Color.red;
			rightXText.color = (J || L) ? Color.green : Color.red;
			rightYText.color = (I || K) ? Color.green : Color.red;   
        }
    }
	void OnDestroy()
	{
		// NOTIFIER
		if (notifier != null)
		{
			notifier.UnsubcribeAll();
		}
	}
}
