using UnityEngine;
using System.Collections;

public class ElisaScript : MonoBehaviour {

	public GameObject[] elisa;

	private float currentYRotation;
	private float rotationSpeed = 1000;
	public float idleRotationSpeed = 1000;
	public float movingRotationSpeed = 2000;
	public float elisaAngle;
	public bool spinDifference = true;
	void Update(){

		RotationInputs ();
		RotationDifferentials ();
		WingtipVortices ();

	}

	void RotationInputs(){
		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||
			Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K)){
			rotationSpeed = movingRotationSpeed;
		}
		else{
			rotationSpeed = idleRotationSpeed;
		}
	}

	void RotationDifferentials(){
		currentYRotation += Time.deltaTime * rotationSpeed;
		for(int i = 0; i< elisa.Length; i++){
			if(spinDifference == true){
				if(i % 2 == 0)	elisa[i].transform.localRotation = Quaternion.Euler(new Vector3(elisaAngle,currentYRotation,transform.rotation.z));
				else	elisa[i].transform.localRotation = Quaternion.Euler(new Vector3(elisaAngle,currentYRotation + 90,transform.rotation.z));
			}
			else{
				elisa[i].transform.localRotation = Quaternion.Euler(new Vector3(elisaAngle,currentYRotation,transform.rotation.z));
			}
		}
	}

	void Awake(){
		LocateWintipParticles ();//used to determine how many particles on propelers do we have?...
	}
	private int amountOfWingtipVorticesOnElisas = 0;
	void LocateWintipParticles(){
		amountOfWingtipVorticesOnElisas = 0; 
		for (int i = 0; i < elisa.Length; i++) {
			if(elisa[i].GetComponent<ParticleSystem> ()){
				amountOfWingtipVorticesOnElisas++;
			}
		}
		wingtipVortices = new ParticleSystem[amountOfWingtipVorticesOnElisas];
		for (int i = 0; i < amountOfWingtipVorticesOnElisas; i++) {
			wingtipVortices[i] = elisa[i].GetComponent<ParticleSystem> ();
		}
	}

	private ParticleSystem[] wingtipVortices;
	public float atWhatSpeedsShowWingtipVortices = 20;//at what speed will this be shown
	private float wantedAlpha;
	private float currentAlpha;
	void WingtipVortices(){
		if (GetComponent<DroneMovementScript>().velocity >= atWhatSpeedsShowWingtipVortices) {
			wantedAlpha = 0.2f;
		} else {
			wantedAlpha = 0;
		}

		currentAlpha = Mathf.Lerp (currentAlpha, wantedAlpha, Time.deltaTime * 3);

		if (wingtipVortices.Length > 0)
			foreach (ParticleSystem ps in wingtipVortices) {
				ps.Play ();
				var x = ps.main;
				x.startColor = new Color (ps.main.startColor.color.r, ps.main.startColor.color.g, ps.main.startColor.color.b, currentAlpha);
			}
	}

}
