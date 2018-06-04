using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jaime.Legacy
{
    public class GameUIController : MonoBehaviour
	{
		public void ExitAction()
		{
			AppManager.Instance.ChangeScene(0);
		}
	}

} 