using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUIController : MonoBehaviour 
{
	[SerializeField] private Transform optionsUIParent;
    [SerializeField] private GameObject SliderOptionPrefab;
    [SerializeField] private GameObject ToggleOptionPrefab;
    [SerializeField] private Toggle optionsToggle;
	// NOTIFIER
	private Notifier notifier;
	public Dictionary<Option, OptionPanelController> optionPanelRel;
	private void Start()
    {
        optionPanelRel = new Dictionary<Option, OptionPanelController>();
		// NOTIFIER
		notifier = new Notifier();
        notifier.Subscribe(GameManager.ON_START_BEGIN, HandleOnDisable);
        notifier.Subscribe(GameManager.ON_START_PLACE, HandleOnDisable);
		notifier.Subscribe(GameManager.ON_START_END, HandleOnDisable);
		notifier.Subscribe(GameManager.ON_START_FLY, HandleOnEnable);
		notifier.Subscribe(GameManager.ON_START_PLAY, HandleOnEnable);
        InitOptionsPanels();
    }
    private void InitOptionsPanels ()
    {
		foreach (Option option in OptionsManager.Instance.Options)
		{
			GameObject prefab;
			switch (option.myType)
			{
				case Option.OptionType.Slider:
					prefab = SliderOptionPrefab;
					break;
				case Option.OptionType.Toggle:
					prefab = ToggleOptionPrefab;
					break;
				default:
					prefab = null;
					break;
			}
			GameObject newPanel;
			newPanel = Instantiate(prefab, optionsUIParent);
			OptionPanelController control = newPanel.GetComponent<OptionPanelController>();
			control.SetValues(option);
			optionPanelRel.Add(option, control);
		}
    }
    private void HandleOnEnable(params object[] args)
    {
        optionsToggle.interactable = true;
    }
    private void HandleOnDisable(params object[] args)
    {
        optionsToggle.interactable = false;
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