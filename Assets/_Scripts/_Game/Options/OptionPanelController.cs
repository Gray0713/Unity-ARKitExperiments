using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    [SerializeField] private Text labelText;
    [SerializeField] private Slider slider;
    [SerializeField] private Text valueText;
	// NOTIFIER
	private Notifier notifier;
    private Option option;

    private void Start()
    {
		// NOTIFIER
		notifier = new Notifier();
    }

    public void SetValues(Option option)
	{
        this.option = option;
        string name = option.name;
        switch(option.myType)
        {
			case Option.OptionType.Slider:
                Option.Slider sldr = option.mySlider;
                SetUIValues(name, sldr.minValue, sldr.maxValue, sldr.defValue);
				break;
			case Option.OptionType.Toggle:
                Option.Toggle tggl = option.myToggle;
                SetUIValues(name, tggl.defValue);
				break;
        }
		slider.onValueChanged.AddListener(delegate {
            SliderChangeValue(option.myType);
			notifier.Notify(OptionsInterface.ON_OPTION_UPDATED, this.option);
		});
	}
    public void SetUIValues(string name, bool defValue)
    {
        float sliderValue = defValue ? 1.0f : 0.0f; 
        SetUIValues(name, 0.0f, 1.0f, sliderValue);
	}
	public void SetUIValues(string name, float minValue, float maxValue, float defValue)
	{
        labelText.text = name.ToUpper();
		slider.minValue = minValue;
		slider.maxValue = maxValue;
		slider.value = defValue;
        valueText.text = defValue.ToString().ToUpper();
	}
    public void SetUIValue(bool val)
    {
        slider.value = val ? 1.0f : 0.0f;
		valueText.text = val ? "RINGS" : "FREE";
    }
    private void SliderChangeValue(Option.OptionType type)
    {
        switch(type)
        {
            case Option.OptionType.Slider:
                this.option.mySlider.value = slider.value;
                valueText.text = slider.value.ToString("F1");
                break;
            case Option.OptionType.Toggle:
                bool isOn = slider.value > 0.0f;
                this.option.myToggle.value = isOn;
                valueText.text = isOn ? "RINGS" : "FREE";
                break;
        }
    }
	void OnDestroy()
	{
        // NOTIFIER
		if (notifier != null)
		{
			notifier.UnsubcribeAll();
		}
	}
}