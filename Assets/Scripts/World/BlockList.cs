using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockList : MonoBehaviour
{
    public static List<Block> Blocks = new List<Block>();

    public void Awake()
    {
        //Dirt Block
        Block dirt = new Block("Dirt", false, 2, 15);
        dirt.SetColor(Color.white, true);
        Blocks.Add(dirt);

        //Grass Block
        Blocks.Add(new Block("Grass", false, 0, 15, 3, 15, 2, 15));

        //Stone Block
        Blocks.Add(new Block("Stone", false, 1, 15));

        //Grass Block
        Block grass = new Block("Grass_", false, 7, 13);
        grass.Collide = false;
        grass.SetColor(Color.green, false);
        Blocks.Add(grass);

        Block flower = new Block("Yellow Flower", false, 13, 15);
        flower.Collide = false;
        Blocks.Add(flower);

        Block rflower = new Block("Red Flower", false, 12, 15);
        rflower.Collide = false;
        Blocks.Add(rflower);

        //Sand Block
        Blocks.Add(new Block("Sand", false, 2, 14));

        //Log Block
        Blocks.Add(new Block("Log", false, 5, 14, 4, 14, 5, 14));

        //Cactus Block
        Blocks.Add(new Block("Cactus", false, 5, 11, 6, 11, 7, 11));

        //Iron Ore Block
        Blocks.Add(new Block("Iron Ore", false, 1, 13));

        //Leave Block
        Block leave = new Block("Leaves", false, 4, 12);
        leave.SetColor(Color.green, true);
        Blocks.Add(leave);
    }

    public static Block GetBlock(string Name)
    {
        foreach(Block b in Blocks)
        {
            if(b.BlockName == Name)
                return b;
        }
        return null;
    }
}
