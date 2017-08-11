using UnityEngine;
using System.Collections;

public class DomeDroneScript : FlyingDroneSpecificScript {

	public Transform blade1;
	public Transform blade2;
	public Transform blade3;
	public Transform blade4;
	public float rotRate = 1000.0f;
	private bool isOnGround;
	private float distFromGround;
	private float heightOfGround;
	private float yScale;
	private float rayStart = 0.4f;
	private float rayLength = 0.3f;
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
            blade1.Rotate(Vector3.up * Time.deltaTime * rotRate);
			blade2.Rotate(Vector3.up * Time.deltaTime * rotRate);
			blade3.Rotate(Vector3.up * Time.deltaTime * rotRate);
			blade4.Rotate(Vector3.up * Time.deltaTime * rotRate);
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
