using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    private Stack<Item> items;

    public int yScale =1;
    public int xScale = 1;
    public Text stackText;
    public Sprite slotEmpty, slotHighlight;

    public bool isEmpty
    {
        get { return items.Count == 0; }
    }

    public bool isAvailable
    {
        get { return CurrentItem.item_max_size > items.Count; }
    }

    public Item CurrentItem
    {
        get { return items.Peek(); }
    }

    public Stack<Item> Items
    {
        get { return items; }
        set { items = value; }
    }

    public int stackAmount
    {
        get { return int.Parse(stackText.text); }
        set { stackText.text = value.ToString();  }
    }

    // Use this for initialization
    void Start ()
    {
        items = new Stack<Item>();
        RectTransform slotRect = GetComponent<RectTransform>();
        RectTransform textRect = stackText.GetComponent<RectTransform>();

        int textScaleFactor = (int)(slotRect.sizeDelta.x *.6);
        stackText.resizeTextMaxSize = textScaleFactor;
        stackText.resizeTextMinSize = textScaleFactor;

        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotRect.sizeDelta.y);
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void addItem(Item item)
    {
        items.Push(item);
        if(items.Count > 1)
        {
            stackText.text = items.Count.ToString();
        }

        changeSprite(item.item_sprite_neutral, item.item_sprite_highlighted);
    }

    public void useItem()
    {
        if(!isEmpty)
        {
            items.Pop();

            stackText.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

            if(isEmpty)
            {
                changeSprite(slotEmpty, slotHighlight);
                Inventory.EmptySlots++;
            }
        }
    }

    public void addItems(Stack<Item> items)
    {
        this.items = new Stack<Item>(items);
        stackText.text = items.Count > 1 ? items.Count.ToString() : string.Empty;
        changeSprite(CurrentItem.item_sprite_neutral, CurrentItem.item_sprite_highlighted);
    }

    private void changeSprite(Sprite neutral, Sprite highlight)
    {
        GetComponent<Image>().sprite = neutral;

        SpriteState st = new SpriteState();
        st.highlightedSprite = highlight;
        st.pressedSprite = neutral;
        GetComponent<Button>().spriteState = st;
    }

    private void dropItem()
    {
        if(!isEmpty)
        {
            items.Pop();
            stackText.text = items.Count > 1 ? items.Count.ToString() : string.Empty;
            if(isEmpty)
            {
                changeSprite(slotEmpty, slotHighlight);
                Inventory.EmptySlots++;
            }
        }
    }

    public void clearSlot()
    {
        items.Clear();
        changeSprite(slotEmpty, slotHighlight);
        stackText.text = string.Empty;
    }
}
