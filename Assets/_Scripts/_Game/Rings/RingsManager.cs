using System.Collections.Generic;
using UnityEngine;

public class RingsManager : Singleton<RingsManager> 
{
    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private int totalRings;
    [SerializeField] private Vector3 volumeDimensions;
    [SerializeField] private float minDistance;
    [SerializeField] private float heightOffset = 0.5f;
    [SerializeField] private Transform ringPanelParent;
    [SerializeField] private GameObject ringPanelPrefab;
    [SerializeField] private int panelCount = 3;
    private List<RingController> rings;
    private List<RingDistancePanelController> panels;
    private bool started;
    private int ringCount;
    private Vector3 cameraPos;
    private Vector3 cameraEuler;
    private Transform player;
	// NOTIFIER
	private Notifier notifier;

	// Use this for initialization
	void Start () 
    {
        rings = new List<RingController>();
        panels = new List<RingDistancePanelController>();
		// NOTIFIER
		notifier = new Notifier();
        notifier.Subscribe(GameManager.ON_START_BEGIN, HandleEndPlay);
        notifier.Subscribe(GameManager.ON_START_PLAY, HandleStartPlay);
        notifier.Subscribe(GameManager.ON_START_PLACE, HandleEndPlay);
        notifier.Subscribe(GameManager.ON_START_FLY, HandleEndPlay);
		notifier.Subscribe(GameManager.ON_START_END, HandleEndPlay);
        notifier.Subscribe(RingController.ON_RING_DESTROYED, HandleRingDestroyed);
	}
    private void Update()
    {
        if (started)
        {
			for (int i = 0; i < rings.Count; i++)
			{
                int index = rings[i].Index;
				float distance = rings[i].DistanceTo(player);
                if ( i < panels.Count)
                {
                    panels[index].UpdateDistance(distance);    
                }
			}
		}
    }
    public void GetToggleValue(float value)
	{
        bool isRings = value > 0.99f;
        GameManager.Instance.CurrenMode = isRings ? GameManager.Mode.Rings : GameManager.Mode.Free;
	}
    private void HandleStartPlay(params object[] args)
    {
        player = PlayerManager.Instance.Player.transform;
	    InstantiateRings();
        cameraEuler = Camera.main.transform.eulerAngles;
        this.transform.eulerAngles = new Vector3(0, cameraEuler.y, 0);
        started = true; 
    }
	private void HandleEndPlay(params object[] args)
    {
        started = false;
        DestroyRings();
        DestroyRingLabels();
	}
	private void HandleRingDestroyed(params object[] args)
    {
        RingController ring = (RingController)args[0];
        int index = ring.Index;
        Debug.Log(index);
        rings.Remove(ring);
        ringPanelParent.GetChild(index).gameObject.SetActive(false);
        ringCount--;
        if (ringCount <= 0) 
        {
            DestroyRingLabels();
            InstantiateRings();
        }
    }
    private void InstantiateRings()
    {
        for (int i = 0; i < totalRings; i++)
        {
			float width = Random.Range(-volumeDimensions.x/2, volumeDimensions.x/2);
			width = width + minDistance;
			float height = Random.Range(0, volumeDimensions.y);
            height = height + heightOffset;
			float depth = Random.Range(0, volumeDimensions.z);
            depth = depth + minDistance;
            cameraPos = Camera.main.transform.position; 
            Vector3 position = new Vector3(cameraPos.x + width, cameraPos.y + height, cameraPos.z + depth);
            GameObject newRingGO = Instantiate(ringPrefab, position, Quaternion.identity, this.transform);
            RingController newRing = newRingGO.GetComponent<RingController>();
            newRing.Index = i;
            Debug.Log("New ring index: " + newRing.Index);
			rings.Add(newRing);
            ringCount++;
            if(i < panelCount)
            {
                GameObject newPanelGO = Instantiate(ringPanelPrefab, ringPanelParent);
                RingDistancePanelController newPanel = newPanelGO.GetComponent<RingDistancePanelController>();
				newPanel.Index = i;
                Debug.Log("New ring panel index: " + newPanel.Index);
                newPanel.SetLabel("RING " + (i + 1).ToString());
                panels.Add(newPanel);
            }
        }
	}
	private void DestroyRings()
	{
        for (int i = transform.childCount - 1; i >= 0; i--)
		{
			rings.RemoveAt(i);
			Destroy(transform.GetChild(i).gameObject);
		}
	}
	private void DestroyRingLabels()
	{
        for (int i = ringPanelParent.childCount - 1; i >= 0; i--)
		{
			panels.RemoveAt(i);
			Destroy(ringPanelParent.GetChild(i).gameObject);
		}
	}
	private void ShowRings(bool value)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(value);
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