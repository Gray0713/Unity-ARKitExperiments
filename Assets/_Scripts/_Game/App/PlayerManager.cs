using System.Collections;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
	// Player
	[SerializeField] private string playerTag = "Player";
	private GameObject player;
	// NOTIFIER
	private Notifier notifier;
    public const string ON_END_FIND_PLAYER = "OnEndFindPlayer";
	public GameObject Player
	{
		get { return player; }
		set { player = value; }
	}
	// Use this for initialization
	void Start()
	{
		// NOTIFIER
		notifier = new Notifier();
		notifier.Subscribe(CharacterManager.ON_END_CHARACTER_SELECT, HandleGetPlayer);
	}
    private void HandleGetPlayer(params object[] args)
    {
        //Debug.Log("Looking after Player GO.");
        player = GameObject.FindGameObjectWithTag(playerTag);
        notifier.Notify(ON_END_FIND_PLAYER);
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
