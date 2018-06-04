using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private float playerHeight;
    [SerializeField] private float heightOffset;
    [SerializeField] private Transform virtualWorld;
	[SerializeField] private GameObject sardinePrefab;
	private Transform player;
    private Transform cameraTransform;
	private DroneMovementScript playerControl;
    // NOTIFIER
    private Notifier notifier;
	public const string ON_OBJECT_PLACED = "OnObjectPlaced";

    // Use this for initialization
    void Start()
    {
		// NOTIFIER
		notifier = new Notifier();
        notifier.Subscribe(GameManager.ON_START_PLACE, HandleStartPlacing);
        notifier.Subscribe(HelipadController.ON_HELIPAD_PLACED, HandleOnHelipadPlaced);
        notifier.Subscribe(HelipadController.ON_RETURN_TO_HELIPAD, HandleOnReturnHelipad);
		// First OnEnable
		//player.gameObject.SetActive(false);
		cameraTransform = Camera.main.transform;
		virtualWorld.gameObject.SetActive(false);
    }

    private void HandleStartPlacing(params object[] args)
    {
		player = PlayerManager.Instance.Player.transform;
		playerControl = player.GetComponent<DroneMovementScript>();
		SetActivePlayObjects(false);
	}
    private void HandleOnHelipadPlaced(params object[] args)
    {
		Vector3 position = (Vector3)args[0];
        float YRot = (float)args[1];
        position.y += heightOffset;
        PlacePlane(position,YRot);
        position.y += playerHeight;
        PlacePlayer(position,YRot);
	}
	private void HandleOnReturnHelipad(params object[] args)
	{
		Vector3 position = (Vector3)args[0];
		float YRot = (float)args[1];
		position.y += heightOffset;
		position.y += playerHeight;
		PlacePlayer(position, YRot);
	}
    public void SetActivePlayObjects(bool value)
    {
        virtualWorld.gameObject.SetActive(value);
        if (player)
        {
            player.gameObject.SetActive(value);    
        }
    }
    private void CreateSardine(Vector3 atPosition)
    {
        Instantiate(sardinePrefab, atPosition, Random.rotationUniform);
    }
    private void PlacePlayer(Vector3 atPosition, float YRot)
    {
		if (!player.gameObject.activeSelf)
        {
            player.gameObject.SetActive(true);
        }

		player.position = atPosition;
        player.eulerAngles = new Vector3(0.0f, YRot, 0.0f);
        playerControl.currentYRotation = YRot;
        playerControl.wantedYRotation = YRot;
        player.GetComponent<Rigidbody>().useGravity = true;
        CreateSardine(atPosition);
	}
    private void PlacePlane(Vector3 atPosition, float YRot)
    {
		if (!virtualWorld.gameObject.activeSelf)
		{
			virtualWorld.gameObject.SetActive(true);
		}
        virtualWorld.position = atPosition;
        virtualWorld.eulerAngles = new Vector3(0.0f, YRot, 0.0f);
		notifier.Notify(ON_OBJECT_PLACED);
	}
    void OnDestroy()
    {
        if (notifier != null)
        {
            notifier.UnsubcribeAll();
        }
    }
#if UNITY_EDITOR
	private float GetCameraYAngle()
	{
		float angle = cameraTransform.eulerAngles.y;
		return angle;
	}
	public void PlacedMOCKAction()
	{
		player = PlayerManager.Instance.Player.transform;
		playerControl = player.GetComponent<DroneMovementScript>();
        Vector3 position = new Vector3(0.0f, 0.0f, 1.0f);
		float YRot = GetCameraYAngle();
		PlacePlayer(
			new Vector3(position.x, position.y + heightOffset + playerHeight, position.z),
			YRot);
		PlacePlane(
			new Vector3(position.x, position.y + heightOffset, position.z),
			YRot);
	}
#endif
	// Update is called once per frame
	//void Update()
	//{
	//    if (Input.touchCount > 0)
	//    {
	//        var touch = Input.GetTouch(0);
	//        if (touch.phase == TouchPhase.Began)
	//        {
	//            var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
	//            ARPoint point = new ARPoint
	//            {
	//                x = screenPosition.x,
	//                y = screenPosition.y
	//            };

	//            List<ARHitTestResult> hitResults =
	//                UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point,
	//                ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
	//            if (hitResults.Count > 0)
	//            {
	//                foreach (var hitResult in hitResults)
	//                {
	//                    Vector3 position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
	//                    float YRot = GetCameraYAngle();
	//                    PlacePlayer(
	//                        new Vector3(position.x, position.y + heightOffset + playerHeight, position.z),
	//                        YRot);
	//                    PlacePlane(
	//                        new Vector3(position.x, position.y + heightOffset, position.z),
	//                        YRot);
	//                    break;
	//                }
	//            }
	//        }
	//    }
	//}
}