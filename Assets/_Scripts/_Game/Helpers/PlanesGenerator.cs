using System;
using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
	public class PlanesGenerator : MonoBehaviour
	{
		[SerializeField] private GameObject planePrefab;
        private UnityARAnchorManager unityARAnchorManager; 
        private Transform planesParent;

		// Use this for initialization
		void Start()
		{
			unityARAnchorManager = new UnityARAnchorManager();
            planesParent = transform;
			UnityARUtility.InitializePlanePrefab(planePrefab);
            // UnityARUtility.InitializePlanesParent(planesParent);
		}
        private void OnEnable()
        {
			UnityARUtility.InitializePlanePrefab(planePrefab);
			int cl = transform.childCount;
            for (int i = 0; i < cl; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        private void OnDisable()
        {
            UnityARUtility.InitializePlanePrefab(null);
			int cl = transform.childCount;
			for (int i = 0; i < cl; i++)
			{
                transform.GetChild(i).gameObject.SetActive(false);
			}
        }
        void Update()
        {
            
        }
        void OnDestroy()
		{
			unityARAnchorManager.Destroy();
		}
	}
}

