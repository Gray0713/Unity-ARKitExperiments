using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUIController : MonoBehaviour 
{
    public void ExitAction ()
    {
        AppManager.Instance.ChangeScene(0);
    }
}
