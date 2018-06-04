using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingController : MonoBehaviour 
{
	[Header("FX")]
	[SerializeField] private GameObject explotionFX;
	[SerializeField] private AudioClip soundFX;
    [Header("Materials")]
    [SerializeField] private Material[] ringMaterials;
    [SerializeField] private Renderer ringMesh;
	private bool inside = false;
    private int index;
	private float distance;
    // NOTIFIER
    private Notifier notifier;
    public const string ON_RING_DESTROYED = "OnRingDestroyed";
    public float Distance
    {
        get { return distance; }
        set { distance = value; }
    }
	public int Index
	{
		get { return index; }
		set { index = value; }
	}
    // Use this for initialization
	void Start () {
		// NOTIFIER
		notifier = new Notifier();
        notifier.Subscribe(RingTriggerController.ON_RING_ENTER, HandleEnterRing);
        notifier.Subscribe(RingTriggerController.ON_RING_EXIT, HandleExitRing);
		// Constructor
		Material material = ringMaterials[Random.Range(0, ringMaterials.Length)];
        ringMesh.material = material;
	}
    public float DistanceTo(Transform target)
    {
        float d = Vector3.Distance(target.position, transform.position);
        this.distance = d; 
        return d;
    }
    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
    private void HandleEnterRing(params object[] args)
	{
        Transform ring = (Transform)args[0];
        //Debug.Log("Entered " + ring.name);
        if (ring == this.transform)
        {
            inside = true;
        }
	}
    private void HandleExitRing(params object[] args)
    {
        Transform ring = (Transform)args[0];
        //Debug.Log("Exited " + ring.name);
        if (ring == this.transform && inside)
		{
            inside = false;
            PlayOneShootFX();
            notifier.Notify(ON_RING_DESTROYED, this);
			Destroy(this.gameObject, 0.25f);
		}
    }
    private void PlayOneShootFX()
    {
		AudioManager.Instance.PlayOneShoot2D(soundFX);
        Instantiate(explotionFX, this.transform.position, this.transform.rotation);
	}
	private void OnDestroy()
	{
		if (notifier != null)
		{
			notifier.UnsubcribeAll();
		}
	}
}
