using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG : Weapon
{
    public override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        weaponType = WeaponType.SMG;
        ammoType = AmmoType.SMG_AMMO;
        weaponName = "M7S SMG";
        var allRounds = roundsInInventory(ammoType) - roundsInClip;
        var clipNum = allRounds / clipRounds;

        if (clipNum == 0)
            updateAmmoCounts(roundsInClip, allRounds % clipRounds);
        else
            updateAmmoCounts(roundsInClip, (clipRounds * (clipNum--)) + (allRounds % clipRounds));
        updateHud();
    }
}
