using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  This script controls all the functions of weapons and ammo
 *  It also controls the animations for the weapons
 */

// WeaponType needs to be in the same order as in the weapon array in PlayerController
public enum WeaponType
{
    PISTOL = 0,
    SMG,
    RIFLE,
    RAIL
}

public class Weapon : MonoBehaviour
{
    public WeaponType weaponType;      // type of weapon
    protected AmmoType ammoType;
    protected string weaponName;    // name of weapon
    public int damage;              // how much damage does the weapon do
    public int clipRounds;          // how many rounds in each ammo clip
    protected int roundsInClip;     // how many rounds are in the current clip
    protected int totalRounds;      // how many rounds player is carrying, ROUNDS IN CLIP ARE NOT COUNTED
    protected bool isReloading;     // is the player currently reloading
    public float fireRate;          // how many rounds per second
    public Sprite weaponIcon;       // icon to be displayed to the HUD
    public Texture2D crosshair;
    public GameObject bullet;

    protected Inventory inventory;
    protected HudController hud;
    protected Animator anim;
    protected AudioSource audioSource;
    [HideInInspector] public int swapDir;             // 1 = swap out, -1 = swap in
    [HideInInspector] public bool isWalking;

    // Seriously Unity??? You're making me hard code animation lengths???
    public float reloadTime;
    public float swapTime;
    public float shootTime;


    //-----------------------------------------------WEAPON PROPERTIES-------------------------------------//

    public AmmoType getAmmoType
    {
        get { return ammoType; }
    }

    public bool Reloading
    {
        get { return isReloading; }
        set { isReloading = value; }
    }

    public string WeaponName
    {
        get { return weaponName; }
    }

    public int RoundsInClip
    {
        get { return roundsInClip; }
    }

    public int TotalRounds
    {
        get { return totalRounds; }
    }

    //-----------------------------------------------WEAPON FUNCTIONS-------------------------------------//
    public virtual void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HudController>();
        audioSource = GetComponent<AudioSource>();
        roundsInClip = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().LoadLastClip;        // load last clip into new clip
        isReloading = false;
        
    }

    private void OnGUI()
    {
        float xMin = (Screen.width / 2) - (crosshair.width / 2);
        float yMin = (Screen.height / 2) - (crosshair.height / 2);
        GUI.DrawTexture(new Rect(xMin, yMin, crosshair.width, crosshair.height), crosshair);
    }

    public void Update()
    {

    }

    public virtual float reload()
    {
        // Only reload if player has less than current clip rounds and has rounds in the inventory
        if (roundsInClip < clipRounds && totalRounds > 0)
        {
            anim.SetTrigger("reload");
            anim.Update(.1f);
            return (reloadTime / anim.GetCurrentAnimatorStateInfo(0).speed);
        }
        else
            return 0f;
    }

    // Function swaps weapon; specifically, plays the swap animation 
    public virtual float swap()
    {
        // Swap Out
        if (swapDir > 0)
            anim.SetTrigger("swapOut");
        // Swap In
        else if (swapDir < 0)
            anim.SetTrigger("swapIn");
        anim.Update(.1f);
        return swapTime / anim.GetCurrentAnimatorStateInfo(0).speed;
    }

    public virtual void walk(bool walking)
    {
        anim.SetBool("walking", walking);
    }

    public virtual float shoot()
    {
        anim.SetTrigger("shoot");
        anim.Update(.1f);
        audioSource.Play();
        // Instantiate bullet, set the bullet damage to weapon damage
        GameObject newBullet = Instantiate(bullet, GameObject.FindGameObjectWithTag("Emitter").transform.position, transform.rotation);
        newBullet.GetComponent<Bullet>().damage = damage;

        return fireRate;
    }

    // Returns the amount of rounds of a given ammo type in the inventory
    public int roundsInInventory(AmmoType type)
    {
        int roundCount = 0;
        List<GameObject> inventorySlots = inventory.getSlotList();
        foreach(GameObject slot in inventorySlots)
        {
            if (!slot.GetComponent<Slot>().isEmpty)
            {
                Stack<Item> items = slot.GetComponent<Slot>().Items;
                if (items.Peek().ammo_type == type)
                {
                    roundCount += slot.GetComponent<Slot>().stackAmount;
                }
            }
        }
        return roundCount;
    }

    public void updateAmmoCounts(int newRoundsInClip, int newTotalRounds)
    {
        roundsInClip = newRoundsInClip;
        totalRounds = newTotalRounds;
    }

    public void updateHud()
    {
        hud.setWeaponIcon(weaponIcon);
        hud.updateAmmoText(roundsInClip, totalRounds);
    }

}
