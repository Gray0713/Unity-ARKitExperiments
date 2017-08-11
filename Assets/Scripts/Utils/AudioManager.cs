using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager> 
{
	private Dictionary<string, AudioSource> loops = new Dictionary<string, AudioSource>();
    private Dictionary<string, AudioSource> sounds = new Dictionary<string, AudioSource>();

    public bool iAmFirst;

    void Awake()
    {
        DontDestroyOnLoad(Instance);

        AudioManager[] audioManagers = FindObjectsOfType(typeof(AudioManager)) as AudioManager[];

        if(audioManagers.Length > 1)
        {
            for(int i = 0; i < audioManagers.Length; i++)
            {
                if(!audioManagers[i].iAmFirst)
                {
                    DestroyImmediate(audioManagers[i].gameObject);
                }
            }
        }
        else
        {
            iAmFirst = true;
        }
    }

	#region One Shoot

	public void PlayOneShoot2D(AudioClip source)
	{
		PlayOneShoot2D(source, 1.0f, 0.0f, true);
	}

	public void PlayOneShoot2D(AudioClip source, float volume)
	{
		PlayOneShoot2D(source, volume, 0.0f, true);
	}

	public void PlayOneShoot2D(AudioClip source, float volume, float delay, bool destroyOnLoad)
	{
		if(Camera.main != null)
		{
			PlayOneShoot(source, volume, Camera.main.transform.position, delay, destroyOnLoad);
		}
		else
		{
			PlayOneShoot(source, volume, Vector3.zero, delay, destroyOnLoad);
		}
	}

	public void PlayOneShoot(AudioClip source)
	{
		PlayOneShoot(source, 1.0f, Vector3.zero, 0.0f, true);
	}

	public void PlayOneShoot(AudioClip source, Vector3 position)
	{
		PlayOneShoot(source, 1.0f, position, 0.0f, true);
	}

	public void PlayOneShoot(AudioClip source, float volume)
	{
		PlayOneShoot(source, volume, Vector3.zero, 0.0f, true);
	}

	public void PlayOneShoot(AudioClip source, float volume, Vector3 position)
	{
		PlayOneShoot(source, volume, position, 0.0f, true);
	}

	public void PlayOneShoot(AudioClip source, float volume, Vector3 position, float delay, bool destroyOnLoad)
	{
		if(source == null)
		{
			//Debug.LogError("[Audio Manager] source is null ");
			return;
		}

		GameObject go = new GameObject("Audio OneShoot - " + source.name);
		go.transform.position = position;
		AudioSource audio = go.AddComponent<AudioSource>();
		audio.clip = source;
		audio.volume = volume;
		audio.PlayDelayed(delay);
		Destroy(go, source.length + 2f);

        if (!destroyOnLoad)
        {
            DontDestroyOnLoad(go);
        }
    }

    public void PlayStopableOneShot(string name, AudioClip source, float volume, float delay, bool destroyOnLoad)
    {
        if (Instance.sounds.ContainsKey(name))
        {
            Debug.LogError("[Audio Manager] Sound name already in use '" + name + "'");
            return;
        }

        if (source == null)
        {
            Debug.LogError("[Audio Manager] source is null ");
            return;
        }

        GameObject go = new GameObject("Audio Stopable FX - " + source.name);
        AudioSource audio = go.AddComponent<AudioSource>();
        audio.clip = source;
        audio.volume = volume;
        audio.loop = false;
        audio.PlayDelayed(delay);

        StopSoundAfterDelay(name, audio.clip.length);

        if (!destroyOnLoad)
        {
            DontDestroyOnLoad(go);
        }

        Instance.sounds.Add(name, audio);
    }

	#endregion

	#region Loop

	public void PlayLoop2D(string name, AudioClip source)
	{
		PlayLoop2D(name, source, 1.0f, 1.0f, true);
	}

	public void PlayLoop2D(string name, AudioClip source, float volume)
	{
		PlayLoop2D(name, source, volume, 0.0f, true);
	}

	public void PlayLoop2D(string name, AudioClip source, float volume, float delay, bool destroyOnLoad)
	{
		if(Camera.main != null)
		{
			PlayLoop(name, source, volume, Camera.main.transform.position, delay, destroyOnLoad);
		}
		else 
		{
			PlayLoop(name, source, volume, Vector3.zero, delay, destroyOnLoad);
		}
	}

	public void PlayLoop(string name, AudioClip source)
	{
		PlayLoop(name, source, 1.0f, Vector3.zero, 0.0f, true);
	}

	public void PlayLoop(string name, AudioClip source, Vector3 position)
	{
		PlayLoop(name, source, 1.0f, position, 0.0f, true);
	}

	public void PlayLoop(string name, AudioClip source, float volume)
	{
		PlayLoop(name, source, volume, Vector3.zero, 0.0f, true);
	}

	public void PlayLoop(string name, AudioClip source, float volume, Vector3 position, float delay, bool destroyOnLoad)
	{
		if(Instance.loops.ContainsKey(name))
		{
			Debug.LogError("[Audio Manager] Loop name already in use '" + name + "'");
			return;
		}

		if(source == null)
		{
			Debug.LogError("[Audio Manager] source is null ");
			return;
		}

		GameObject go = new GameObject("Audio Loop - " + source.name);
		go.transform.position = position;
		AudioSource audio = go.AddComponent<AudioSource>();
		audio.clip = source;
		audio.volume = volume;
		audio.loop = true;
		audio.PlayDelayed(delay);

        if(!destroyOnLoad)
        {
            DontDestroyOnLoad(go);
        }

        Instance.loops.Add(name, audio);
	}

	public void StopLoop(string name)
	{
		if(Instance.loops.ContainsKey(name))
		{
			if(Instance.loops[name] != null)
			{
                Instance.loops[name].Stop();
				Destroy(instance.loops[name].gameObject);
			}
            Instance.loops.Remove(name);
		}
	}

    public void StopSound(string name)
    {
        if (Instance.sounds.ContainsKey(name))
        {
            if (Instance.sounds[name] != null)
            {
                Instance.sounds[name].Stop();
                Destroy(instance.sounds[name].gameObject);
            }
            Instance.sounds.Remove(name);
        }
    }

    private IEnumerator StopSoundAfterDelay(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        StopSound(name);
    }

    #endregion

}
