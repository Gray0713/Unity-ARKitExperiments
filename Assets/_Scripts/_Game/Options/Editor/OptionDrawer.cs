using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Option))]
public class OptionDrawer : Editor
{
    
    // TODO: Correct function to display only one kind of Option (Slider, Toggle)

	//public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	//{
	//	EditorGUI.PrefixLabel(position, label);
 //       EditorGUI.PropertyField(position, property.FindPropertyRelative("mySlider"));
        
 //       Option myOption = (Option)target;
 //       myOption.myType = (Option.OptionType)EditorGUILayout.EnumPopup("Type", myOption.myType);
 //       switch(myOption.myType)
 //       {
 //           case Option.OptionType.Slider:
 //               //myOption.mySlider = (Option.Slider)EditorGUILayout.PropertyField()
	//			break;
 //           case Option.OptionType.Toggle:
 //               break;
 //       }
	//}
}
