using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ItemAutoCollect : MonoBehaviour
{
    private Transform itemParent;

    private int itemCount;
    private bool isPlayerConnect;

    private void Start()
    {
        itemParent = GameObject.Find("ITEM").transform.Find("Item_Temp");

        itemCount = itemParent.childCount;
        isPlayerConnect = false;
    }

    private void Update()
    {
        itemCount = itemParent.childCount;

        for (int i = 0; i < itemCount; i++)
        {
            if (isPlayerConnect.Equals(true))
            {
                itemParent.GetChild(i).GetComponent<ItemStatus>().SetPlayerFind(true);
            }
            else
            {
                itemParent.GetChild(i).GetComponent<ItemStatus>().SetPlayerFind(false);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PLAYER"))
        {
            isPlayerConnect = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerConnect = false;
    }
}
