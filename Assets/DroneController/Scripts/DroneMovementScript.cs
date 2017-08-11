using UnityEngine;
using System.Collections;
public class DroneMovementScript: MonoBehaviour{
	[HideInInspector]//used for idle animations 
	public bool idle = true;
	public  Animator animatedGameObject;
	public bool mobile_turned_on = false;
	public bool joystick_turned_on = false;
	Rigidbody ourDrone;
	AudioSource droneSound;
	public  float velocity; //check for speed
	[HideInInspector]public CameraScript mainCamera;
	public Transform droneObject;

	void Awake(){
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraScript> ();
		if (!mainCamera) {
			print ("Missing main camera! check the tags");
		}
		ourDrone = GetComponent<Rigidbody>();
		try{
			droneSound = gameObject.transform.Find("drone_sound").GetComponent<AudioSource>();
		}
		catch(System.Exception ex){
			print("No Sound Child GameObject ->" + ex.StackTrace.ToString());
		}
	}
	void FixedUpdate(){
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraScript> ();
		velocity = ourDrone.velocity.magnitude;	

		ClampingSpeedValues();
		MovementUpDown();
		MovementLeftRight();
		Rotation();
		MovementForward();
		DroneSound();

		if(joystick_turned_on == false){
			Input_Mobile_Sensitvity_Calculation();
		}
		else{
			Joystick_Input_Sensitivity_Calculation();
		}

		if(mobile_turned_on == true){
			TouchCalculations();
			CheckingIfInside();
		}
	

		ourDrone.AddRelativeForce(Vector3.up * upForce);

		ourDrone.rotation = Quaternion.Euler(
			new Vector3(0, currentYRotation, 0)
			);

		
		droneObject.rotation = Quaternion.Euler(
			new Vector3(tiltAmountForward, currentYRotation, tiltAmountSideways)
		);
	}
		
	void RotationUpdateLoop_TrickRotation(){
		if ((mainCamera.pickedMyDrone == true && mainCamera.ourDrone.transform == transform)) {
			if(Input.GetKeyDown(KeyCode.U)){
				wantedYRotation -= 100;
			}
			if(Input.GetKeyDown(KeyCode.O)){
				wantedYRotation += 100;
			}
		}
	}
	void Update(){

		RotationUpdateLoop_TrickRotation();
		Animations();

		/*
		 * If we picked the drone we wish to control and if its the same one as this one
		 * control only this drone, else remain uncontrolled
		 */
		if ((mainCamera.pickedMyDrone == true && mainCamera.ourDrone.transform == transform)) {
			if (mobile_turned_on == false && joystick_turned_on == false) {
				W = (Input.GetKey (KeyCode.W)) ? true : false;
				S = (Input.GetKey (KeyCode.S)) ? true : false;
				A = (Input.GetKey (KeyCode.A)) ? true : false;
				D = (Input.GetKey (KeyCode.D)) ? true : false;

				I = (Input.GetKey (KeyCode.I)) ? true : false;
				J = (Input.GetKey (KeyCode.J)) ? true : false;
				K = (Input.GetKey (KeyCode.K)) ? true : false;
				L = (Input.GetKey (KeyCode.L)) ? true : false;
			}
			if (mobile_turned_on == false && joystick_turned_on == true) {
				W = (Input.GetAxisRaw ("Vertical") > 0) ? true : false;
				S = (Input.GetAxisRaw ("Vertical") < 0) ? true : false;

				D = (Input.GetAxisRaw ("Horizontal") > 0) ? true : false;
				A = (Input.GetAxisRaw ("Horizontal") < 0) ? true : false;

				K = (Input.GetKey (downButton)) ? true : false;
				I = (Input.GetKey (upButton)) ? true : false;
			}
		}

	}


	private void Animations(){
		if(animatedGameObject != null){
			animatedGameObject.SetBool("idle", idle);
			if(Input.GetKeyDown(KeyCode.U)){
				StartCoroutine("Twirl_left_Method");
			}
			if(Input.GetKeyDown(KeyCode.O)){
				StartCoroutine("Twirl_right_Method");
			}
			animatedGameObject.SetBool("left_passage", Input.GetKey(KeyCode.Q));
			animatedGameObject.SetBool("right_passage", Input.GetKey(KeyCode.E));
		}
	}

	IEnumerator Twirl_left_Method(){
		animatedGameObject.SetBool("twirl_left", true);
		yield return new WaitForEndOfFrame();
		animatedGameObject.SetBool("twirl_left", false);
	}
	IEnumerator Twirl_right_Method(){
		animatedGameObject.SetBool("twirl_right", true);
		yield return new WaitForEndOfFrame();
		animatedGameObject.SetBool("twirl_right", false);
  	}

	private Vector3 velocityToSmoothDampToZero;
	[Header("MAX SPEEDS")]
	public int maxForwardSpeed = 10;
	public int maxSidewaySpeed = 5;
	[Header("Drone slowdown")]
	[Range(0.0f,2.0f)]
	public float slowDownTime = 0.95f;
	private void ClampingSpeedValues(){
		if((W || S) && (A || D)){
		//if(Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f){
			ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, maxForwardSpeed, Time.deltaTime * 5f));
		}
		if((W || S) && (!A && !D)){
		//if(Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f){
			ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, maxForwardSpeed, Time.deltaTime * 5f));
		}
		if((!W && !S) && (A || D)){
		//if(Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f){
			ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, maxSidewaySpeed, Time.deltaTime * 5f));
		}
		if(!W && !S && !A && !D){
		//if(Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f){
			//ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.SmoothDamp(ourDrone.veloc/ity.magnitude,0.0f, ref currentVelocityToSlowDown,0.5f));
			ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, slowDownTime);
		}
	}

	private void DroneSound(){
		velocity = ourDrone.velocity.magnitude;
		if(droneSound){
			droneSound.pitch = 1 + (velocity/100);
		}

		if (velocity > 1) {
			droneSound.spatialBlend = Mathf.Lerp (droneSound.spatialBlend, 0.0f, Time.deltaTime * 1);
		} else {
			droneSound.spatialBlend = Mathf.Lerp (droneSound.spatialBlend, 1.0f, Time.deltaTime * 1);
		}

	}

	private float mouseScrollWheelAmount;
	private float wantedHeight;
	private float currentHeight;
	private float heightVelocity;
	//[HideInInspector]

	[Header("Up & Down Forces")]
	public float upForce;
	public float forceUpHover = 450;
	public float forceDownHover = -200;

	private void MovementUpDown(){
		if((W || S) || (A || D)){
			idle = false;
		//if((Mathf.Abs(Input.GetAxis("Vertical")) > 0.2 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2)){
			if(I || K){
			//if(Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K)){
				ourDrone.velocity = ourDrone.velocity;
			}
			if(!I && !K && !J && !L){
			//if(!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.L)){
				ourDrone.velocity = new Vector3(ourDrone.velocity.x,Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5),ourDrone.velocity.z);
				upForce = 98.001f;//271
			}
			if(!I && !K && (J || L)){
			//if(!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.L))){
				ourDrone.velocity = new Vector3(ourDrone.velocity.x,Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5),ourDrone.velocity.z);
				upForce = 98.005f;//110
			}
		}
		if((!W || !S) && (A || D)){
			idle = false;
		//if((Mathf.Abs(Input.GetAxis("Vertical")) < 0.2 && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2)){
			upForce = 98.002f;//136
		}
		if((W || S) && (A || D)){
			idle = false;
		//if((Mathf.Abs(Input.GetAxis("Vertical")) > 0.2 && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2)){
			if(J || L){
			//if(Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.L)){
				upForce = 98.003f;//410
			}
		}
		if(I){
			idle = false;
		//if(Input.GetKey(KeyCode.I)){
			upForce = forceUpHover; //450
			if(A || D)
			//if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
				upForce = forceUpHover; //500
		}
		else if(K){
			idle = false;
	//	else if(Input.GetKey(KeyCode.K)){
			upForce = forceDownHover;//-200
		}
		else if(!I && !K && ((!W && !S) && (!A && !D))){
		//else if(!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && (Mathf.Abs(Input.GetAxis("Vertical")) < 0.2 && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2)){
			upForce = 98;
			idle = true;
		}

	}

	[Header("Front & Side Movement Forces")]
	public float sideMovementAmount = 300.0f;
	public float movementForwardSpeed = 500.0f;

	private float tiltAmountSideways = 0;
	private float tiltVelocitySideways;
	private void MovementLeftRight(){
		if(A){
			ourDrone.AddRelativeForce(Vector3.right * Horizontal_A * sideMovementAmount);
			tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways,-20 * Horizontal_A, ref tiltVelocitySideways, tiltMovementSpeed);
		}
		if(D){
			ourDrone.AddRelativeForce(Vector3.right * Horizontal_D * sideMovementAmount);
			tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways,-20 * Horizontal_D, ref tiltVelocitySideways, tiltMovementSpeed);
		}
		if(!A && !D){
			tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltVelocitySideways, tiltNoMovementSpeed);
		}
		/*
		if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f ){
			ourDrone.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * sideMovementAmount);
			tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways,-20 * Input.GetAxis("Horizontal"), ref tiltVelocitySideways, 0.1f);
		}
		else{
			tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltVelocitySideways, 0.1f);
		}
		*/
	}

	private float wantedYRotation;
	[HideInInspector]public float currentYRotation;
	[Header("Rotation Amount Mulitplier")]
	public float rotationAmount = 2.5f;
	private float rotationYVelocity;
	private void Rotation(){
		if(joystick_turned_on == false){
			if(J){
				//if(Input.GetKey(KeyCode.J)){
				wantedYRotation -= rotationAmount;
			}
			if(L){	
				//if(Input.GetKey(KeyCode.L)){
				wantedYRotation += rotationAmount;
			}
		}
		else{
			wantedYRotation += rotationAmount * Input.GetAxis(right_analog_x);
		}

	

		currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
	}


	private float tiltAmountForward = 0;
	private float tiltVelocityForward;
	[Header("Movement Tilt Speed")]
	[Range(0.0f,1.0f)]
	public float tiltMovementSpeed = 0.1f;
	[Range(0.0f,1.0f)]
	public float tiltNoMovementSpeed = 0.3f;
	private void MovementForward(){

		if(W){
			ourDrone.AddRelativeForce(Vector3.forward * Vertical_W * movementForwardSpeed);
			tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20 * Vertical_W, ref tiltVelocityForward, tiltMovementSpeed);
		}
		if(S){
			ourDrone.AddRelativeForce(Vector3.forward * Vertical_S * movementForwardSpeed);
			tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20 * Vertical_S, ref tiltVelocityForward, tiltMovementSpeed);
		}

		if(!W && !S){
			tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 0, ref tiltVelocityForward, tiltNoMovementSpeed);

		}

	}
	private void Input_Mobile_Sensitvity_Calculation(){
		if(W == true){
			Vertical_W = Mathf.LerpUnclamped(Vertical_W,1, Time.deltaTime * 10);
		}
		else Vertical_W = Mathf.LerpUnclamped(Vertical_W,0, Time.deltaTime * 10);

		if(S == true){
			Vertical_S = Mathf.LerpUnclamped(Vertical_S,-1, Time.deltaTime * 10);
		}
		else Vertical_S = Mathf.LerpUnclamped(Vertical_S,0, Time.deltaTime * 10);

		if(A == true){
			Horizontal_A = Mathf.LerpUnclamped(Horizontal_A,-1, Time.deltaTime * 10);
		}
		else Horizontal_A = Mathf.LerpUnclamped(Horizontal_A,0, Time.deltaTime * 10);

		if(D == true){
			Horizontal_D = Mathf.LerpUnclamped(Horizontal_D,1, Time.deltaTime * 10);
		}
		else Horizontal_D = Mathf.LerpUnclamped(Horizontal_D,0, Time.deltaTime * 10);

	}

	[Header("JOYSTICK AXIS INPUT")]
	public string left_analog_x = "Horizontal";
	public string left_analog_y = "Vertical";
	public string right_analog_x = "Horizontal_Right";
	public KeyCode downButton = KeyCode.JoystickButton13;
	public KeyCode upButton = KeyCode.JoystickButton14;
	private void Joystick_Input_Sensitivity_Calculation(){
		
		Vertical_W = Input.GetAxis(left_analog_y);
		Vertical_S = Input.GetAxis(left_analog_y);

		Horizontal_D = Input.GetAxis(left_analog_x);
		Horizontal_A = Input.GetAxis(left_analog_x);

	}
	private float Vertical_W = 0;
	private float Vertical_S = 0;
	private float Horizontal_A = 0;
	private float Horizontal_D = 0;

	public bool W,S,A,D,I,J,K,L;
	private Rect wRect = new Rect(10,55,14,20);
	private Rect sRect = new Rect(10,80,14,20);
	private Rect aRect = new Rect(0,67.5f,14,20);
	private Rect dRect = new Rect(20,67.5f,14,20);

	private Rect iRect = new Rect(76,55,14,20);
	private Rect jRect = new Rect(66,67.5f,14,20);
	private Rect kRect = new Rect(76,80,14,20);
	private Rect lRect = new Rect(86,67.5f,14,20);
	public Texture buttonTexture;


	void OnGUI(){
		if(mobile_turned_on == true){
			DrawGUI.DrawTexture(wRect.x,wRect.y,wRect.width,wRect.height, buttonTexture);
			DrawGUI.DrawTexture(sRect.x,sRect.y,sRect.width,sRect.height, buttonTexture);
			DrawGUI.DrawTexture(aRect.x,aRect.y,aRect.width,aRect.height, buttonTexture);
			DrawGUI.DrawTexture(dRect.x,dRect.y,dRect.width,dRect.height, buttonTexture);

			DrawGUI.DrawTexture(iRect.x,iRect.y,iRect.width,iRect.height, buttonTexture);
			DrawGUI.DrawTexture(jRect.x,jRect.y,jRect.width,jRect.height, buttonTexture);
			DrawGUI.DrawTexture(kRect.x,kRect.y,kRect.width,kRect.height, buttonTexture);
			DrawGUI.DrawTexture(lRect.x,lRect.y,lRect.width,lRect.height, buttonTexture);
		}
	}

	void CheckingIfInside(){		
		W = ( wRect.Contains(PositionMoja[0]) || wRect.Contains(PositionMoja[1]) ) ? true : false;
		S = ( sRect.Contains(PositionMoja[0]) || sRect.Contains(PositionMoja[1]) ) ? true : false;
		A = ( aRect.Contains(PositionMoja[0]) || aRect.Contains(PositionMoja[1]) ) ? true : false;
		D = ( dRect.Contains(PositionMoja[0]) || dRect.Contains(PositionMoja[1]) ) ? true : false;

		I = ( iRect.Contains(PositionMoja[0]) || iRect.Contains(PositionMoja[1]) ) ? true : false;
		J = ( jRect.Contains(PositionMoja[0]) || jRect.Contains(PositionMoja[1]) ) ? true : false;
		K = ( kRect.Contains(PositionMoja[0]) || kRect.Contains(PositionMoja[1]) ) ? true : false;
		L = ( lRect.Contains(PositionMoja[0]) || lRect.Contains(PositionMoja[1]) ) ? true : false;
	}

	private Vector2[] PositionMoja = new Vector2[]{
		new Vector2(0,0),
		new Vector2(0,0)
	};

	void TouchCalculations(){
		for(int i = 0; i < Input.touches.Length; i++){
			PositionMoja[i] = DrawGUI.Percentages(new Vector2(Input.GetTouch(i).position.x, Screen.height - Input.GetTouch(i).position.y));
		}
		if(Input.touchCount == 0){
			for(int i = 0; i < 2; i++){
				PositionMoja[i] = Vector2.zero;
			}
		}
		if(Input.touchCount == 1){
			PositionMoja[1] = Vector2.zero;
		}
	}

}
