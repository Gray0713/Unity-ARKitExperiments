using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrientationIndicatorController : MonoBehaviour 
{
    [SerializeField] private GameObject orientationPanel;
    [SerializeField] private Transform indicatorImage;
    private Transform player;
    private new Transform camera;
    private new bool enabled;
	// NOTIFIER
	private Notifier notifier;

	// Use this for initialization
	void Start () 
    {
		// NOTIFIER
		notifier = new Notifier();
        notifier.Subscribe(GameManager.ON_START_BEGIN, HandleEnd);
		notifier.Subscribe(GameManager.ON_START_FLY, HandleStart);
        notifier.Subscribe(GameManager.ON_START_PLAY, HandleStart);
        notifier.Subscribe(GameManager.ON_START_END, HandleEnd);
        orientationPanel.SetActive(false);
        enabled = false;
	}
    private void HandleStart(params object[] args)
    {
        player = PlayerManager.Instance.Player.transform;
        camera = Camera.main.transform;
        orientationPanel.SetActive(true);
        enabled = true;
    }
    private void HandleEnd(params object[] args)
    {
		orientationPanel.SetActive(false);
        enabled = false;
	}
	// Update is called once per frame
	void Update ()
    {
        if (enabled)
        {
            Vector3 cameraFwdProj = Vector3.ProjectOnPlane(camera.forward, Vector3.up);
            Vector3 playerFwdProj = Vector3.ProjectOnPlane(player.forward, Vector3.up);
            float angle = Vector3.SignedAngle(
                playerFwdProj, cameraFwdProj, Vector3.up);
            indicatorImage.eulerAngles = new Vector3(0, 0, angle);
        }
	}
}
