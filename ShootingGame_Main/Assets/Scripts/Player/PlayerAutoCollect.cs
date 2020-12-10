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
        playerStatus = GetComponent<PlayerStatus>();
        itemParent = GameObject.Find("ITEM").transform.Find("Item_Temp");
	}

    // 화면 상단 자동 회수존에 진입했을 경우 아이템 전부 자동 회수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("AUTOCOLLECTZONE") && playerStatus.GetSpriteOff().Equals(false))
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
        if (collision.name.Equals("AUTOCOLLECTZONE") && playerStatus.GetSpriteOff().Equals(false))
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
        if (collision.name.Equals("AUTOCOLLECTZONE") && playerStatus.GetSpriteOff().Equals(false))
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
