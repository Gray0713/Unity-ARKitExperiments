using UnityEngine;
using System.Collections;

public class FlyingSaucerScript : FlyingDroneSpecificScript {
	
	public float rotRate = 100.0f;
	private bool isOnGround;
	private float distFromGround;
	private float heightOfGround;
	private float yScale;
	private float rayStart = -10.0f;
	private float rayLength = 10.0f;
	private Transform assembly;
    private bool soundPlaying = true;
	
	void Start () {
		yScale = transform.localScale.y;
		assembly = transform.GetChild(0);
		isOnGround = false;
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
            assembly.Rotate(Vector3.up * Time.deltaTime * rotRate);
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
