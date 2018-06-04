using UnityEngine;
using System.Collections;
using UnityEngine.VR;
public class CameraScript : MonoBehaviour {
	public Transform[] dronesToControl; //used to pick  between more drones in one scene
	private int counterToControl = 0; //counter which determins what drone we are following
	[HideInInspector]public bool pickedMyDrone = false; // if we picked our drone or not
	[HideInInspector]public GameObject ourDrone; //our drone game object

	void Awake(){
		UnityEngine.XR.InputTracking.Recenter();

		//added pick the drone before you fly
		if (dronesToControl.Length > 1)
			ourDrone = dronesToControl [counterToControl].gameObject;
		else {
			ourDrone = GameObject.FindGameObjectWithTag("Player").gameObject;
			pickedMyDrone = true;
		}
		Input.gyro.enabled = true;

	}
	private Vector3 velocitiCameraFollow;
	public Vector3 positionBehindDrone = new Vector3(0,2,-4);
	void FixedUpdate(){
		
		FollowDroneMethod();

		FreeMouseMovementView();

		ScrollMath();

	}
	[Range(0.0f,0.1f)]
	public float cameraFollowPositionTime = 0.1f;
	void FollowDroneMethod(){
		if(pickedMyDrone) 
			transform.position = Vector3.SmoothDamp(transform.position, ourDrone.transform.TransformPoint(positionBehindDrone + new Vector3(0, yScrollValue, zScrollValue)), ref velocitiCameraFollow, cameraFollowPositionTime);
		else
			transform.position = Vector3.SmoothDamp(transform.position, dronesToControl[counterToControl].transform.TransformPoint(positionBehindDrone + new Vector3(0, yScrollValue, zScrollValue)), ref velocitiCameraFollow, cameraFollowPositionTime);
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.U)){
			UnityEngine.XR.InputTracking.Recenter();

		}
		VRMovement();
		PickDroneToControl ();

	}

	public GameObject[] canvasSelectButtons;
	public GameObject[] canvasExitButtons;
	void PickDroneToControl (){
		if (dronesToControl.Length > 1) {
			if (pickedMyDrone == false) {

				//freeMouseMovement = true;

				foreach (GameObject go in canvasSelectButtons) {
					go.SetActive (true);
				}
				foreach (GameObject go in canvasExitButtons) {
					go.SetActive (false);
				}

				if (Input.GetKeyDown (KeyCode.Return)) {
					Select ();
				}
				if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) {
					PressedLeft ();
				}
				if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) {
					PressedRight ();
				}
			} else {

				//freeMouseMovement = false;
				//mouseXwanted = 0;
			//	mouseYwanted = 0;

				foreach (GameObject go in canvasSelectButtons) {
					go.SetActive (false);
				}
				foreach (GameObject go in canvasExitButtons) {
					go.SetActive (true);
				}

				if (Input.GetKeyDown (KeyCode.Escape)) {
					ReturnToPick ();
				}
			}
		} else {
			foreach (GameObject go in canvasSelectButtons) {
				go.SetActive (false);
			}
			foreach (GameObject go in canvasExitButtons) {
				go.SetActive (false);
			}
		}
	}
	public void ReturnToPick(){
		pickedMyDrone = false;

	}
	public void Select(){
		ourDrone = dronesToControl [counterToControl].gameObject;
		pickedMyDrone = true;
	}
	public void PressedLeft (){
		if (counterToControl >= 1) {
			counterToControl--;
		} else {
			counterToControl = dronesToControl.Length - 1;
		}
	}
	public void PressedRight(){
		if (counterToControl < dronesToControl.Length - 1) {
			counterToControl++;
		} else {
			counterToControl = 0;
		}
	}

	private float x_Rotation, y_Rotation;
	public Transform VR_rotator;
	void VRMovement(){
		if(gameObject.name.Contains("VR")){
			x_Rotation -= Input.gyro.rotationRateUnbiased.x;
			y_Rotation -= Input.gyro.rotationRateUnbiased.y;

			transform.rotation = Quaternion.Euler(new Vector3(14,ourDrone.GetComponent<DroneMovementScript>().currentYRotation,0));
			VR_rotator.rotation = Quaternion.Euler(new Vector3(14,ourDrone.GetComponent<DroneMovementScript>().currentYRotation,0)) * Quaternion.Euler(x_Rotation, y_Rotation, 0);
		}
	}
	public bool freeMouseMovement = false;
	private float mouseXwanted,mouseYwanted;
	public float mouseSensitvity = 100;
	private float currentXPos, currentYPos;
	private float xVelocity, yVelocity;
	public float mouseFollowTime = 0.2f;
	void FreeMouseMovementView(){
		if(freeMouseMovement == true){
			mouseXwanted -= Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitvity;
			mouseYwanted += Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitvity;

			currentXPos = Mathf.SmoothDamp(currentXPos, mouseXwanted, ref xVelocity, mouseFollowTime);
			currentYPos = Mathf.SmoothDamp(currentYPos, mouseYwanted, ref yVelocity, mouseFollowTime);

			transform.rotation = Quaternion.Euler(new Vector3(14,ourDrone.GetComponent<DroneMovementScript>().currentYRotation,0)) *
				Quaternion.Euler(currentXPos, currentYPos, 0);

		}
		else{
			if(pickedMyDrone) 
				transform.rotation = Quaternion.Euler(new Vector3(14,ourDrone.GetComponent<DroneMovementScript>().currentYRotation,0));
			else
				transform.rotation = Quaternion.Euler(new Vector3(14,dronesToControl[counterToControl].transform.GetComponent<DroneMovementScript>().currentYRotation,0));
		}
	}

	private float zScrollAmountSensitivity = 1, yScrollAmountSensitivity = -0.5f;
	private float zScrollValue, yScrollValue;
	void ScrollMath(){
		if (Input.GetAxis("Mouse ScrollWheel") != 0f ){
			zScrollValue += Input.GetAxis("Mouse ScrollWheel") * zScrollAmountSensitivity;
			yScrollValue += Input.GetAxis("Mouse ScrollWheel") * yScrollAmountSensitivity;
		}
	}

}
