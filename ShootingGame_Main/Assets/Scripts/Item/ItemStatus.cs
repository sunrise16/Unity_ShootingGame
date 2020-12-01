using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ItemStatus : MonoBehaviour
{
    private ItemSize itemSize;
    private ItemType itemType;

    private bool isPlayerFind;

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
