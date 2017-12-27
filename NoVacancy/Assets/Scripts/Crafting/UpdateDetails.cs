using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 *  This script is responsible for displaying a crafting recipie's resource requirement and the players currently held amounts.
 *  The information is displayed in the DetailsPanel within the CraftingPanel
 */

public class UpdateDetails : MonoBehaviour
{
    private List<GameObject> list;
    private PlayerController player;

	void Start ()
    {
        list = new List<GameObject>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // Grab the ingredient components
		foreach(Transform child in transform)
        {
            if(child.gameObject.GetComponent<Image>())
            {
                foreach(Transform grandchild in child.gameObject.transform)
                {
                        list.Add(grandchild.gameObject);
                }
            }
        }
        setDefaultInfo();
    }

    // Change details back to empty
    public void setDefaultInfo()
    {
        for (int i = 0; i < list.Count; i = i + 4)
        {
            list[i].GetComponent<Text>().text = "";
            list[i + 1].GetComponent<Image>().enabled = false;
            list[i + 2].GetComponent<Text>().text = "";
            list[i + 3].GetComponent<Text>().text = "";
        }
    }

    // Updates the text fields and images to the information provided by the recipie
    public void updateInfo(GameObject caller)
    {
        ItemRecipie details = caller.GetComponent<ItemRecipie>();
        for (int i = 0; i < list.Count; i = i + 4)
        {
            switch(i)
            {
                case 0:
                {
                    if (details.item_ingredient1_sprite)
                    {
                        list[i].GetComponent<Text>().text = details.item_ingredient1_name;
                        list[i + 1].GetComponent<Image>().enabled = true;
                        list[i + 1].GetComponent<Image>().sprite = details.item_ingredient1_sprite;
                        list[i + 2].GetComponent<Text>().text = "" + details.item_ingredient1_count;
                        list[i + 3].GetComponent<Text>().text = "" + player.getResourceAmount((int)details.item_ingredient1);
                    }
                    break;
                }

                case 4:
                {
                    if (details.item_ingredient2_sprite)
                    {
                        list[i].GetComponent<Text>().text = details.item_ingredient2_name;
                        list[i + 1].GetComponent<Image>().enabled = true;
                        list[i + 1].GetComponent<Image>().sprite = details.item_ingredient2_sprite;
                        list[i + 2].GetComponent<Text>().text = "" + details.item_ingredient2_count;
                        list[i + 3].GetComponent<Text>().text = "" + player.getResourceAmount((int)details.item_ingredient2);
                    }
                    break;
                }

                case 8:
                {
                    if (details.item_ingredient3_sprite)
                    {
                        list[i].GetComponent<Text>().text = details.item_ingredient3_name;
                        list[i + 1].GetComponent<Image>().enabled = true;
                        list[i + 1].GetComponent<Image>().sprite = details.item_ingredient3_sprite;
                        list[i + 2].GetComponent<Text>().text = "" + details.item_ingredient3_count;
                        list[i + 3].GetComponent<Text>().text = "" + player.getResourceAmount((int)details.item_ingredient3);
                    }
                    break;
                }
            }
        }
    }
}
