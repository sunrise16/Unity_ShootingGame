using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAutoCollect : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private Transform itemParent;

    private int itemCount;

	private void Start()
    {
        itemParent = GameObject.Find("ITEM").transform.Find("Item_Temp");
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("AUTOCOLLECTZONE"))
        {
            itemCount = itemParent.childCount;
            for (int i = 0; i < itemCount; i++)
            {
                GameObject item = itemParent.GetChild(i).gameObject;
                ItemStatus itemStatus = item.GetComponent<ItemStatus>();
                itemStatus.SetPlayerFind(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name.Equals("AUTOCOLLECTZONE"))
        {
            itemCount = itemParent.childCount;
            for (int i = 0; i < itemCount; i++)
            {
                GameObject item = itemParent.GetChild(i).gameObject;
                ItemStatus itemStatus = item.GetComponent<ItemStatus>();
                itemStatus.SetPlayerFind(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Equals("AUTOCOLLECTZONE"))
        {
            itemCount = itemParent.childCount;
            for (int i = 0; i < itemCount; i++)
            {
                GameObject item = itemParent.GetChild(i).gameObject;
                ItemStatus itemStatus = item.GetComponent<ItemStatus>();
                itemStatus.SetPlayerFind(true);
            }
        }
    }
}
