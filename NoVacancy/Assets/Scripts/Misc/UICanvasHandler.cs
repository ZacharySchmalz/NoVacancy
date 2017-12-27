using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasHandler : MonoBehaviour
{
    public GameObject craftBench;

    private bool invMode;
    private bool pauseMode;
    private bool craftMode;
    private GameObject inventory;
    private GameObject craftPanel;

	void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        invMode = false;
        pauseMode = false;
        craftMode = false;
        inventory = GameObject.FindGameObjectWithTag("Inventory");
        craftPanel = GameObject.FindGameObjectWithTag("CraftingPanel");
	}

    public bool isInputAllowed()
    {
        if (craftMode == false && pauseMode == false && invMode == false)
            return true;
        else
            return false;
    }
	
	void Update ()
    {
        // Key for inventory
        if (Input.GetKeyDown("tab") && craftMode == false && pauseMode == false)
        {
            invMode = !invMode;
            Cursor.lockState = CursorLockMode.None;
            // activated, activate inventory
            if(invMode)
            {
                inventory.GetComponent<Inventory>().swapParent(GameObject.FindGameObjectWithTag("InventoryPanel").GetComponent<Transform>());
                Cursor.lockState = CursorLockMode.Confined;
                inventory.GetComponent<Inventory>().activate();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                inventory.GetComponent<Inventory>().deactivate();
            }
        }

        else if(Input.GetKeyDown("escape"))
        {
            pauseMode = !pauseMode;
            Cursor.lockState = CursorLockMode.None;
            if (pauseMode)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
            }
        }

        // Key for crafting menu
        else if (Input.GetKeyDown("e") && invMode == false && pauseMode == false)
        {
            if (craftBench.GetComponent<CraftBench>().isReady() == true && craftMode == false)
            {
                inventory.GetComponent<Inventory>().swapParent(GameObject.Find("Inventory_Holder").GetComponent<Transform>());
                Cursor.lockState = CursorLockMode.None;
                craftMode = true;
                Cursor.lockState = CursorLockMode.Confined;
                craftPanel.GetComponent<Crafting>().activate();

            }
            else if(craftMode)
            {
                Cursor.lockState = CursorLockMode.None;
                craftMode = false;
                Cursor.lockState = CursorLockMode.Locked;
                craftPanel.GetComponent<Crafting>().deactivate();
            }
        }
    }
}
