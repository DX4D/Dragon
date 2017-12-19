using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CurrentWeaponUI : MonoBehaviour {

    public Text WeaponName;
    //public Image WeaponIcon;

    public void UIWeaponName(GameObject weaponName)
    {
        if (WeaponName !=null)
        {
            WeaponName.text = weaponName.name.Replace("(Clone)","");
        }
    }

}
