using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class VRSelectScript : MonoBehaviour {

	RaycastHit hit;
	private float buttonFill = 0;
	public Image circleLoad_left;
	public Image circleLoad_right;

	private float x_Rotation, y_Rotation;
	void Update () {
/*
 		x_Rotation -= Input.gyro.rotationRate.x;
		y_Rotation -= Input.gyro.rotationRate.y;

		transform.rotation = Quaternion.Euler(x_Rotation, y_Rotation, 0);
*/
		if(Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 100.0f)){
			if(hit.transform.tag == "Respawn" && buttonFill < 1){
				buttonFill += 0.5f*Time.deltaTime;

				if(buttonFill >= 1){
					StartCoroutine("LoadLevel","_Drone_VR");
				}
			}
			else{
				buttonFill = 0;
			}
		}
		else{
			buttonFill = 0;
		}

		circleLoad_left.fillAmount = buttonFill;
		circleLoad_right.fillAmount = buttonFill;
	}

	IEnumerator LoadLevel(string _levelName){
		yield return new WaitForSeconds(2);
		SceneManager.LoadScene(_levelName);
	}
}
