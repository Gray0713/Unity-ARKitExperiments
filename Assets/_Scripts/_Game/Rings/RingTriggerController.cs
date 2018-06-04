using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTriggerController : MonoBehaviour 
{
    private enum TriggerType
    {
        Enter,
        Exit
    }
    [SerializeField] private TriggerType type;
	// NOTIFIER
    private Notifier notifier;
    public const string ON_RING_ENTER = "OnRingEnter";
	public const string ON_RING_EXIT = "OnRingExit";

	private void Start()
    {
		// NOTIFIER
		notifier = new Notifier();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (type == TriggerType.Enter)
        {
			//Debug.Log(other.name + " triggered enter!");
            notifier.Notify(ON_RING_ENTER, this.transform.parent);
        }
    }
	private void OnTriggerExit(Collider other)
	{
        if (type == TriggerType.Exit)
		{
            //Debug.Log(other.name + " triggered exit!");
            notifier.Notify(ON_RING_EXIT, this.transform.parent);
		}
	}
    private void OnDestroy()
    {
        if (notifier != null)
        {
            notifier.UnsubcribeAll();
        }
    }
}
