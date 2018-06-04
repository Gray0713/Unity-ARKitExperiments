using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioController : MonoBehaviour 
{
	private enum DeviceType
	{
		iPad,
		iPhone
	}
    [SerializeField] private Sprite ipadBackground;
    [SerializeField] private Sprite iphoneBackground;
    [SerializeField] private Image[] backgrounds;
    private DeviceType myDevice;
    private int numBackgrounds;

    void Start()
    {
        myDevice = DetectDevice();
        SetDeviceBackgrounds(myDevice);
    }
    private DeviceType DetectDevice()
    {
		if (Camera.main.aspect >= 1.7)
		{
			//Debug.Log("iPhone Detected");
			return DeviceType.iPhone;
		}
		else
		{
			//Debug.Log("iPad Detected");
			return DeviceType.iPad;
		}
    }
    private void SetDeviceBackgrounds(DeviceType device)
    {
		numBackgrounds = backgrounds.Length;
		switch(device)
        {
            case DeviceType.iPad:
                ChangeSprites(ipadBackground);
                break;
            case DeviceType.iPhone:
                ChangeSprites(iphoneBackground);
                break;
            default:
                break;
        }
    }
    private void ChangeSprites(Sprite sprite)
    {
        for (int i = 0; i < numBackgrounds; i++)
        {
            backgrounds[i].sprite = sprite;
        }
    }
}
