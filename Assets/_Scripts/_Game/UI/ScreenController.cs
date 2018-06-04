using System.Collections;
using UnityEngine;

public class ScreenController : MonoBehaviour 
{
    [SerializeField] private Canvas startScreen;
    [SerializeField] private Canvas endScreen;
    [SerializeField] private Canvas helpScreen;
    [SerializeField] private Canvas HUDCanvas;
	// NOTIFIER
	private Notifier notifier;
	public const string ON_PRESS_ENTER = "OnPressEnter";
	public const string ON_CLOSE_HELP = "OnCloseHelp";
	public const string ON_EXIT_GAME = "OnExitGame";

	// Use this for initialization
	void Start () 
    {
		// NOTIFIER
		notifier = new Notifier();
		// AppManager suscriptions
		notifier.Subscribe(AppManager.ON_START_OPENING, HandleOnStartOpening);
		notifier.Subscribe(AppManager.ON_START_SELECTION, HandleOnStartSelection);
        notifier.Subscribe(AppManager.ON_START_TUTORIAL, HandleOnStartTutorial);
        notifier.Subscribe(AppManager.ON_START_GAME, HandleOnStartGame);
		// GameManager suscription
		notifier.Subscribe(GameManager.ON_START_END, HandleOnStartEnding);
	}
    public void EnterGameAction()
    {
        notifier.Notify(ON_PRESS_ENTER);
    }
    public void CloseHelpAction()
    {
        notifier.Notify(ON_CLOSE_HELP);
    }
    public void ExitGameAction()
    {
        notifier.Notify(ON_EXIT_GAME);
    }
	public void TryAgainAction()
	{
        notifier.Notify(ON_CLOSE_HELP);
	}
	private void HandleOnStartOpening(params object[] args)
	{
		Debug.Log(this.name + ". Started: Opening");
		startScreen.enabled = true;
		endScreen.enabled = false;
		helpScreen.enabled = false;
		HUDCanvas.enabled = false;
	}
	private void HandleOnStartSelection(params object[] args)
	{
        Debug.Log(this.name + ". Started: Selection");
        startScreen.enabled = false;
		endScreen.enabled = false;
		helpScreen.enabled = false;
		HUDCanvas.enabled = false;
	}
	private void HandleOnStartTutorial(params object[] args)
	{
        Debug.Log(this.name + ". Started: Tutorial");
        startScreen.enabled = false;
		endScreen.enabled = false;
		helpScreen.enabled = true;
		HUDCanvas.enabled = false;
	}
	private void HandleOnStartGame(params object[] args)
	{
        Debug.Log(this.name + ": Started: Game");
        startScreen.enabled = false;
		endScreen.enabled = false;
		helpScreen.enabled = false;
		HUDCanvas.enabled = true;	
	}
	private void HandleOnStartEnding(params object[] args)
	{
		//Debug.Log(this.name + ": On Placing Handler");
		startScreen.enabled = false;
		endScreen.enabled = true;
		helpScreen.enabled = false;
        HUDCanvas.enabled = false;
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
