using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsInterface : MonoBehaviour 
{
    [SerializeField] float mediumForceHover = 98.0f;
	private Transform player;
	private DroneMovementScript playerControl;
    // NOTIFIER
    private Notifier notifier;
    public const string ON_OPTION_UPDATED = "OnOptionUpdated";

	// Use this for initialization
	void Start()
	{
        // NOTIFIER
        notifier = new Notifier();
		notifier.Subscribe(GameManager.ON_START_FLY, HandleStartFly);
        notifier.Subscribe(GameManager.ON_START_PLAY, HandleStartFly);
		notifier.Subscribe(ON_OPTION_UPDATED, HandleOptionUpdated);
	}
	private void HandleStartFly(params object[] args)
    {
		player = PlayerManager.Instance.Player.transform;
		playerControl = player.GetComponent<DroneMovementScript>();        
    }
	private void HandleOptionUpdated(params object[] args)
    {
        Option option = (Option)args[0];
        //Debug.Log(option.name + " updated!");
		switch(option.myType)
		{
			case Option.OptionType.Slider:
				Option.Slider slider = option.mySlider;
				switch (option.name)
				{
					case "Forward/Backward Speed":
                        playerControl.movementForwardSpeed = slider.value;
						break;
					case "Rotation Speed":
                        playerControl.rotationAmount = slider.value;
                        break;
                    case "Side Movement Speed":
                        playerControl.sideMovementAmount = slider.value;
                        break;
					case "Ascend/Descend Speed":
                        playerControl.forceUpHover = mediumForceHover + slider.value;
                        playerControl.forceDownHover = mediumForceHover - slider.value;
						break;
				}
				break;
			case Option.OptionType.Toggle:
				Option.Toggle toggle = option.myToggle;
				switch (option.name)
				{
					case "Game Mode":
                        notifier.Notify(toggle.value ? GameManager.ON_START_PLAY : GameManager.ON_START_FLY);
						break;
				}
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