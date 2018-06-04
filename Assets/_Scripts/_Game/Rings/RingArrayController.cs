using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingArrayController : MonoBehaviour 
{
    [SerializeField] private GameObject ringPrefab;
    public bool showRings = false;
    private int totalRings;
    private int ringCount;
    private Vector3[] ringPositions;
	// NOTIFIER
	private Notifier notifier;

	// Use this for initialization
	void Start () 
    {
		// NOTIFIER
		notifier = new Notifier();
        notifier.Subscribe(RingController.ON_RING_DESTROYED, HandleRingDestroyed);
        totalRings = transform.childCount;
        ringPositions = new Vector3[totalRings];
        ringCount = totalRings;
        SaveRingPositions();
        ShowRings(showRings);
	}
	public void GetToggleValue(float value)
	{
        showRings = value > 0.99f;
        ShowRings(showRings);
	}
    private void ShowRings(bool show)
    {
		for (int i = 0; i < totalRings; i++)
		{
            transform.GetChild(i).gameObject.SetActive(show);
		}
    }
    private void HandleRingDestroyed(params object[] args)
    {
        ringCount--;
        if (ringCount <= 0) 
        {
            InstantiateRings();
        }
    }
    private void SaveRingPositions()
    {
        for (int i = 0; i < totalRings; i++)
        {
            ringPositions[i] = transform.GetChild(i).position;
        }
    }
    private void InstantiateRings()
    {
        for (int i = 0; i < totalRings; i++)
        {
            Instantiate(ringPrefab, ringPositions[i], Quaternion.identity, this.transform);
            ringCount++;
        }
    }
	// Update is called once per frame
	void Update () 
    {	
	}
}
