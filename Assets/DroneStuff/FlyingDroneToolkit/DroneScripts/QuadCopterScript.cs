using UnityEngine;
using System.Collections;

public class QuadCopterScript : FlyingDroneSpecificScript {
	
	public Transform blade1;
	public Transform blade2;
	public Transform blade3;
	public Transform blade4;
	public float rotRate = 1000.0f;
	private bool isOnGround;
	private float distFromGround;
	private float heightOfGround;
	private float yScale;
	public float rayStart = 0.0f;
	public float rayLength = 0.2f;
    private bool soundPlaying = true;
	
	void Start () {
		yScale = transform.localScale.y;
	}
	
	public override bool OnGround() {
		return isOnGround;
	}

	public override float DistanceFromGround() {
		return distFromGround;
	}
	
	public override float HeightOfGround() {
		return heightOfGround;
	}
	
	void Update () {
		CastRayToGround (rayStart, rayLength, yScale, out isOnGround, out distFromGround, out heightOfGround);
		if (!isOnGround)
		{
            if (!soundPlaying)
            {
                GetComponent<AudioSource>().Play();
                soundPlaying = true;
            }
			Vector3 rot = -1.0f * Vector3.forward * Time.deltaTime * rotRate;
			blade1.Rotate(rot);
			blade2.Rotate(rot);
			blade3.Rotate(rot);
			blade4.Rotate(rot);
        }
        else  // isOnGround
        { 
            if (soundPlaying)
            {
                GetComponent<AudioSource>().Stop();
                soundPlaying = false;
            }
        }
	}
	
}
