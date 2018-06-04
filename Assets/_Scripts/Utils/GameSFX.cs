using UnityEngine;
using System.Collections;

public class GameSFX : MonoBehaviour 
{
	public enum SFXType
	{
		Ambient,
		FX
	}

	public enum SFXMode
	{
		Single,
		RandomBox
	}

	[System.Serializable]
	public class SFXConfig
	{
		public SFXType 	   type;
		public SFXMode 	   mode;
		public AudioClip   clip;
		public AudioClip[] clips;
		public float       volume = 1f;
		public float       delay  = 0f;
		public float 	   restTime = 0.0f;
        public bool        destroyOnLoad = true;
        public bool        dontRepeatRandomBox = false;
		[HideInInspector]
		public float	   lastPlayTimestamp = 0.0f;
        public int         randomBoxIndex = -1;
	}

    [Header("Settings")]
	[Range(0.0f, 1.0f)]
	[SerializeField]
	private float globalVolume = 1.0f;

	[Range(0.0f, 1.0f)]
	[SerializeField]
	private float ambientVolume = 1.0f;

	[Range(0.0f, 1.0f)]
	[SerializeField]
	private float sfxVolume = 1.0f;

    
    private Notifier notifier;

    private void Awake()
	{
		notifier = new Notifier();
    }

	private void Start()
	{
    }

	private void Update()
	{
		if(AudioListener.volume != globalVolume)
		{
			AudioListener.volume = globalVolume;
		}
	}

    private void OnDestroy()
    {
        notifier.UnsubcribeAll();
    }

	private void PlayOneShoot(SFXConfig sfx)
	{
		if(Time.time - sfx.lastPlayTimestamp >= sfx.restTime)
		{
			if(sfx.mode == SFXMode.Single)
			{
				AudioManager.Instance.PlayOneShoot2D(sfx.clip, GetVolume(sfx), sfx.delay, sfx.destroyOnLoad);
			}
			else if(sfx.mode == SFXMode.RandomBox)
			{
                if(sfx.dontRepeatRandomBox)
                {
                    if(sfx.randomBoxIndex < 0)
                    {
                        sfx.randomBoxIndex = Random.Range(0, sfx.clips.Length);
                    }
                    else
                    {
                        sfx.randomBoxIndex = (sfx.randomBoxIndex + 1) >= sfx.clips.Length ? 0 : (sfx.randomBoxIndex + 1);
                    }
                }
                else
                {
                    sfx.randomBoxIndex = Random.Range(0, sfx.clips.Length);
                }

				AudioClip clip = sfx.clips[sfx.randomBoxIndex];
				AudioManager.Instance.PlayOneShoot2D(clip, GetVolume(sfx), sfx.delay, sfx.destroyOnLoad);
			}
			sfx.lastPlayTimestamp = Time.time;
		}
	}

    private void PlayStopableOneShoot(string name, SFXConfig sfx)
    {
        if (sfx.mode == SFXMode.Single)
        {
            AudioManager.Instance.PlayStopableOneShot(name, sfx.clip, GetVolume(sfx), sfx.delay, sfx.destroyOnLoad);
        }
        else if (sfx.mode == SFXMode.RandomBox)
        {
            AudioClip clip = sfx.clips[Random.Range(0, sfx.clips.Length)];
            AudioManager.Instance.PlayStopableOneShot(name, clip, GetVolume(sfx), sfx.delay, sfx.destroyOnLoad);
        }
    }

	private void PlayLoop(string name, SFXConfig sfx)
	{
		if(Time.time - sfx.lastPlayTimestamp >= sfx.restTime)
		{
			if(sfx.mode == SFXMode.Single)
			{
				AudioManager.Instance.PlayLoop2D(name, sfx.clip, GetVolume(sfx), sfx.delay, sfx.destroyOnLoad);
			}
			else if(sfx.mode == SFXMode.RandomBox)
			{
				AudioClip clip = sfx.clips[Random.Range(0, sfx.clips.Length)];
				AudioManager.Instance.PlayLoop2D(name, clip, GetVolume(sfx), sfx.delay, sfx.destroyOnLoad);
			}
			sfx.lastPlayTimestamp = Time.time;
		}
	}

	private float GetVolume(SFXConfig sfx)
	{
		return sfx.volume * (sfx.type == SFXType.FX ? sfxVolume : ambientVolume); 
	}

}
