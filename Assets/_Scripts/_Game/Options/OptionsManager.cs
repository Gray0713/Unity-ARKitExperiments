using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : Singleton<OptionsManager> 
{
	[SerializeField] private OptionsUIController UIControl;
	[SerializeField] private Option[] options;
	// NOTIFIER
	private Notifier notifier;
	public Option[] Options
	{
        get { return options; }
	}
	// Use this for initialization
	void Start () 
    {
		// NOTIFIER
		notifier = new Notifier();
        notifier.Subscribe(GameManager.ON_START_BEGIN, HandleStartGame);
        notifier.Subscribe(GameManager.ON_START_PLACE, HandleStartGame);
	}
    private void HandleStartGame(params object[] args)
    {
        ReadValues();
    }
	private void ReadValues()
	{
        for (int i = 0; i < options.Length; i++)
		{
            Option option = options[i];
            OptionPanelController panel = UIControl.optionPanelRel[option];
			switch (option.myType)
			{
				case Option.OptionType.Slider:
					break;
				case Option.OptionType.Toggle:
					Option.Toggle toggle = option.myToggle;
					switch (option.name)
					{
						case "Game Mode":
                            Debug.Log(option.name + toggle.defValue);
                            toggle.defValue = GameManager.Instance.CurrenMode == GameManager.Mode.Rings;
                            Debug.Log(option.name + toggle.defValue);
                            panel.SetUIValue(toggle.defValue);
							break;
					}
					break;
			}
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
