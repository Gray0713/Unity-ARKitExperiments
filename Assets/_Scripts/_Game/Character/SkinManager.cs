using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : Singleton<SkinManager> 
{
    //[SerializeField] private Texture2D[] textures;
    //[SerializeField] private Material[] materials;
    [SerializeField] private Canvas sectionCanvas;
	private int length;
	private int currentIndex = 0;
    //private Texture currentTexture;
    private Material currentMaterial;
    private Character player;
    [SerializeField] private Material[] materials;
    private GameObject selectionCam;
	// NOTIFIER
	private Notifier notifier;
	public const string ON_END_SKIN_SELECT = "OnEndSkinSelect"; 
	// Use this for initialization
	void Start () 
    {
		// NOTIFIER
		notifier = new Notifier();
        notifier.Subscribe(PlayerManager.ON_END_FIND_PLAYER, HandleStart);
        notifier.Subscribe(ON_END_SKIN_SELECT, HandleEnd);
        // Enables
        sectionCanvas.enabled = false;
        //length = textures.Length;
        //length = materials.Length;
	}
	// BUTTON ACTIONS
	public void NextTextureAction()
	{
		SetCurrentTexture(++currentIndex);
	}
	public void PrevTextureAction()
	{
		SetCurrentTexture(--currentIndex);
	}
	public void SelectTextureAction()
	{
		notifier.Notify(ON_END_SKIN_SELECT);
	}
	// Texture Selection Loop
	private void SetCurrentTexture(int index)
	{
		if (index < 0)
		{
			index = length - 1;
		}
		else if (index >= length)
		{
			index = 0;
		}
        //currentTexture = textures[index];
        currentMaterial = materials[index];
        player.ChangeBodyMaterial(currentMaterial);
		currentIndex = index;
		//Debug.Log(this.name + ". Current Index: " + currentIndex);
	}
    // Start Texture selection section
	private void HandleStart(params object[] args)
	{
		sectionCanvas.enabled = true;
        selectionCam = CharacterManager.Instance.selectionCam;
		player = PlayerManager.Instance.Player.GetComponent<Character>();
        materials = player.bodyMaterials;
        length = materials.Length;
		SetCurrentTexture(currentIndex);
	}
	// End Character selection section
	private void HandleEnd(params object[] args)
	{
        sectionCanvas.enabled = false;
		//selectionCam.enabled = false;
        selectionCam.SetActive(false);
        player.gameObject.SetActive(false);
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
