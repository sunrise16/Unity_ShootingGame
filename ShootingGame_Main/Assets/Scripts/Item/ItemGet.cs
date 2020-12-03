using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ItemGet : MonoBehaviour
{
    private GameObject player;
    private ItemStatus itemStatus;
    private PlayerStatus playerStatus;
    private Transform itemParent;

    private float scoreRatio;
    
    private void Start()
    {
        player = GameObject.Find("CHARACTER").transform.Find("Player").gameObject;
        itemStatus = GetComponent<ItemStatus>();
        playerStatus = player.GetComponent<PlayerStatus>();
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
                            GameData.currentScore += 10;
                            if (GameData.currentPower < 4.0f)
                            {
                                GameData.currentPower += 0.01f;
                            }
                            break;
                        case ItemType.ITEMTYPE_SCORE:
                            if (itemStatus.GetPlayerFind().Equals(true))
                            {
                                scoreRatio = 1.0f;
                            }
                            else
                            {
                                scoreRatio = (player.transform.position.y + 4.334f) / 8.668f;
                            }
                            switch (GameData.gameDifficulty)
                            {
                                case GameDifficulty.DIFFICULTY_EASY:
                                    GameData.currentScore += (int)Mathf.Round((5000 + (int)Mathf.Round(5000 * scoreRatio)) * 0.1f) * 10;
                                    break;
                                case GameDifficulty.DIFFICULTY_NORMAL:
                                    GameData.currentScore += (int)Mathf.Round((10000 + (int)Mathf.Round(10000 * scoreRatio)) * 0.1f) * 10;
                                    break;
                                case GameDifficulty.DIFFICULTY_HARD:
                                    GameData.currentScore += (int)Mathf.Round((15000 + (int)Mathf.Round(15000 * scoreRatio)) * 0.1f) * 10;
                                    break;
                                case GameDifficulty.DIFFICULTY_LUNATIC:
                                    GameData.currentScore += (int)Mathf.Round((20000 + (int)Mathf.Round(20000 * scoreRatio)) * 0.1f) * 10;
                                    break;
                                case GameDifficulty.DIFFICULTY_EXTRA:
                                    GameData.currentScore += (int)Mathf.Round((30000 + (int)Mathf.Round(30000 * scoreRatio)) * 0.1f) * 10;
                                    break;
                                default:
                                    break;
                            }
                            GameData.currentScoreItem++;
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
                            GameData.currentScore += 50;
                            if (GameData.currentPower < 4.0f)
                            {
                                GameData.currentPower += 0.05f;
                                if (GameData.currentPower >= 4.0f)
                                {
                                    GameData.currentPower = 4.0f;
                                }
                            }
                            break;
                        case ItemType.ITEMTYPE_SCORE:
                            if (itemStatus.GetPlayerFind().Equals(true))
                            {
                                scoreRatio = 1.0f;
                            }
                            else
                            {
                                scoreRatio = (player.transform.position.y + 4.334f) / 8.668f;
                            }
                            switch (GameData.gameDifficulty)
                            {
                                case GameDifficulty.DIFFICULTY_EASY:
                                    GameData.currentScore += (int)Mathf.Round((25000 + (int)Mathf.Round(25000 * scoreRatio)) * 0.1f) * 10;
                                    break;
                                case GameDifficulty.DIFFICULTY_NORMAL:
                                    GameData.currentScore += (int)Mathf.Round((50000 + (int)Mathf.Round(50000 * scoreRatio)) * 0.1f) * 10;
                                    break;
                                case GameDifficulty.DIFFICULTY_HARD:
                                    GameData.currentScore += (int)Mathf.Round((75000 + (int)Mathf.Round(75000 * scoreRatio)) * 0.1f) * 10;
                                    break;
                                case GameDifficulty.DIFFICULTY_LUNATIC:
                                    GameData.currentScore += (int)Mathf.Round((100000 + (int)Mathf.Round(100000 * scoreRatio)) * 0.1f) * 10;
                                    break;
                                case GameDifficulty.DIFFICULTY_EXTRA:
                                    GameData.currentScore += (int)Mathf.Round((150000 + (int)Mathf.Round(150000 * scoreRatio)) * 0.1f) * 10;
                                    break;
                                default:
                                    break;
                            }
                            GameData.currentScoreItem += 5;
                            break;
                        case ItemType.ITEMTYPE_FULLPOWER:
                            GameData.currentScore += 10000;
                            if (GameData.currentPower < 4.0f)
                            {
                                GameData.currentPower = 4.0f;
                                // 풀 파워 메세지
                            }
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
                            GameData.currentScore += 150;
                            if (GameData.currentPower < 4.0f)
                            {
                                GameData.currentPower += 0.15f;
                                if (GameData.currentPower >= 4.0f)
                                {
                                    GameData.currentPower = 4.0f;
                                }
                            }
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
                            GameData.currentPlayerSpell++;
                            // 알림 메세지
                            break;
                        case ItemType.ITEMTYPE_SPELLFRAGMENT:
                            GameData.currentPlayerSpellFragment++;
                            if (GameData.currentPlayerSpellFragment >= 8)
                            {
                                GameData.currentPlayerSpell++;
                                GameData.currentPlayerSpellFragment = 0;
                                // 알림 메세지
                            }
                            break;
                        case ItemType.ITEMTYPE_FULLPOWER:
                            GameData.currentScore += 100000;
                            if (GameData.currentPower < 4.0f)
                            {
                                GameData.currentPower = 4.0f;
                                // 풀 파워 메세지
                            }
                            break;
                        default:
                            break;
                    }
                }
                ClearItem(collision.gameObject);
            }
            else if (gameObject.name.Equals("ItemCircle"))
            {
                if (playerStatus.GetSlowMove().Equals(true))
                {
                    itemStatus.SetPlayerFind(true);
                }
            }
            else if (gameObject.name.Equals("ITEMDESTROYZONE"))
            {
                ClearItem(collision.gameObject);
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
