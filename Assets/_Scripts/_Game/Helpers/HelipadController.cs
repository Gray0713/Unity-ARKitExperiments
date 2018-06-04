using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelipadController : MonoBehaviour 
{
    [SerializeField] private LayerMask raycastMask = -1;
	[SerializeField] private string defaultPlaneTag;
    [SerializeField] private string anchorPlaneTag;
	[SerializeField] private float rayDistance;
    [SerializeField] private float heightOffset;
    private Transform cameraTransform;
    private float lastYRot = 0.0f;
    private bool onPlane = false;
    private bool onPlace = false;
    //private new Renderer renderer;
    private Renderer[] children;
    //private Material material;
    private new Collider collider;
	// NOTIFIER
	private Notifier notifier;
	public const string ON_HELIPAD_PLACED = "OnHelipadPlaced";
    public const string ON_RETURN_TO_HELIPAD = "OnReturnToHelipad";

    // Use this for initialization
    private void Awake()
    {
		collider = GetComponent<Collider>();
		//renderer = GetComponent<Renderer>();
        children = GetComponentsInChildren<Renderer>();
    }
    void Start () 
    {
		// NOTIFIER
		notifier = new Notifier();
        //notifier.Subscribe(GameManager.ON_START_PLACE, HandleAppear);
        notifier.Subscribe(GameManager.ON_START_END, HandleDisappear);
        //collider = GetComponent<Collider>();
        //renderer = GetComponent<Renderer>();
        //material = renderer.material;
		cameraTransform = Camera.main.transform;
		HandleDisappear();
	}
    private void HandleAppear(params object[] args)
    {
        collider.enabled = true;
		//renderer.enabled = true;
		foreach(Renderer child in children)
        {
            child.enabled = true;    
        }
    }
    private void HandleDisappear(params object[] args)
    {
        collider.enabled = false;
        //renderer.enabled = false;
		foreach (Renderer child in children)
		{
            child.enabled = false;
		}
    }
    private void OnEnable()
    {
        onPlace = false;
        HandleAppear();
    }
    private void OnDisable()
    {
        //HandleDisappear();
    }
    // Update is called once per frame
    void Update()
    {
        if (!onPlace)
        {
			float deltaYRot = cameraTransform.eulerAngles.y - lastYRot;
			lastYRot = cameraTransform.eulerAngles.y;
			transform.RotateAround(cameraTransform.position, Vector3.up, deltaYRot);    
        }
    }
	void FixedUpdate()
	{
        if (!onPlace)
        {
			Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
			Ray ray = Camera.main.ScreenPointToRay(screenCenter);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, rayDistance, raycastMask))
			{
				Debug.Log("Plane detected: " + hit.transform.gameObject.name);
				Debug.DrawLine(cameraTransform.position, hit.point);
				if (hit.collider.tag == defaultPlaneTag)
				{
					transform.position = new Vector3(hit.point.x, hit.point.y + heightOffset, hit.point.z);
				}
				if (hit.collider.tag == anchorPlaneTag)
				{
					transform.position = new Vector3(hit.point.x, hit.point.y + heightOffset, hit.point.z);
                    //material.color = new Color(1.0f, 1.0f, 1.0f, 0.95f);
                    onPlane = true;
				}
				else
				{
                    //material.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
				}
			}
			else
			{
                //material.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
			}
        }

	}
    public void OnMouseDown()
    {
        if (onPlane && this.enabled)
        //if (onPlane)
        {
			float YRot = cameraTransform.eulerAngles.y;
			onPlace = true;
			notifier.Notify(ON_HELIPAD_PLACED, transform.position, YRot);   
        }
    }
    public void ReturnToHelipadAction()
    {
        Debug.Log("Return to Helipad");
		if (onPlane)
		{
			float YRot = cameraTransform.eulerAngles.y;
			notifier.Notify(ON_RETURN_TO_HELIPAD, transform.position, YRot);
		}
    }
}
