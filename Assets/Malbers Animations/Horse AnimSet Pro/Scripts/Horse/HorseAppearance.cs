using UnityEngine;
using System.Collections;

public class HorseAppearance : MonoBehaviour {

  
    [Header("Horse Color")]

    public Material[] HorseSkins;
    public Transform[] HorseMesh;

    [Header("Mane Color")]
    public Material[] HairColor;
    public Transform[] HairMesh;

    [Header("Horse Mount")]
    public bool Mount;
    public Material[] MountColor;
    public Transform[] MountMesh;

    [Header("Horse Armor")]
    public bool Armor;
    public Material[] ArmorColor;
    public Transform[] ArmorMesh;
}
