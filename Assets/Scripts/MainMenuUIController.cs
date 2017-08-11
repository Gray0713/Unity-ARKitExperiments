using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour {

	public void SelectSceneAction(int scene)
	{
        AppManager.Instance.ChangeScene(scene);
	}

	public void QuitAction()
	{
		AppManager.Instance.QuitApplication();
	}
}
