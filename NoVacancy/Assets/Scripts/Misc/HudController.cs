using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public GameObject weaponIcon;
    public GameObject ammoText;

    public void setWeaponIcon(Sprite icon)
    {
        weaponIcon.GetComponent<Image>().sprite = icon;
    }

    public void updateAmmoText(int clipRounds, int totalRounds)
    {
        ammoText.GetComponent<Text>().text = clipRounds + " / " + totalRounds;
    }
}
