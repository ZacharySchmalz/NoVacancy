using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/*
 *  This script holds information for each crafting recipie possible.
 *  It also controls input from the mouse and its corresponding functions 
 */

public class ItemRecipie : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject item;                                             // GameObject/Prefab to craft, contains item script
    public ItemType item_type;                                          // type of item to craft
    public string item_name;                                            // name of item to craft
    public int item_yield_amount;                                       // yield amount
    [HideInInspector] public string item_ingredient1_name;              // names of the resources
    [HideInInspector] public string item_ingredient2_name;              // used in the UpdateDetails script
    [HideInInspector] public string item_ingredient3_name;
    public ResourceType item_ingredient1;                               // the ingredient list
    public ResourceType item_ingredient2;                               // each item will require no more than 3 ingredients
    public ResourceType item_ingredient3;
    public int item_ingredient1_count;                                  // how many of each resource is required
    public int item_ingredient2_count;
    public int item_ingredient3_count;
    public Sprite item_ingredient1_sprite;                              // link the sprites of the required materials
    public Sprite item_ingredient2_sprite;                              // used in the UpdateDetails script
    public Sprite item_ingredient3_sprite;

    private UpdateDetails details;                                      // the panel displaying the details
    private Crafting crafter;                                           // crafting script
    private bool once;

	void Start ()
    {
        once = true;
        item_ingredient1_name = setName((int)item_ingredient1);
        item_ingredient2_name = setName((int)item_ingredient2);
        item_ingredient3_name = setName((int)item_ingredient3);
        updateText();
        details = GameObject.FindGameObjectWithTag("DetailsPanel").GetComponent<UpdateDetails>();
        crafter = GameObject.FindGameObjectWithTag("CraftingPanel").GetComponent<Crafting>();
    }

    // Sets the strings of the ingredient types names; used for display purposes in UpdateDetails script
    private string setName(int type)
    {
        string s = "";
        switch (type)
        {
            case 0:
            {
                s = "Scrap Metal";
                break;
            }
            case 1:
            {
                s = "Bandages";
                break;
            }
            case 2:
            {
                s = "Gunpowder";
                break;
            }
            case 3:
            {
                s = "Glue";
                break;
            }
        }
        return s;
    }

    // Updates text info in the UI element
    public void updateText()
    {
        int count = 0;
        foreach(Transform child in transform)
        {
            // Update name text
            if (count == 0)
            {
                child.gameObject.GetComponent<Text>().text = item_name;
                count++;
            }

            // Update yield text
            else
            {
                child.gameObject.GetComponent<Text>().text = "x" + item_yield_amount;
            }
        }
    }

    // When mouse is over the UI object, display the crafting details once
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (once)
        {
            details.updateInfo(this.gameObject);
            once = false;
        }
    }

    // When mouse leaves the UI element, reset drafting details to default
    public void OnPointerExit(PointerEventData eventData)
    {
        details.setDefaultInfo();
        once = true;
    }

    // Clicking on the UI element crafts the item, if possible, and updates info
    public void OnPointerClick(PointerEventData eventData)
    {
        bool result = crafter.craft(this, item);

        if (result)
        {
            details.updateInfo(this.gameObject);
        }
    }
}
