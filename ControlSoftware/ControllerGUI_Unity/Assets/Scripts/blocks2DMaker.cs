using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blocks2DMaker : MonoBehaviour
{
    private static GameObject blockPrefab;
    private static GameObject blockContainer;
    private static int blockCount = 0;
    private static List<GameObject> blocks;

    private static GameObject GetBlockPrefab()
    {
        if (blockPrefab == null)
            blockPrefab = Resources.Load("block") as GameObject;
        return blockPrefab;
    }

    private Color blockColor(int h)
    {
        return new Color(255 - h * 21, 255 - h * 21, 255 - h * 5);
    }

    public static GameObject MakeBlock(Vector2 position, Color color)
    {
        blockCount++;
        if(blockContainer == null)
        {
            blockContainer = new GameObject("block container");
            blocks = new List<GameObject>();
        }

        GameObject block = Instantiate(GetBlockPrefab()) as GameObject;
        blocks.Add(block);
        block.transform.position = position;
        block.transform.parent = blockContainer.transform;
        block.name = "block" + blockCount;

        block.GetComponent<Renderer>().material.color = color;

        return block;
    }
}