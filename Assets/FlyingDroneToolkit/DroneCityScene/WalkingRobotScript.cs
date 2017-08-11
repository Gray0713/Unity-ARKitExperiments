using UnityEngine;
using System.Collections;

public class WalkingRobotScript : MonoBehaviour {

    private Animator anim;

	void Start () {
        anim = GetComponent<Animator>();
	}
	
	void FixedUpdate () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        anim.SetFloat("Direction", h);
        anim.SetFloat("Speed",     v);
    }
}
