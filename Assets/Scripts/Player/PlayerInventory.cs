using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static Block[,] InventoryItems;
    public static int[,] InventoryNum;

    public static Block[] HotbarItems;
    public static int[] HotbarNum;

    public Texture2D SlotBackground;
    public Texture2D Crosshair;

    public static int SelectedBlock;

    static int Width = 9, Height = 5;

    public static bool showInventory = false;

    public MouseLook[] mL;

    void Start()
    {
        Screen.showCursor = false;

        InventoryItems = new Block[Width, Height];
        InventoryNum = new int[Width, Height];

        HotbarItems = new Block[Width];
        HotbarNum = new int[Width];

        AddItem(Block.getBlock("Dirt"), 5);
        AddItem(Block.getBlock("Dirt"), 5);
        AddItem(Block.getBlock("Dirt"), 64);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            showInventory = !showInventory;

            foreach (MouseLook l in mL)
            {
                l.enabled = !showInventory;
            }

            if(showInventory)
            {
                Screen.showCursor = true;
            }
            else
            {
                Screen.showCursor = false;
            }
        }

        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (SelectedBlock + 1 >= Width)
                SelectedBlock = 0;
            else
                SelectedBlock++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (SelectedBlock - 1 < 0)
                SelectedBlock = Width - 1;
            else
                SelectedBlock--;
        }
    }
    void OnGUI()
    {
        float crosshairSize = 2.5f;
        GUI.DrawTexture(new Rect(
            Screen.width / 2 - ((Crosshair.width / crosshairSize) / 2),
            Screen.height / 2 - ((Crosshair.height / crosshairSize) / 2),
            Crosshair.width / crosshairSize,
            Crosshair.height / crosshairSize), Crosshair);

        Event e = Event.current;
        float space = 5;

        if (showInventory == true)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int InventoryWidth = Width * SlotBackground.width;
                    int InventoryHeight = Width * SlotBackground.height;

                    Rect offset = new Rect((Screen.width / 2) - (InventoryWidth / 2), (Screen.height / 2) - (InventoryHeight / 2), InventoryWidth, InventoryHeight);
                    Rect slotPos = new Rect(offset.x + SlotBackground.width * x, offset.y + SlotBackground.height * y, SlotBackground.width, SlotBackground.height);

                    GUI.DrawTexture(slotPos, SlotBackground);
                    Block b = InventoryItems[x, y];
                    int n = InventoryNum[x, y];

                    if (b != null)
                    {
                        Rect BlockPos = new Rect(slotPos.x + (space / 2), slotPos.y + (space / 2), slotPos.width - space, slotPos.height - space);
                        GUI.DrawTexture(BlockPos, b.ItemView);

                        GUI.Label(slotPos, n.ToString());

                        if (slotPos.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0 && draggingItem == null)
                        {
                            DragItem(x, y);
                            break;
                        }
                    }

                    if (slotPos.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0 && draggingItem != null)
                    {
                        MoveItem(x, y);
                        break;
                    }
                    if (slotPos.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 1)
                    {
                        SplitItem(x, y);
                        break;
                    }
                }
            }
        }

        ShowHotbarItems(e, space);
        ShowDraggingItem(e, space);
    }

    void ShowDraggingItem(Event e, float space)
    {
        if (showInventory == false) return;
        if(draggingItem != null)
        {
            GUI.DrawTexture(new Rect(e.mousePosition.x, e.mousePosition.y, SlotBackground.width - space, SlotBackground.height - space), draggingItem.ItemView);
            GUI.Label(new Rect(e.mousePosition.x + (SlotBackground.width - space) / 2, e.mousePosition.y + (SlotBackground.height - space)/2, SlotBackground.width - space, SlotBackground.height - space), draggingItemNum.ToString());
        }
    }
    void ShowHotbarItems(Event e, float space)
    {
        for (int x = 0; x < Width; x++)
        {
            int InventoryWidth = Width * SlotBackground.width;
            int InventoryHeight = SlotBackground.height;

            Rect offset = new Rect();

            if(showInventory)
                offset = new Rect((Screen.width / 2) - (InventoryWidth / 2), (Screen.height / 2) + (InventoryHeight + 20) - (InventoryHeight / 2), InventoryWidth, InventoryHeight);
            else
                offset = new Rect((Screen.width / 2) - (InventoryWidth / 2), (Screen.height) - (InventoryHeight + (20 / 2)) - (InventoryHeight / 2), InventoryWidth, InventoryHeight);

            Rect slotPos = new Rect(offset.x + SlotBackground.width * x, offset.y + SlotBackground.height * 0, SlotBackground.width, SlotBackground.height);

            if (SelectedBlock == x)
                GUI.color = Color.red;
            GUI.DrawTexture(slotPos, SlotBackground);
            GUI.color = Color.white;

            Block b = HotbarItems[x];
            int n = HotbarNum[x];

            if (b != null)
            {
                Rect BlockPos = new Rect(slotPos.x + (space / 2), slotPos.y + (space / 2), slotPos.width - space, slotPos.height - space);
                GUI.DrawTexture(BlockPos, b.ItemView);

                GUI.Label(slotPos, n.ToString());

                if (slotPos.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0 && draggingItem == null)
                {
                    DragHotbarItem(x);
                    break;
                }
            }

            if (slotPos.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0 && draggingItem != null)
            {
                MoveHotbarItem(x);
                break;
            }
            if (slotPos.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 1)
            {
                SplitHotbarItem(x);
                break;
            }
        }
    }

    Block draggingItem;
    int draggingItemNum;
    void DragItem(int x, int y)
    {
        if (draggingItem != null) return;

        draggingItem = InventoryItems[x, y];
        draggingItemNum = InventoryNum[x, y];

        InventoryItems[x, y] = null;
        InventoryNum[x, y] = 0;
    }
    void MoveItem(int x, int y)
    {
        if (draggingItem == null) return;

        Block bloc = InventoryItems[x, y];
		int blocN = InventoryNum[x, y];

		if (bloc == null)
        {
            InventoryItems[x, y] = draggingItem;
            InventoryNum[x, y] = draggingItemNum;

            draggingItem = null;
            draggingItemNum = 0;
        }
		else if (bloc == draggingItem)
        {
			if(blocN + draggingItemNum > bloc.BlockMaxStack)
            {
				int rest = InventoryNum[x, y] + draggingItemNum - bloc.BlockMaxStack;
				InventoryNum[x, y] = bloc.BlockMaxStack;

                draggingItemNum = rest;
            }
            else
            {
                InventoryNum[x, y] += draggingItemNum;

                draggingItem = null;
                draggingItemNum = 0;
            }
        }
    }
    void SplitItem(int x, int y)
    {
		Block bloc = InventoryItems[x, y];
        int blocN = InventoryNum[x, y];

		if(draggingItem != null && bloc == draggingItem)
        {
			if(InventoryNum[x, y] + 1 > bloc.BlockMaxStack)
            {

            }
            else
            {
                InventoryNum[x, y]++;
                draggingItemNum--;
            }
        }
        else if(draggingItem == null)
        {
            if (blocN / 2 <= 0)
                return;

			draggingItem = bloc;
            draggingItemNum = blocN / 2;

            InventoryNum[x, y] -= draggingItemNum;
        }
        else if(draggingItem != null)
        {
            InventoryItems[x, y] = draggingItem;
            InventoryNum[x, y]++;
            draggingItemNum--;
        }

        if (draggingItemNum <= 0)
        {
            draggingItem = null;
        }
    }

    void DragHotbarItem(int x)
    {
        if (draggingItem != null) return;

        draggingItem = HotbarItems[x];
        draggingItemNum = HotbarNum[x];

        HotbarItems[x] = null;
        HotbarNum[x] = 0;
    }
    void MoveHotbarItem(int x)
    {
        if (draggingItem == null) return;

        Block bloc = HotbarItems[x];
        int blocN = HotbarNum[x];

        if (bloc == null)
        {
            HotbarItems[x] = draggingItem;
            HotbarNum[x] = draggingItemNum;

            draggingItem = null;
            draggingItemNum = 0;
        }
        else if (bloc == draggingItem)
        {
            if (blocN + draggingItemNum > bloc.BlockMaxStack)
            {
                int rest = HotbarNum[x] + draggingItemNum - bloc.BlockMaxStack;
                HotbarNum[x] = bloc.BlockMaxStack;

                draggingItemNum = rest;
            }
            else
            {
                HotbarNum[x] += draggingItemNum;

                draggingItem = null;
                draggingItemNum = 0;
            }
        }
    }
    void SplitHotbarItem(int x)
    {
        Block bloc = HotbarItems[x];
        int blocN = HotbarNum[x];

        if (draggingItem != null && bloc == draggingItem)
        {
            if (HotbarNum[x] + 1 > bloc.BlockMaxStack)
            {

            }
            else
            {
                HotbarNum[x]++;
                draggingItemNum--;
            }
        }
        else if (draggingItem == null)
        {
            if (blocN / 2 <= 0)
                return;

            draggingItem = bloc;
            draggingItemNum = blocN / 2;

            HotbarNum[x] -= draggingItemNum;
        }
        else if (draggingItem != null)
        {
            HotbarItems[x] = draggingItem;
            HotbarNum[x]++;
            draggingItemNum--;
        }

        if (draggingItemNum <= 0)
        {
            draggingItem = null;
        }
    }

    public static void AddItem(Block b, int num)
    {
        for (int y = 0; y < Height + 1; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (y < Height)
                {
                    if (InventoryItems[x, y] == null)
                    {
                        InventoryItems[x, y] = b;
                        if (num > b.BlockMaxStack)
                            InventoryNum[x, y] = b.BlockMaxStack;
                        else
                            InventoryNum[x, y] = num;

                        return;
                    }
                    else if (InventoryItems[x, y] == b && InventoryNum[x, y] < b.BlockMaxStack)
                    {
                        if (num > b.BlockMaxStack)
                        {
                            int rest = num - b.BlockMaxStack;
                            InventoryItems[x, y] = b;
                            InventoryNum[x, y] = b.BlockMaxStack;

                            AddItem(b, rest);
                            return;
                        }
                        else
                        {
                            if (InventoryNum[x, y] + num > b.BlockMaxStack)
                            {
                                int rest = InventoryNum[x, y] + num - b.BlockMaxStack;

                                InventoryItems[x, y] = b;
                                InventoryNum[x, y] = b.BlockMaxStack;

                                AddItem(b, rest);
                            }
                            else
                            {
                                InventoryItems[x, y] = b;
                                InventoryNum[x, y] += num;
                            }
                            return;
                        }
                    }
                }
                else
                {
                    if (HotbarItems[x] == null)
                    {
                        HotbarItems[x] = b;
                        if (num > b.BlockMaxStack)
                            HotbarNum[x] = b.BlockMaxStack;
                        else
                            HotbarNum[x] = num;

                        return;
                    }
                    else if (HotbarItems[x] == b && HotbarNum[x] < b.BlockMaxStack)
                    {
                        if (num > b.BlockMaxStack)
                        {
                            int rest = num - b.BlockMaxStack;
                            HotbarItems[x] = b;
                            HotbarNum[x] = b.BlockMaxStack;

                            AddItem(b, rest);
                            return;
                        }
                        else
                        {
                            if (HotbarNum[x] + num > b.BlockMaxStack)
                            {
                                int rest = HotbarNum[x] + num - b.BlockMaxStack;

                                HotbarItems[x] = b;
                                HotbarNum[x] = b.BlockMaxStack;

                                AddItem(b, rest);
                            }
                            else
                            {
                                HotbarItems[x] = b;
                                HotbarNum[x] += num;
                            }
                            return;
                        }
                    }
                }
            }
        }
    }
    public static void RemoveItem(Block b, int num)
    {
        for (int y = 0; y < Height + 1; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (y < Height)
                {
                    if (InventoryItems[x, y] == b)
                    {
                        InventoryNum[x, y] -= num;

                        if (InventoryNum[x, y] == 0)
                            InventoryItems[x, y] = null;

                        break;
                    }
                }
                else
                {
                    if (HotbarItems[x] == b)
                    {
                        HotbarNum[x] -= num;

                        if (HotbarNum[x] == 0)
                            HotbarItems[x] = null;

                        break;
                    }
                }
            }
        }
    }
    public static Block GetSelectedBlock()
    {
        return HotbarItems[SelectedBlock];
    }
}