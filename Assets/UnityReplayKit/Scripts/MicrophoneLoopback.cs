using UnityEngine;

namespace Rarebyte.REK
{

  [RequireComponent(typeof(AudioSource))]
  public class MicrophoneLoopback : MonoBehaviour
  {

    #region Inspector

    [Tooltip("Microphone volume")]
    [SerializeField]
    [Range(0, 1)]
    private float volume = 1;

    [Tooltip("Length of the recording buffer in seconds")]
    [SerializeField]
    private int lengthSec = 10;

    [Tooltip("Sample rate")]
    [SerializeField]
    private int frequency = 44100;

    #endregion

    private AudioSource audioSource;

    public string DeviceName { get; set; }

    public float Volume
    {
      get
      {
        return volume;
      }
      set
      {
        volume = value;
        audioSource.volume = volume;
      }
    }

    public void Awake()
    {
      audioSource = GetComponent<AudioSource>();
#if !REK_MICROPHONE_DISABLED
      if (Microphone.devices.Length > 0)
      {
        DeviceName = Microphone.devices[0];
      }
      Volume = volume;
#endif
    }

    public void StartMicrophone()
    {
#if !REK_MICROPHONE_DISABLED

      audioSource.clip = Microphone.Start(DeviceName, true, lengthSec, frequency);
      audioSource.loop = true;
      while ((Microphone.GetPosition(DeviceName) > 0) == false) { }
      audioSource.Play();
#endif
    }

    public void StopMicrophone()
    {
#if !REK_MICROPHONE_DISABLED
      if (Microphone.IsRecording(DeviceName))
      {
        Microphone.End(DeviceName);
        audioSource.Stop();
      }
#endif
    }
  }
}
