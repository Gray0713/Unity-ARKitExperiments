using System;

[Serializable]
public class Option
{
    public enum OptionType
    {
        Slider = 0,
        Toggle,
    }
    [Serializable]
    public struct Slider
    {
        public float value;
		public float minValue;
		public float maxValue;
		public float defValue;
    }
    [Serializable]
    public struct Toggle
    {
        public bool value;
        public bool defValue;
    }
    public string name;
    public OptionType myType;
    public Slider mySlider; 
    public Toggle myToggle;
}