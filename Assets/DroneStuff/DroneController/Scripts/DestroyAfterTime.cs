using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {

	void Awake(){
		Destroy (gameObject, 2);
	}
}
