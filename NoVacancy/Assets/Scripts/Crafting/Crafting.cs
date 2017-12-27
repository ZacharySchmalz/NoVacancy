using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  This script controls the animations for opening and closing the crafting interface.
 *  It also implements the crafting function for all craftable items 
 */

public class Crafting : MonoBehaviour
{
    private Animator anim;
    private bool active;
    private GameObject player;

	void Start ()
    {
        anim = GetComponent<Animator>();
        active = false;
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
    // Opens the interface
    public void activate()
    {
        active = true;
        anim.SetBool("active", active);
    }

    // Close the interface
    public void deactivate()
    {
        active = false;
        anim.SetBool("active", active);
    }

    // The crafting function
    public bool craft(ItemRecipie details, GameObject item)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        // Get player resource counts and subtract from requirement amount
        int ingredient1 = controller.getResourceAmount((int)details.item_ingredient1) - details.item_ingredient1_count;
        int ingredient2 = controller.getResourceAmount((int)details.item_ingredient2) - details.item_ingredient2_count;
        int ingredient3 = controller.getResourceAmount((int)details.item_ingredient3) - details.item_ingredient3_count;

        // If any one of the counts is negative, crafting is not possible
        if (ingredient1 >= 0 && ingredient2 >= 0 && ingredient3 >= 0)
        {
            // Add the item to the inventory according to the yield amount
            for(int i = 0; i < details.item_yield_amount; i++)
                controller.inventory.addItem(item.GetComponent<Item>());

            // Subtract the resources from the player counts
            controller.subtractResourceAmount((int)details.item_ingredient1, details.item_ingredient1_count);
            controller.subtractResourceAmount((int)details.item_ingredient2, details.item_ingredient2_count);
            controller.subtractResourceAmount((int)details.item_ingredient3, details.item_ingredient3_count);

            // If the item crafted is ammo and the same type as weapon held, update the HUD counter
            if (details.item_type == ItemType.AMMO && details.item.GetComponent<Item>().ammo_type == controller.getWeapon.GetComponentInChildren<Weapon>().getAmmoType)
            {
                Weapon weaponHeld = controller.getWeapon.GetComponentInChildren<Weapon>();
                HudController hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HudController>();
                weaponHeld.updateAmmoCounts(weaponHeld.RoundsInClip, weaponHeld.TotalRounds + details.item_yield_amount);
                hud.updateAmmoText(weaponHeld.RoundsInClip, weaponHeld.TotalRounds);
            }
            return true;
        }
        else
            return false;
    }
}
