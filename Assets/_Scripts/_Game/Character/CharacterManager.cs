using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : Singleton<CharacterManager> 
{
    [SerializeField] private Character[] characters;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Canvas sectionCanvas;
    [SerializeField] private Text characterName;
    public GameObject selectionCam;
    //public GameObject selectionLights;
    private int length; 
    private int currentIndex = 0;
    private Character currentCharacter;
	// NOTIFIER
	private Notifier notifier;
    public const string ON_END_CHARACTER_SELECT = "OnEndCharSelect";

	// Use this for initialization
	void Start()
    {
        // NOTIFIER
        notifier = new Notifier();
        notifier.Subscribe(AppManager.ON_START_SELECTION, HandleStart);
        notifier.Subscribe(ON_END_CHARACTER_SELECT, HandleEnd);
        // Enables
        //selectionCam.enabled = false;
        selectionCam.SetActive(false);
        sectionCanvas.enabled = false;
        length = characters.Length;
	}
    // BUTTON ACTIONS
    public void NextCharacterAction()
    {
        SetCurrentCharacter(++currentIndex);
    }
	public void PrevCharacterAction()
	{
        SetCurrentCharacter(--currentIndex);
	}
    public void SelectCharacterAction()
    {
        notifier.Notify(ON_END_CHARACTER_SELECT);
    }
    // Character Selection Loop
	private void SetCurrentCharacter(int index)
	{
        if (index < 0)
        {
            index = length - 1;
        }
        else if (index >= length)
        {
            index = 0;
        }
		if (currentCharacter != null)
		{
			Destroy(currentCharacter.gameObject);
		}
		currentCharacter = Instantiate<Character>(
			characters[index], spawnPoint.position, spawnPoint.rotation);
        characterName.text = currentCharacter.type.ToString().ToUpper();
		currentIndex = index;
        //Debug.Log(this.name + ". Current Index: " + currentIndex);
	}
    // Start Character selection section
	private void HandleStart(params object[] args)
    {
        sectionCanvas.enabled = true;
        //selectionCam.enabled = true;
        selectionCam.SetActive(true);
        SetCurrentCharacter(currentIndex);
    }
	// End Character selection section
	private void HandleEnd(params object[] args)
    {
        sectionCanvas.enabled = false;
	}
	// Update is called once per frame
	void Update () 
    {
		
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
