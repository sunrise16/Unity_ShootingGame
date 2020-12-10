using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ItemStatus : MonoBehaviour
{
    private ItemSize itemSize;              // 아이템 사이즈
    private ItemType itemType;              // 아이템 종류

    private bool isPlayerFind;              // 아이템 자동 회수 체크

    #region GET, SET

    public ItemSize GetItemSize()
    {
        return itemSize;
    }
    
    public ItemType GetItemType()
    {
        return itemType;
    }

    public bool GetPlayerFind()
    {
        return isPlayerFind;
    }

    public void SetItemSize(ItemSize size)
    {
        itemSize = size;
    }

    public void SetItemType(ItemType type)
    {
        itemType = type;
    }

    public void SetPlayerFind(bool find)
    {
        isPlayerFind = find;
    }

    #endregion
}
