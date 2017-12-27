using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        weaponType = WeaponType.PISTOL;
        ammoType = AmmoType.PISTOL_AMMO;
        weaponName = "M6D Pistol";
        var allRounds = roundsInInventory(ammoType) - roundsInClip;
        var clipNum = allRounds / clipRounds;

        if (clipNum == 0)
            updateAmmoCounts(roundsInClip, allRounds % clipRounds);
        else
            updateAmmoCounts(roundsInClip, (clipRounds * (clipNum--)) + (allRounds % clipRounds));
        updateHud();
    }
}
