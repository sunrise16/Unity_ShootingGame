using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerGetItem : MonoBehaviour
{
    private Transform itemParent;
    
    private void Start()
    {
        itemParent = GameObject.Find("ITEM").transform.Find("Item");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ITEM"))
        {
            ItemStatus itemStatus = collision.GetComponent<ItemStatus>();

            if (gameObject.name.Equals("ItemCapsule"))
            {
                if (itemStatus.GetItemSize().Equals(ItemSize.ITEMSIZE_SMALL))
                {
                    switch (itemStatus.GetItemType())
                    {
                        case ItemType.ITEMTYPE_POWER:
                            if (GameData.currentPower >= 4.0f)
                            {
                                GameData.currentScore += 10;
                            }
                            else
                            {
                                GameData.currentPower += 0.01f;
                            }
                            break;
                        case ItemType.ITEMTYPE_SCORE:
                            GameData.currentScore += (10 * GameData.currentScoreItem);
                            break;
                        default:
                            break;
                    }
                }
                else if (itemStatus.GetItemSize().Equals(ItemSize.ITEMSIZE_MEDIUM))
                {
                    switch (itemStatus.GetItemType())
                    {
                        case ItemType.ITEMTYPE_POWER:
                            if (GameData.currentPower >= 4.0f)
                            {
                                GameData.currentScore += 50;
                            }
                            else
                            {
                                GameData.currentPower += 0.05f;
                                if (GameData.currentPower >= 4.0f)
                                {
                                    GameData.currentPower = 4.0f;
                                }
                            }
                            break;
                        case ItemType.ITEMTYPE_SCORE:
                            GameData.currentScore += (50 * GameData.currentScoreItem);
                            break;
                        case ItemType.ITEMTYPE_FULLPOWER:
                            if (GameData.currentPower < 4.0f)
                            {
                                // 풀 파워 메세지
                            }
                            GameData.currentPower = 4.0f;
                            break;
                        default:
                            break;
                    }
                }
                else if (itemStatus.GetItemSize().Equals(ItemSize.ITEMSIZE_LARGE))
                {
                    switch (itemStatus.GetItemType())
                    {
                        case ItemType.ITEMTYPE_POWER:
                            if (GameData.currentPower >= 4.0f)
                            {
                                GameData.currentScore += 150;
                            }
                            else
                            {
                                GameData.currentPower += 0.15f;
                                if (GameData.currentPower >= 4.0f)
                                {
                                    GameData.currentPower = 4.0f;
                                }
                            }
                            break;
                        case ItemType.ITEMTYPE_SCORE:
                            GameData.currentScore += (150 * GameData.currentScoreItem);
                            break;
                        case ItemType.ITEMTYPE_LIFE:
                            GameData.currentPlayerLife++;
                            // 알림 메세지
                            break;
                        case ItemType.ITEMTYPE_LIFEFRAGMENT:
                            GameData.currentPlayerLifeFragment++;
                            if (GameData.currentPlayerLifeFragment >= 8)
                            {
                                GameData.currentPlayerLife++;
                                GameData.currentPlayerLifeFragment = 0;
                                // 알림 메세지
                            }
                            break;
                        case ItemType.ITEMTYPE_SPELL:
                            GameData.currentPlayerBomb++;
                            // 알림 메세지
                            break;
                        case ItemType.ITEMTYPE_SPELLFRAGMENT:
                            GameData.currentPlayerBombFragment++;
                            if (GameData.currentPlayerBombFragment >= 8)
                            {
                                GameData.currentPlayerBomb++;
                                GameData.currentPlayerBombFragment = 0;
                                // 알림 메세지
                            }
                            break;
                        case ItemType.ITEMTYPE_FULLPOWER:
                            if (GameData.currentPower < 4.0f)
                            {
                                // 풀 파워 메세지
                            }
                            GameData.currentPower = 4.0f;
                            break;
                        default:
                            break;
                    }
                }
                ClearItem(collision.gameObject);
            }
            else if (gameObject.name.Equals("ItemCircle"))
            {
                itemStatus.SetPlayerFind(true);
            }
        }
    }

    private void ClearItem(GameObject obj)
    {
        obj.transform.SetParent(itemParent);
        obj.transform.position = new Vector2(0.0f, 0.0f);
        obj.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        obj.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);

        obj.SetActive(false);
    }
}
