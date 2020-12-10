using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ItemGet : MonoBehaviour
{
    private GameObject player;
    private ItemStatus itemStatus;
    private PlayerStatus playerStatus;
    private Transform itemParent;

    private float scoreRatio;                   // 플레이어의 현재 y축 위치값에 따라 변하는 점수 비율
                                                // (화면 상단으로 갈수록 높은 점수를 받게 하고, 아이템 자동 회수 시 최대치로 자동 설정)
    
    private void Start()
    {
        player = GameObject.Find("CHARACTER").transform.Find("Player").gameObject;
        itemStatus = GetComponent<ItemStatus>();
        playerStatus = player.GetComponent<PlayerStatus>();
        itemParent = GameObject.Find("ITEM").transform.Find("Item");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트가 아이템인지 체크
        if (collision.CompareTag("ITEM"))
        {
            ItemStatus itemStatus = collision.GetComponent<ItemStatus>();

            // 플레이어 아이템 회수 담당 영역에 닿은 경우
            if (gameObject.name.Equals("ItemCapsule") && playerStatus.GetSpriteOff().Equals(false))
            {
                if (itemStatus.GetItemSize().Equals(ItemSize.ITEMSIZE_SMALL))
                {
                    // 효과음 재생
                    SoundManager.instance.PlaySE(21);

                    switch (itemStatus.GetItemType())
                    {
                        case ItemType.ITEMTYPE_POWER:
                            GameData.currentScore += 10;
                            if (GameData.currentPower < 4.0f)
                            {
                                GameData.currentPower += 0.01f;
                                if (GameData.currentPower.Equals(4.0f))
                                {
                                    // 효과음 재생
                                    SoundManager.instance.PlaySE(39);

                                    // 풀 파워 메세지
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
                    // 효과음 재생
                    SoundManager.instance.PlaySE(21);

                    switch (itemStatus.GetItemType())
                    {
                        case ItemType.ITEMTYPE_POWER:
                            GameData.currentScore += 50;
                            if (GameData.currentPower < 4.0f)
                            {
                                GameData.currentPower += 0.05f;
                                if (GameData.currentPower >= 4.0f)
                                {
                                    // 효과음 재생
                                    SoundManager.instance.PlaySE(39);

                                    GameData.currentPower = 4.0f;
                                    // 풀 파워 메세지
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
                                // 효과음 재생
                                SoundManager.instance.PlaySE(39);

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
                            // 효과음 재생
                            SoundManager.instance.PlaySE(22);

                            GameData.currentScore += 150;
                            if (GameData.currentPower < 4.0f)
                            {
                                GameData.currentPower += 0.15f;
                                if (GameData.currentPower >= 4.0f)
                                {
                                    // 효과음 재생
                                    SoundManager.instance.PlaySE(39);

                                    GameData.currentPower = 4.0f;
                                    // 풀 파워 메세지
                                }
                            }
                            break;
                        case ItemType.ITEMTYPE_LIFE:
                            // 효과음 재생
                            SoundManager.instance.PlaySE(17);

                            GameData.currentPlayerLife++;
                            // 플레이어 잔기 추가 알림 메세지
                            break;
                        case ItemType.ITEMTYPE_LIFEFRAGMENT:
                            // 효과음 재생
                            SoundManager.instance.PlaySE(22);

                            GameData.currentPlayerLifeFragment++;
                            if (GameData.currentPlayerLifeFragment >= 8)
                            {
                                // 효과음 재생
                                SoundManager.instance.PlaySE(17);

                                GameData.currentPlayerLife++;
                                GameData.currentPlayerLifeFragment = 0;
                                // 플레이어 잔기 추가 알림 메세지
                            }
                            break;
                        case ItemType.ITEMTYPE_SPELL:
                            // 효과음 재생
                            SoundManager.instance.PlaySE(6);

                            GameData.currentPlayerSpell++;
                            // 플레이어 스펠 추가 알림 메세지
                            break;
                        case ItemType.ITEMTYPE_SPELLFRAGMENT:
                            // 효과음 재생
                            SoundManager.instance.PlaySE(22);

                            GameData.currentPlayerSpellFragment++;
                            if (GameData.currentPlayerSpellFragment >= 8)
                            {
                                // 효과음 재생
                                SoundManager.instance.PlaySE(6);

                                GameData.currentPlayerSpell++;
                                GameData.currentPlayerSpellFragment = 0;
                                // 플레이어 스펠 추가 알림 메세지
                            }
                            break;
                        case ItemType.ITEMTYPE_FULLPOWER:
                            // 효과음 재생
                            SoundManager.instance.PlaySE(22);

                            GameData.currentScore += 100000;
                            if (GameData.currentPower < 4.0f)
                            {
                                // 효과음 재생
                                SoundManager.instance.PlaySE(39);

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
            // 저속 이동 시에 활성화되는 플레이어 주변 아이템 자동 회수 영역에 닿은 경우
            else if (gameObject.name.Equals("ItemCircle") && playerStatus.GetSpriteOff().Equals(false))
            {
                if (playerStatus.GetSlowMove().Equals(true))
                {
                    itemStatus.SetPlayerFind(true);
                }
            }
            // 아이템이 화면 밑으로 벗어났을 경우 아이템 자동 제거
            else if (gameObject.name.Equals("ITEMDESTROYZONE"))
            {
                ClearItem(collision.gameObject);
            }
        }
    }

    // 아이템 제거 함수
    private void ClearItem(GameObject obj)
    {
        obj.transform.SetParent(itemParent);
        obj.transform.position = new Vector2(0.0f, 0.0f);
        obj.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        obj.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);

        obj.SetActive(false);
    }
}
