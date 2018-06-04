using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingDistancePanelController : MonoBehaviour 
{
    [SerializeField] private Text labelText;
    [SerializeField] private Text distanceText;
    private int index;
	public int Index
	{
		get { return index; }
		set { index = value; }
	}
    public void SetLabel(string label)
    {
        labelText.text = label;
    }
    public void UpdateDistance(float distance)
    {
		distanceText.text = distance.ToString((distance < 9.99f) ? "F2" : "F1") + " m";
    }
}
