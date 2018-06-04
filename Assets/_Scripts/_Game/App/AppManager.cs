using System.Collections;
using UnityEngine;

public class AppManager : Singleton<AppManager> 
{
    // App State
    private enum AppState
    {
        Opening,
		Selection, 
        Tutorial,
		Game,
        Ending
    }
    [SerializeField] private AppState currentState = AppState.Opening;
    // NOTIFIER
    private Notifier notifier;
	public const string ON_START_OPENING = "OnStartOpening";
	public const string ON_START_SELECTION = "OnStartSelection";
	public const string ON_START_TUTORIAL = "OnStartTutorial";
    public const string ON_START_GAME = "OnStartGame";
    public const string ON_START_ENDING = "OnStartEnding";
	
    // Use this for initialization
	void Start () 
    {
        notifier = new Notifier();
		notifier.Subscribe(ScreenController.ON_PRESS_ENTER, HandleBeginSelection);
        notifier.Subscribe(SkinManager.ON_END_SKIN_SELECT, HandleBeginTutorial);
        notifier.Subscribe(ScreenController.ON_CLOSE_HELP, HandleBeginGame);
        notifier.Subscribe(ScreenController.ON_EXIT_GAME, HandleBeginOpening);
		StartCoroutine(StartApp());
	}
    private IEnumerator StartApp()
    {
        yield return null;
        HandleBeginOpening();
	}
    private void HandleBeginSelection(params object[] args)
	{
        ChangeState(AppState.Selection);
	}
	private void HandleBeginTutorial(params object[] args)
	{
        ChangeState(AppState.Tutorial);
	}
	private void HandleBeginGame(params object[] args)
	{
        ChangeState(AppState.Game);
	}
	private void HandleBeginEnding(params object[] args)
	{
        ChangeState(AppState.Ending);
	}
	private void HandleBeginOpening(params object[] args)
	{
        ChangeState(AppState.Opening);
	}
    private void ChangeState(AppState state)
	{
		//Debug.Log(this.name + ". State: " + state);
		currentState = state;
		switch (currentState)
		{
			case AppState.Opening:
				notifier.Notify(ON_START_OPENING);
				break;
            case AppState.Selection:
                notifier.Notify(ON_START_SELECTION);
				break;
            case AppState.Tutorial:
                notifier.Notify(ON_START_TUTORIAL);
				break;
            case AppState.Game:
                notifier.Notify(ON_START_GAME);
				break;
			case AppState.Ending:
				notifier.Notify(ON_START_ENDING);
				break;
			default:
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
