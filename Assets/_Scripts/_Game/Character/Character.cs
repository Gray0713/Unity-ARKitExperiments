using System;
using UnityEngine;

[Serializable]
public class Character : MonoBehaviour
{
    public enum CharacterType
    {
        Mavic,
        Phantom,
        Spark
    }
    public CharacterType type;
    public new string name;
    public string description;
    public GameObject body;
    public GameObject[] propellers;
    public Material[] bodyMaterials;
    public Material[] propellerMaterials;
    public Material currentBodyMaterial;
    public Material currentPropellerMaterial;

    private void Start()
    {
        ChangeBodyMaterial(bodyMaterials[0]);
        ChangePropellerMaterial(propellerMaterials[0]);
    }
    public void ChangeBodyMaterial(Material material)
    {
        currentBodyMaterial = material;
        body.GetComponent<Renderer>().material = currentBodyMaterial;
    }
    public void ChangePropellerMaterial(Material material)
    {
        currentPropellerMaterial = material;
		for (int i = 0; i < propellers.Length; i++)
		{
			propellers[i].GetComponent<Renderer>().material = currentPropellerMaterial;
		}
    }
}
