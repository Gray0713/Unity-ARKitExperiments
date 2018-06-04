using UnityEngine;
using System.Collections;

public class SampleSceneGUI : MonoBehaviour {

    // Main toolbar.
	private int toolbarInt = -1;
	private int saveToolbarInt = -1;
	private string[] toolbarStrings = {"Hover", "Manual", "Land", "Patrol", "Follow Saucer", "Follow Kyle"};
	private Rect toolbarRect;
	private bool changed=false;
    public GameObject theDrone;
    public GameObject saucerDrone;
    public GameObject Kyle;

    // Camera toolbar.
    private int camToolbarInt = -1;
    private int camSaveToolbarInt = -1;
    private string[] camToolbarStrings = { "Saucer Cam", "Kyle Cam", "No Cam" };
    private Rect camToolbarRect;
    private bool camChanged = false;
    public GameObject saucerCam;
    public GameObject KyleCam;

	private FlyingDroneScript drScript;

	void Start()
	{
		drScript = theDrone.GetComponent<FlyingDroneScript>();
	}

	void OnGUI () 
	{
        // Manage main toolbar.
		toolbarRect = gameObject.GetComponent<Camera>().pixelRect;
		toolbarRect.xMin += 10;
		toolbarRect.yMin += 10;
		toolbarRect.width -= 20;
		toolbarRect.height = 20;
		toolbarInt = GUI.Toolbar (toolbarRect, toolbarInt, toolbarStrings);
		if (toolbarInt != saveToolbarInt)
			changed = true;
		else
			changed = false;
		saveToolbarInt = toolbarInt;
		if (changed)
		{
			//Debug.Log(string.Format("toolbar changed to {0}", toolbarInt));
			switch(toolbarInt)
			{
			case 0:       // Hover
				drScript.SetHoverMode();
				break;
			case 1:       // Manual
				drScript.SetManualMode();
				break;
			case 2:       // Land
				drScript.SetLandMode();
				break;
			case 3:       // Patrol
				drScript.SetPatrolMode();
				break;
			case 4:       // Follow Saucer
				drScript.followee = saucerDrone;
				drScript.SetFollowMode();
				break;
			case 5:       // Follow Kyle
				drScript.followee = Kyle;
				drScript.SetFollowMode();
				break;
			}
		}

        // Manage camera toolbar.
        camToolbarRect = gameObject.GetComponent<Camera>().pixelRect;
        camToolbarRect.xMin += 100;
        camToolbarRect.yMin = camToolbarRect.height - 40;
        camToolbarRect.width /= 2;
        camToolbarRect.height = 20;
        camToolbarInt = GUI.Toolbar(camToolbarRect, camToolbarInt, camToolbarStrings);
        if (camToolbarInt != camSaveToolbarInt)
            camChanged = true;
        else
            camChanged = false;
        camSaveToolbarInt = camToolbarInt;
        if (camChanged)
        {
            //Debug.Log(string.Format("camToolbar changed to {0}", camToolbarInt));
            switch (camToolbarInt)
            {
                case 0:       // Saucer Cam
                    saucerCam.SetActive(true);
                    KyleCam.SetActive(false);
                    break;
                case 1:       // Kyle Cam
                    saucerCam.SetActive(false);
                    KyleCam.SetActive(true);
                    break;
                case 2:       // No Cam
                    saucerCam.SetActive(false);
                    KyleCam.SetActive(false);
                    break;
            }
        }

    }
		
}
