using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public int rows, slots;
    public float slotLeftPad, slotTopPad;
    public float slotScale;
    public Canvas canvas;
    public EventSystem eventSystem;
    public GameObject iconPrefab;
    public GameObject slotPrefab;

    private float invHeight, invWidth;
    private float slotSize;
    private static int emptySlots;
    private List<GameObject> allSlots;
    private RectTransform inventoryRect;
    private static GameObject hoverObject;
    private static Slot from, to;

    private Animator anim;
    private bool active;

    public static int EmptySlots
    {
        get { return emptySlots; }
        set { emptySlots = value; }
    }

    // Use this for initialization
    void Start ()
    {
        createLayout();
        anim = GetComponentInParent<Animator>();
        active = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!eventSystem.IsPointerOverGameObject(-1) && from != null)
            {
                from.GetComponent<Image>().color = Color.white;
                from.clearSlot();
                Destroy(GameObject.Find("Hover"));
                emptySlots++;
                to = null;
                from = null;
                hoverObject = null;
            }
        }

        if (hoverObject != null)
        {
            float xm = Input.mousePosition.x;
            float ym = Input.mousePosition.y;
            hoverObject.transform.position = new Vector2(xm - (hoverObject.GetComponent<Image>().rectTransform.rect.width/2), ym + (hoverObject.GetComponent<Image>().rectTransform.rect.height / 2));
        }
	}

    // Function handles all events associated with opening the inventory
    public void activate()
    {
        active = true;
        anim.SetBool("active", active);
    }

    public void deactivate()
    {
        active = false;
        anim.SetBool("active", active);
    }

    public void createLayout()
    {
        // Create Inventory
        emptySlots = slots;
        allSlots = new List<GameObject>();
        slotSize = (Screen.width / Screen.height) * slotScale;
        invWidth = (slots / rows) * (slotSize + slotLeftPad) + slotLeftPad;
        invHeight = rows * (slotSize + slotTopPad) + slotTopPad;
        inventoryRect = GetComponent<RectTransform>();
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, invWidth);
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, invHeight);
        inventoryRect.transform.position = new Vector2((Screen.width - (invWidth * canvas.scaleFactor)) / 2, (Screen.height + (invHeight * canvas.scaleFactor)) / 2);

        int columns = slots / rows;
        for(int y = 0; y < rows; y++)
        {
            for(int x = 0; x < columns; x++)
            {
                GameObject newSlot = (GameObject)Instantiate(slotPrefab);
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();
                newSlot.name = "Slot";
                newSlot.transform.SetParent(inventoryRect.transform.parent);
                slotRect.localPosition = inventoryRect.localPosition + new Vector3(((slotSize + 2) / 2) + (slotLeftPad * (x + 1) + (slotSize * x)), -((slotSize + 2) / 2) + (-slotTopPad * (y + 1) - (slotSize * y)));
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize);
                slotRect.localScale = new Vector3(1, 1, 1);
                allSlots.Add(newSlot);
            }
        }
        inventoryRect.transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    public bool addItem(Item item)
    {
        if(item.item_max_size == 1)
        {
            placeEmpty(item);
            return true;
        }

        else
        {
            foreach(GameObject slot in allSlots)
            {
                Slot temp = slot.GetComponent<Slot>();
                if(!temp.isEmpty)
                {
                    if(temp.CurrentItem.item_name == item.item_name && temp.isAvailable)
                    {
                        temp.addItem(item);
                        return true;
                    }
                }
            }
            if(emptySlots > 0)
            {
                placeEmpty(item);
                return true;
            }
        }
        return false;
    }

    private bool placeEmpty(Item item)
    {
        if(emptySlots > 0)
        {
            foreach(GameObject slot in allSlots)
            {
                Slot temp = slot.GetComponent<Slot>();
                if(temp.isEmpty)
                {
                    temp.addItem(item);
                    emptySlots--;
                    return true;
                }
            }
        }
        return false;
    }

    public void moveItem(GameObject clicked)
    {
        if(from == null)
        {
            if(!clicked.GetComponent<Slot>().isEmpty)
            {
                from = clicked.GetComponent<Slot>();
                from.GetComponent<Image>().color = Color.gray;

                hoverObject = (GameObject)Instantiate(iconPrefab);
                hoverObject.GetComponent<Image>().sprite = clicked.GetComponent<Image>().sprite;
                hoverObject.name = "Hover";

                RectTransform hoverTransfrom = hoverObject.GetComponent<RectTransform>();
                RectTransform clickedTransform = clicked.GetComponent<RectTransform>();

                hoverTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clickedTransform.sizeDelta.x);
                hoverTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clickedTransform.sizeDelta.y);
                hoverObject.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true);
                hoverObject.transform.localScale = from.gameObject.transform.localScale;
            }
        }
        else if(to == null)
        {
            to = clicked.GetComponent<Slot>();
            Destroy(GameObject.Find("Hover"));
        }

        if(to != null && from != null)
        {
            Stack<Item> tempTo = new Stack<Item>(to.Items);
            to.addItems(from.Items);

            if(tempTo.Count == 0)
            {
                from.clearSlot();
            }
            else
            {
                from.addItems(tempTo);
            }

            from.GetComponent<Image>().color = Color.white;
            to = null;
            from = null;
            hoverObject = null;
        }
    }

    public void swapParent(Transform newParent)
    {
        inventoryRect = GetComponent<RectTransform>();
        inventoryRect.transform.SetParent(newParent, false);

        for (int i = 0; i < slots; i++)
        {
            allSlots[i].transform.SetParent(newParent, false);
            allSlots[i].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }

    public List<GameObject> getSlotList()
    {
        return allSlots;
    }
}
