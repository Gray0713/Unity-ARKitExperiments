using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Battery")]
    [Tooltip("Battery duration in seconds.")]
    [SerializeField] private float batteryDuration = 1200.0f;
    [SerializeField] private Text batteryPowerText;
    [SerializeField] private Image batteryPowerFillImage;
    [SerializeField] private Slider batteryPowerSlider;
    //[Range(0.0f, 1.0f)] [SerializeField] private float batteryPower;
    [Header("GPS")]
#if UNITY_EDITOR
    //[MinMaxSlider(12.0f, 18.0f)]
    [MinMaxRange(10.0f, 20.0f)]
#endif
    //[SerializeField] private Vector2 GPSSignalStrengthRange;
    [SerializeField] private MinMaxRange GPSSignalStrengthRange;
    //public Vector2 GPSSignalStrengthRange;
    [SerializeField] private Text GPSSignalStrengthText;
    [SerializeField] private float updateInterval = 1.0f;
    private bool restarted;
    // Battery
    private float batteryPower;
	private float startTime;
    private float elapsedTime;
    private float remainingTime;
    // GPS
    private float GPSSignalStrength;
    private float updateTime;
	// NOTIFIER
	private Notifier notifier;
	public const string ON_BATTERY_DEAD = "OnBatteryDead";
 
	// Use this for initialization
	void Start () 
    {
        // NOTIFIER
        notifier = new Notifier();
        notifier.Subscribe(ScreenController.ON_CLOSE_HELP, HandleOnRestart);
		if (batteryDuration <= 0.0f)
        {
            Debug.LogError("Battery duration cannot be equal or less than zero!");
        }
        if (!batteryPowerText || !batteryPowerFillImage || !batteryPowerSlider)
        {
            Debug.LogError("Please attach the missed Battery UI elements to the script");
        }
		if (!GPSSignalStrengthText)
		{
			Debug.LogError("Please attach the missed GPS UI element to the script");
		}
	}
    private void OnEnable()
    {
        if (restarted)
        {
            elapsedTime = 0.0f;
            restarted = false;
        }
    }
    // Update is called once per frame
    void Update () 
    {
        elapsedTime += Time.deltaTime;
		remainingTime = batteryDuration - elapsedTime;
        if (remainingTime <= 0.0f)
        {
            notifier.Notify(ON_BATTERY_DEAD);
        }
		// Battery
		SetBatteryDisplay(remainingTime);
		// GPS
		updateTime += Time.deltaTime;
		if (updateTime > updateInterval)
		{
			updateTime = 0.0f;
            GPSSignalStrength = GPSSignalStrengthRange.GetRandomValue();
			GPSSignalStrengthText.text = GPSSignalStrength.ToString("F1");
		}
	}
	private void HandleOnRestart(params object[] args)
	{
		restarted = true;
        SetBatteryDisplay();
	}
    private void SetBatteryDisplay()
    {
        SetBatteryDisplay(batteryDuration);
    }
    private void SetBatteryDisplay(float power)
    {
		batteryPower = Mathf.Clamp01(power / batteryDuration);
		batteryPowerSlider.value = batteryPower;
		batteryPowerText.text = Mathf.RoundToInt(batteryPower * 100.0f) + "%";
		batteryPowerFillImage.fillAmount = batteryPower;
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
