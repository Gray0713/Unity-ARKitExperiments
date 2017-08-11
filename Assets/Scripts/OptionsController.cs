using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour {

	//class LightControl {
	//    float rotationValue;
	//    Slider slider;
	//    Text displayText;
	//}
	//[SerializeField] private LightControl lc;
	//[SerializeField] private bool startActive;
	[SerializeField] private Toggle toggle;
    [SerializeField] private Slider slider;
    [SerializeField] private Text displayText;
    [SerializeField] private Transform lightsTransform;

    private float rotationValue;

	void Awake () 
    {
        bool startActive = toggle.isOn;
        gameObject.SetActive(startActive);
	}

    public void OnSliderChanged() 
    {
        rotationValue = slider.value;
        displayText.text = rotationValue.ToString();
        lightsTransform.eulerAngles = new Vector3(0, rotationValue, 0);
    }
	
	void Update () {
		
	}
}
