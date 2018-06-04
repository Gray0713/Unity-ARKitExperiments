using System.Collections;
using UnityEngine;
using UnityEngine.XR.iOS;

public class PlacingStateController : MonoBehaviour
{
    [SerializeField] private PlanesGenerator planesGenerator;
    [SerializeField] private HelipadController helipadControl;
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private Canvas placingCanvas;
    // NOTIFIER
    private Notifier notifier;
    public const string ON_END_PLACING = "OnEndPlacing";

    // Use this for initialization
    void Start()
    {
        // NOTIFIER
        notifier = new Notifier();
        notifier.Subscribe(AppManager.ON_START_OPENING, HandleOnEndPlacing);
        notifier.Subscribe(GameManager.ON_START_BEGIN, HandleOnEndGame);
		notifier.Subscribe(GameManager.ON_START_PLACE, HandleOnStartPlacing);
        notifier.Subscribe(GameManager.ON_START_FLY, HandleOnEndPlacing);
        notifier.Subscribe(GameManager.ON_START_PLAY, HandleOnEndPlacing);
		notifier.Subscribe(GameManager.ON_START_END, HandleOnEndGame);
		notifier.Subscribe(ObjectPlacer.ON_OBJECT_PLACED, HandleOnObjectPlaced);
        ShowCanvases(false);
    }
    private void HandleOnStartPlacing(params object[] args)
    {
        Debug.Log(this.name + ". Placing started!");
        planesGenerator.enabled = true;
        helipadControl.enabled = true;
        objectPlacer.enabled = true;
        ShowCanvases(false);
	}
    private void HandleOnEndPlacing(params object[] args)
    {
        Debug.Log(this.name + ". Placing finished!");
        planesGenerator.enabled = false;
        helipadControl.enabled = false;
        objectPlacer.enabled = false;
        ShowCanvases(false);
    }
	private void HandleOnEndGame(params object[] args)
	{
		//Debug.Log(this.name + ": On Flying Handler");
        objectPlacer.SetActivePlayObjects(false);
        HandleOnEndPlacing();
	}
    private void HandleOnObjectPlaced(params object[] args)
    {
        ShowCanvases(true);
    }
    private void ShowCanvases(bool show)
    {
        placingCanvas.enabled = show;
	}
    public void AcceptPlacingAction()
    {
        notifier.Notify(ON_END_PLACING);
    }
    void OnDestroy()
    {
        if (notifier != null)
        {
            notifier.UnsubcribeAll();
        }
    }
}
