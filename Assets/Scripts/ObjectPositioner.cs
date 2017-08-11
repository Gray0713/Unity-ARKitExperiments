using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ObjectPositioner : MonoBehaviour {

    [SerializeField] Transform targetObject;
    [SerializeField] float heightOffset;

	// Use this for initialization
	void Start () {
        targetObject.gameObject.SetActive(false);
	}

    void MoveObject(Vector3 position) 
    {
        if (!targetObject.gameObject.activeSelf)
        {
            targetObject.gameObject.SetActive(true);
        }
        targetObject.position = position;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0 )
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                ARPoint point = new ARPoint
                {
                    x = screenPosition.x,
                    y = screenPosition.y
                };

                List<ARHitTestResult> hitResults = 
                    UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, 
                    ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
                if (hitResults.Count > 0) 
                {
                    foreach (var hitResult in hitResults)
                    {
                        Vector3 position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                        MoveObject(new Vector3(position.x, position.y + heightOffset, position.z));
                        break;
                    }    
                }
            }
        }
	}
}
