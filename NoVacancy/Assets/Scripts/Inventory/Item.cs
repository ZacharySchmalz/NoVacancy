using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    RESOURCE,
    AMMO,
    WEAPON
}

public enum ResourceType
{
    SCRAP = 0,
    BANDAGES,
    GUNPOWDER,
    GLUE
}

public enum AmmoType
{
    PISTOL_AMMO = 0,
    SMG_AMMO,
    RIFLE_AMMO,
    RAIL_AMMO
}

public class Item : MonoBehaviour
{
    public string item_name;
    public int item_max_size;
    public Sprite item_sprite_neutral;
    public Sprite item_sprite_highlighted;
    protected ItemType item_type;
    public AmmoType ammo_type;

    public ItemType itemType
    {
        get { return item_type; }
    }

}
