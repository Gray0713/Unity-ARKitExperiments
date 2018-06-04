using UnityEngine;
using UnityEngine.UI;

public class HelipadDistance : MonoBehaviour 
{
	[SerializeField] private Text distanceText;
    // Use this for initialization
	void Start () 
    {
		distanceText.text = "0 m";
	}
	// Update is called once per frame
	void Update () 
    {
        float distance = DistanceToPlayer();
        distanceText.text = distance.ToString((distance < 9.99f) ? "F2" : "F1") + " m";
	}
	private float DistanceToPlayer()
	{
        if (PlayerManager.Instance.Player.transform != null)
        {
			Transform player = PlayerManager.Instance.Player.transform;
			float distance = Vector3.Distance(player.position, this.transform.position);
			return distance;
        }
        return 0f;
	}
}
