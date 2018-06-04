using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager> 
{
    // Game State
    public enum Mode
    {
        Free,
        Rings
    }
	public enum GameState
	{
        Begin,
        Find,
        Place,
        FreeFly,
        Play,
		Flyback,
		End
	}
    [SerializeField] private Mode currentMode;
    [SerializeField] private GameState currentState;
    [SerializeField] private GameObject gameLights;
    public Mode CurrenMode
    {
        get { return currentMode; }
        set { currentMode = value; }
    }
	// NOTIFIER
	private Notifier notifier;
	public const string ON_START_BEGIN = "OnStartBegin";
	public const string ON_START_FIND = "OnStartFind";
	public const string ON_START_PLACE = "OnStartPlace";
	public const string ON_START_FLY = "OnStartFly";
	public const string ON_START_PLAY = "OnStartPlay";
	public const string ON_START_FLYBACK = "OnStartFlyback";
	public const string ON_START_END = "OnStartEnd";

    void Start()
    {
        // NOTIFIER
        notifier = new Notifier();
        notifier.Subscribe(AppManager.ON_START_OPENING, HandleBegin);
        notifier.Subscribe(AppManager.ON_START_GAME, HandlePlacing);
        notifier.Subscribe(PlacingStateController.ON_END_PLACING, HandleOnEndPlacing);
		notifier.Subscribe(FlyingStateController.ON_PLACE_AGAIN, HandlePlacing);
		notifier.Subscribe(HUDController.ON_BATTERY_DEAD, HandleEnd);
        notifier.Subscribe(ScreenController.ON_EXIT_GAME, HandleBegin);
		Init();
	}
    private void Init()
    {
        gameLights.SetActive(false);
    }
	private void HandleBegin(params object[] args)
	{
        ChangeState(GameState.Begin);
	}
	private void HandlePlacing(params object[] args)
    {
        ChangeState(GameState.Place);
    }
    private void HandleOnEndPlacing(params object[] args)
    {
        switch(currentMode)
        {
            case Mode.Free:
                ChangeState(GameState.FreeFly);
                break;
            case Mode.Rings:
                ChangeState(GameState.Play);
                break;
        }
	}
   private void HandleEnd(params object[] args)
	{
        ChangeState(GameState.End);
    }
    private void ChangeState(GameState state) 
    {
		//Debug.Log(this.name + ". State: " + state);
        currentState = state;
		//Debug.Log(currentState);
		switch (currentState) 
        {
            case GameState.Begin:
                notifier.Notify(ON_START_BEGIN);
                break;
            case GameState.Find:
				notifier.Notify(ON_START_FIND);
				break;
            case GameState.Place:
                gameLights.SetActive(true);
                notifier.Notify(ON_START_PLACE);
				break;
            case GameState.FreeFly:
                notifier.Notify(ON_START_FLY);
                break;
			case GameState.Play:
				notifier.Notify(ON_START_PLAY);
				break;
			case GameState.Flyback:
                notifier.Notify(ON_START_FLYBACK);
                break;
           case GameState.End:
                gameLights.SetActive(false);
                notifier.Notify(ON_START_END);
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
