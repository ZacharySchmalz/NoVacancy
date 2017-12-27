using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
 *  This script controlls player behavior.
 *  It keeps track of resource amounts as well as collisions with resources and other items
 */

public class PlayerController : MonoBehaviour
{
    public float speed;
    public GameObject canvas;
    public GameObject[] weaponArray;    // array that hold all weapons
    [HideInInspector] public Inventory inventory;

    private int[] resourceArray;        // array that holds amounts for all resources
    private int[] lastClipArray;        // array that stores all weapons RoundsInClip to be loaded upon creation of weapon
    private GameObject[] resourceList;  // array that holds resource text components
    private GameObject currentWeapon;
    private Weapon currentWeaponScript;

    private bool isSwapping;
    private bool isShooting;

    public GameObject getWeapon
    {
        get { return currentWeapon; }
    }

    public int LoadLastClip
    {
        get{ return lastClipArray[getWeaponLocation(currentWeaponScript)]; }
    }

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        resourceArray = new int[6];
        resourceList = new GameObject[6];
        lastClipArray = new int[weaponArray.Length];

        // Grab all resources in the panel
        GameObject resourcePanel = GameObject.FindGameObjectWithTag("ResourcePanel");
        int index = 0;
        foreach(Transform child in resourcePanel.transform)
        {
            resourceList[index] = child.gameObject;
            updateCount(index);
            index++;
        }
    }

    // Check for weapon input keys here
    public void Update()
    {
        bool allowInput;

        if (currentWeaponScript && isSwapping == false && currentWeaponScript.Reloading == false && canvas.GetComponent<UICanvasHandler>().isInputAllowed())
            allowInput = true;
        else if (!currentWeaponScript)
            allowInput = true;
        else
            allowInput = false;

        // If input is allowed
        if (allowInput)
        {
            // Swap Weapons
            float scrollDir = Input.GetAxis("Mouse ScrollWheel");
            if (scrollDir != 0)
            {
                beginSwap(scrollDir);
            }

            // Reload
            if(Input.GetKeyDown(KeyCode.R))
            {
                beginReload();
            }

            // Shoot (LMB)
            if(Input.GetMouseButton(0))
            {
                if(!isShooting)
                    beginShoot();
            }

            if(Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("Main");
            }

            if(Input.GetKeyDown(KeyCode.Backspace))
            {
                Application.Quit();
            }
        }
    }

    // Player Movement
    void FixedUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float translation = Input.GetAxis("Vertical") * speed;
            float straffe = Input.GetAxis("Horizontal") * speed;
            translation *= Time.deltaTime;
            straffe *= Time.deltaTime;
            transform.Translate(straffe, 0, translation);

            if (currentWeaponScript)
            {
                if (straffe != 0 || translation != 0)
                {
                    if (currentWeaponScript.isWalking != true)
                    {
                        currentWeaponScript.isWalking = true;
                        currentWeaponScript.walk(true);
                    }
                }
                else
                {
                    if (currentWeaponScript.isWalking != false)
                    {
                        currentWeaponScript.isWalking = false;
                        currentWeaponScript.walk(false);
                    }
                }
            }
        }
    }

    public void beginSwap(float direction)
    {
        StartCoroutine(swapWeapon(direction));
    }

    public void beginReload()
    {
        StartCoroutine(reloadWeapon());
    }

    public void beginShoot()
    {
        StartCoroutine(shootWeapon());
    }

    public IEnumerator swapWeapon(float direction)
    {
        isSwapping = true;
        // If player is holding a weapon
        if (currentWeapon)
        {
            currentWeaponScript = currentWeapon.GetComponentInChildren<Weapon>();

            // Play the swap out animation
            currentWeaponScript.swapDir = 1;
            yield return new WaitForSeconds(currentWeaponScript.swap());

            // Save current clip before destroying weapon
            lastClipArray[getWeaponLocation(currentWeaponScript)] = currentWeaponScript.RoundsInClip;

            // If swapping from weapon at beginning of array to weapon at end of array
            if(getWeaponLocation(currentWeaponScript) == 0 && direction > 0)
            {
                Destroy(currentWeapon);
                currentWeapon = Instantiate(weaponArray[weaponArray.Length - 1], GameObject.FindGameObjectWithTag("Gun").transform.position, new Quaternion(), GameObject.FindGameObjectWithTag("Gun").transform).gameObject;
            }
            // If swapping from weapon at end of array to weapon at beginning of array
            else if(getWeaponLocation(currentWeaponScript) == weaponArray.Length-1 && direction < 0)
            {
                Destroy(currentWeapon);
                currentWeapon = Instantiate(weaponArray[0], GameObject.FindGameObjectWithTag("Gun").transform.position, new Quaternion(), GameObject.FindGameObjectWithTag("Gun").transform).gameObject;
            }
            // If swapping to a weapon higher in the array
            else if(direction < 0)
            {
                int loc = getWeaponLocation(currentWeaponScript);
                Destroy(currentWeapon);
                currentWeapon = Instantiate(weaponArray[loc + 1], GameObject.FindGameObjectWithTag("Gun").transform.position, new Quaternion(), GameObject.FindGameObjectWithTag("Gun").transform).gameObject;
            }
            // If swapping to a weapon lower in the array
            else if (direction > 0)
            {
                int loc = getWeaponLocation(currentWeaponScript);
                Destroy(currentWeapon);
                currentWeapon = Instantiate(weaponArray[loc - 1], GameObject.FindGameObjectWithTag("Gun").transform.position, new Quaternion(), GameObject.FindGameObjectWithTag("Gun").transform).gameObject;
            }
        }
        // Player is not holding any weapon
        else
        {
            // Set weapon to first weapon
            if (direction > 0)
            {
                currentWeapon = Instantiate(weaponArray[0], GameObject.FindGameObjectWithTag("Gun").transform.position, new Quaternion(), GameObject.FindGameObjectWithTag("Gun").transform).gameObject;
            }
            // Set weapon to last weapon
            else
            {
                currentWeapon = Instantiate(weaponArray[weaponArray.Length - 1], GameObject.FindGameObjectWithTag("Gun").transform.position, new Quaternion(), GameObject.FindGameObjectWithTag("Gun").transform);
            }
        }

        // Swap in to the new weapon
        currentWeaponScript = currentWeapon.GetComponentInChildren<Weapon>();
        currentWeaponScript.Start();
        currentWeaponScript.swapDir = -1;
        yield return new WaitForSeconds(currentWeaponScript.swap());
        isSwapping = false;
    }

    public IEnumerator reloadWeapon()
    {
        currentWeaponScript.Reloading = true;
        // Only reload if player is holding a weapon
        if(currentWeapon)
        {
            float time = currentWeaponScript.reload();
            yield return new WaitForSeconds(time);

            // Weapon actually needs reloading
            if (time != 0)
            {
                var needs = currentWeaponScript.clipRounds - currentWeaponScript.RoundsInClip;
                // Move all rounds into the clip
                if(currentWeaponScript.TotalRounds <= needs)
                {
                    currentWeaponScript.updateAmmoCounts(currentWeaponScript.RoundsInClip + currentWeaponScript.TotalRounds, 0);
                }
                // Inventory has enough rounds to fill a clip
                else
                {
                    currentWeaponScript.updateAmmoCounts(currentWeaponScript.RoundsInClip + needs, currentWeaponScript.TotalRounds - needs);
                }
                currentWeaponScript.updateHud();
            }
        }
        currentWeaponScript.Reloading = false;
    }

    public IEnumerator shootWeapon()
    {
        isShooting = true;
        // Only shoot if theres rounds in the clip
        if(currentWeapon && currentWeaponScript.RoundsInClip > 0)
        {
            float time = currentWeaponScript.shoot();
            yield return new WaitForSeconds(time);

            // Subtract ammo in inventory
            List<GameObject> inventorySlots = inventory.getSlotList();
            int lastIndexOf = -1;
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (!inventorySlots[i].GetComponent<Slot>().isEmpty)
                {
                    // Slot has ammo type of current gun
                    if (inventorySlots[i].GetComponent<Slot>().Items.Peek().ammo_type == currentWeaponScript.getAmmoType && inventorySlots[i].GetComponent<Slot>().Items.Count == currentWeaponScript.clipRounds)
                    {
                        lastIndexOf = i;
                    }
                    else if (inventorySlots[i].GetComponent<Slot>().Items.Peek().ammo_type == currentWeaponScript.getAmmoType && inventorySlots[i].GetComponent<Slot>().Items.Count != currentWeaponScript.clipRounds)
                    {
                        inventorySlots[i].GetComponent<Slot>().useItem();
                        currentWeaponScript.updateAmmoCounts(currentWeaponScript.RoundsInClip - 1, currentWeaponScript.TotalRounds);
                        lastIndexOf = -1;
                        break;
                    }
                }
            }
            if(lastIndexOf != -1)
            {
                inventorySlots[lastIndexOf].GetComponent<Slot>().useItem();
                currentWeaponScript.updateAmmoCounts(currentWeaponScript.RoundsInClip - 1, currentWeaponScript.TotalRounds);
            }
            currentWeaponScript.updateHud();
        }
        isShooting = false;
    }

    // Return index of weapon
    public int getWeaponLocation(Weapon currentWeapon)
    {
        int loc = -1;
        if (currentWeapon)
        {
            for(int i = 0; i < weaponArray.Length; i++)
            {
                if ((int)currentWeapon.weaponType == i)
                {
                    loc = i;
                    break;
                }
            }
        }
        return loc;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            inventory.addItem(other.GetComponent<Item>());
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Collision with Resources
        if(collision.gameObject.tag == "Resource")
        {
            Resource item = collision.gameObject.GetComponent<Resource>();
            int loc = (int)item.getType();
            resourceArray[loc] += item.amount;
            updateCount(loc);
            Destroy(collision.gameObject);
        }
    }

    // Update the count in the text component
    public void updateCount(int location)
    {
        resourceList[location].GetComponentInChildren<Text>().text = " x  " + resourceArray[location];
    }

    public int getResourceAmount(int location)
    {
        return resourceArray[location];
    }

    public void subtractResourceAmount(int location, int amount)
    {
        resourceArray[location] -= amount;
        updateCount(location);
    }
}
