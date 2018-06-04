using System.Collections;
using UnityEngine;

public class FlyingStateController : MonoBehaviour 
{
    [SerializeField] private HUDController HUDControl;
    [SerializeField] private Canvas joystickCanvas;
	[SerializeField] private Canvas flyingCanvas;
    [SerializeField] private GameObject acceptReturnPanel;
	private DroneMovementScript playerControl;
	// NOTIFIER
	private Notifier notifier;
	public const string ON_PLACE_AGAIN = "OnPlaceAgain";
	
    // Use this for initialization
	void Start () 
    {
		// NOTIFIER
		notifier = new Notifier();
        notifier.Subscribe(GameManager.ON_START_BEGIN, HandleOnStartBegin);
		notifier.Subscribe(GameManager.ON_START_END, HandleOnEndFlying);
		notifier.Subscribe(GameManager.ON_START_FLY, HandleOnStartFlying);
        notifier.Subscribe(GameManager.ON_START_PLAY, HandleOnStartFlying);
		notifier.Subscribe(ON_PLACE_AGAIN, HandleOnEndFlying);
        ShowCanvases(false);
	}
    private void HandleOnStartFlying(params object[] args)
    {
		Debug.Log(this.name + ". Flying started!");
		playerControl = PlayerManager.Instance.Player.GetComponent<DroneMovementScript>();
        playerControl.enabled = true;
        ShowCanvases(true);
		acceptReturnPanel.SetActive(false);
	}
	private void HandleOnEndFlying(params object[] args)
	{
		Debug.Log(this.name + ". Flying finished!");
		playerControl.enabled = false;
		ShowCanvases(false);
	}
	private void HandleOnStartBegin(params object[] args)
	{
		Debug.Log(this.name + ". Start begin!");
        ShowCanvases(false);
	}
    private void ShowCanvases(bool show)
    {
		joystickCanvas.enabled = show;
        flyingCanvas.enabled = show;
		// Other
        HUDControl.enabled = show;
	}
    // RETURN TO PLACING
	public void PlacingButtonAction()
    {
        acceptReturnPanel.SetActive(true);
    }
    public void AcceptPlacingAction()
    {
		notifier.Notify(ON_PLACE_AGAIN);
	}
	void OnDestroy()
	{
		if (notifier != null)
		{
			notifier.UnsubcribeAll();
		}
	}
}
