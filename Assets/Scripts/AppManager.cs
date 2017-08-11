using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : Singleton<AppManager> 
{
    [SerializeField] private GameObject UIPrefab;
    [SerializeField] private bool iAmFirst;

    private int lastSceneIndex;
    private int lastLevelIndex;
    // NOTIFIER
    private Notifier notifier;  

    void Awake() 
    {
        DontDestroyOnLoad(Instance);
        AppManager[] gameManagers = FindObjectsOfType(typeof(AppManager)) as AppManager[];
        if(gameManagers.Length > 1) 
        {
            for(int i = 0; i < gameManagers.Length; i++) 
            {
                if(!gameManagers[i].iAmFirst) 
                {
                    DestroyImmediate(gameManagers[i].gameObject);
                }
            }
        } else 
        {
            iAmFirst = true;
        }
    }

    void Start () 
    {
        // NOTIFIER
        notifier = new Notifier ();
    }

    void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

	public void ChangeScene(int scene)
	{
		Time.timeScale = 1;
		SceneManager.LoadScene(scene, LoadSceneMode.Single);
	}

    public void ChangeScene(string scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void RestartCurrentScene () 
    {
        Time.timeScale = 1;
        int scene = SceneManager.GetActiveScene ().buildIndex;
        SceneManager.LoadScene (scene, LoadSceneMode.Single);
        if (lastSceneIndex != 0){
            SceneManager.LoadScene(lastLevelIndex, LoadSceneMode.Additive);
        }
    }

    //public void LoadLevel (string level) 
    //{
    //    Time.timeScale = 1;
    //    SceneManager.LoadScene(baseLevel, LoadSceneMode.Single);
    //    SceneManager.LoadScene(level, LoadSceneMode.Additive);
    //}

    public void QuitApplication () 
    {
        Application.Quit ();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (mode) {
            case LoadSceneMode.Additive:
                Debug.Log("Level Loaded: " + scene.name);
                lastLevelIndex = scene.buildIndex;
                break;
            case LoadSceneMode.Single:
                Debug.Log("Current scene: " + scene.name);
                lastSceneIndex = scene.buildIndex;
                if (scene.buildIndex > 0)
                {
                    Instantiate(UIPrefab);
                }
                //Debug.Log("Last scene: " + lastSceneIndex);
                break;
            default:
                Debug.Log("Current scene: " + scene.name);
                break;
        }
    }

    // NOTIFIER
    void OnDestroy () 
    {
        if (notifier != null) 
        {
            notifier.UnsubcribeAll ();
        }
    }
}
