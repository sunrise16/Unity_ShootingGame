using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyDestroy : MonoBehaviour
{
    private GameManager gameManager;
    private EnemyStatus enemyStatus;
    private EnemyFire enemyFire;
    private EnemyMove enemyMove;
    private CircleCollider2D circleCollider2D;
    private Animator animator;
    private Transform enemyParent;
    private Transform itemPool;
    private Transform itemParent;

    private void Start()
    {
        gameManager = GameObject.Find("MANAGER").transform.Find("GameManager").GetComponent<GameManager>();
        enemyStatus = GetComponent<EnemyStatus>();
        enemyFire = GetComponent<EnemyFire>();
        enemyMove = GetComponent<EnemyMove>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        enemyParent = GameObject.Find("CHARACTER").transform.Find("Enemy");
        itemPool = GameObject.Find("ITEM").transform.Find("Item");
        itemParent = GameObject.Find("ITEM").transform.Find("Item_Temp");
    }

    private void Update()
    {
        // 체력이 0 이하로 내려갔을 경우
        if (enemyStatus.GetEnemyCurrentHP() <= 0.0f)
        {
            // 돌아가고 있는 모든 적 코루틴 중지
            StopAllCoroutines();
            enemyFire.StopAllCoroutines();
            enemyMove.StopAllCoroutines();

            // 적 종류가 보스일 경우
            if (enemyStatus.GetEnemyType().Equals(EnemyType.ENEMYTYPE_BOSS))
            {
                switch (GameData.currentStage)
                {
                    case 1:
                    case 2:
                        if (GameData.currentChapter < 6)
                        {
                            StartCoroutine(Destroy(false, true, false));
                            GameData.currentChapter++;
                            enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyMaxHPArray()[GameData.currentChapter - 1]);
                        }
                        else
                        {
                            StartCoroutine(Destroy(false, true, true));
                            enemyStatus.SetEnemyCurrentHP(1.0f);
                            GameData.currentStage++;
                            GameData.currentChapter = 1;
                        }
                        break;
                    case 3:
                    case 5:
                        if (GameData.currentChapter < 8)
                        {
                            StartCoroutine(Destroy(false, true, false));
                            GameData.currentChapter++;
                            enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyMaxHPArray()[GameData.currentChapter - 1]);
                        }
                        else
                        {
                            StartCoroutine(Destroy(false, true, true));
                            enemyStatus.SetEnemyCurrentHP(1.0f);
                            GameData.currentStage++;
                            GameData.currentChapter = 1;
                        }
                        break;
                    case 4:
                        if (GameData.currentChapter < 9)
                        {
                            StartCoroutine(Destroy(false, true, false));
                            GameData.currentChapter++;
                            enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyMaxHPArray()[GameData.currentChapter - 1]);
                        }
                        else
                        {
                            StartCoroutine(Destroy(false, true, true));
                            enemyStatus.SetEnemyCurrentHP(1.0f);
                            GameData.currentStage++;
                            GameData.currentChapter = 1;
                        }
                        break;
                    case 6:
                        if (GameData.currentChapter < 11)
                        {
                            StartCoroutine(Destroy(false, true, false));
                            GameData.currentChapter++;
                            enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyMaxHPArray()[GameData.currentChapter - 1]);
                        }
                        else
                        {
                            StartCoroutine(Destroy(false, true, true));
                            enemyStatus.SetEnemyCurrentHP(1.0f);
                            GameData.currentStage++;
                            GameData.currentChapter = 1;
                        }
                        break;
                    case 7:
                        if (GameData.currentChapter < 21)
                        {
                            StartCoroutine(Destroy(false, true, false));
                            GameData.currentChapter++;
                            enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyMaxHPArray()[GameData.currentChapter - 1]);
                        }
                        else
                        {
                            StartCoroutine(Destroy(false, true, true, true));
                            enemyStatus.SetEnemyCurrentHP(1.0f);
                        }
                        break;
                    default:
                        break;
                }
            }
            // 적 종류가 미니언일 경우
            else if (!enemyStatus.GetEnemyType().Equals(EnemyType.ENEMYTYPE_BOSS) && !enemyStatus.GetEnemyType().Equals(EnemyType.ENEMYTYPE_NONE))
            {
                // 반격탄 발사 설정이 되어있을 경우
                if (enemyStatus.GetCounter().Equals(true))
                {
                    enemyFire.EnemyCounter(enemyFire.GetEnemyCounterPatternNumber());
                }
                StartCoroutine(Destroy(false));
                enemyStatus.SetEnemyCurrentHP(1.0f);
            }
        }
    }

    // 적 제거 함수
    public IEnumerator Destroy(bool isAutoDestroy, bool isBoss = false, bool isFinalChapter = false, bool isEnd = false)
    {
        // 아이템 드랍 (적이 자동으로 제거되지 않고 플레이어의 탄에 죽은 경우)
        if (isAutoDestroy.Equals(false))
        {
            // 효과음 재생
            SoundManager.instance.PlaySE(14);

            for (int i = 0; i < 11; i++)
            {
                if (enemyStatus.GetEnemyItem(i) > 0)
                {
                    ItemDrop(i, enemyStatus.GetEnemyItem(i));
                }
            }
        }

        // 적 종류에 따라 다른 로직 실행
        if (isBoss.Equals(false) || (isBoss.Equals(true) && isFinalChapter.Equals(true)))
        {
            // 충돌 판정 및 스프라이트 애니메이션 비활성화
            circleCollider2D.radius = 0.5f;
            circleCollider2D.enabled = false;
            animator.runtimeAnimatorController = null;

            // 적의 상태, 위치값 등의 정보 초기화
            InitEnemyState();

            // 비활성화
            gameObject.SetActive(false);

            // 다음 스테이지 실행
            if (isBoss.Equals(true) && isFinalChapter.Equals(true))
            {
                yield return new WaitForSeconds(1.5f);

                gameManager.StageStart();
            }
        }
        else
        {
            Vector3 originPosition = new Vector3(0.0f, 3.0f, 0.0f);
            enemyStatus.SetInvincible(true);
            StartCoroutine(enemyMove.EnemyMoveOnce(originPosition, iTween.EaseType.easeInOutQuad, 1.0f));

            yield return new WaitForSeconds(1.5f);

            enemyStatus.SetInvincible(false);
            enemyFire.EnemyBossAttack(GameData.currentStage, GameData.currentChapter);
        }

        // 게임 종료
        if (isEnd.Equals(true))
        {
            yield return new WaitForSeconds(1.0f);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit()
#endif
        }
    }

    // 일정 시간 경과 후 적 자동 제거 함수
    public IEnumerator EnemyAutoDestroy(float waitTime, bool isBoss = false, bool isFinalChapter = false, bool isEnd = false)
    {
        yield return new WaitForSeconds(waitTime);

        if (gameObject.activeSelf.Equals(true))
        {
            StartCoroutine(Destroy(true, isBoss, isFinalChapter, isEnd));
        }
    }

    // 적 정보 초기화 함수
    private void InitEnemyState()
    {
        enemyStatus.SetEnemyType(EnemyType.ENEMYTYPE_NONE);
        enemyStatus.SetEnemyNumber(0);
        enemyStatus.SetEnemyCurrentHP(1.0f);
        enemyStatus.SetEnemyMaxHP(1.0f);
        enemyStatus.SetCounter(false);
        enemyStatus.SetScreenOut(false);

        enemyFire.SetEnemyPatternNumber(0);
        enemyFire.SetEnemyCounterPatternNumber(0);
        enemyFire.SetEnemyAttackWaitTime(0.0f);
        enemyFire.SetEnemyAttackDelayTime(0.0f);
        enemyFire.SetEnemyPatternOnce(false);
        enemyFire.SetEnemyPatternRepeat(false);
        enemyFire.SetEnemyFireCount(0);
        enemyFire.SetEnemyAttackRepeatTime(0.0f);
        enemyFire.SetEnemyCustomPatternNumber(0);

        transform.SetParent(enemyParent);
        transform.position = new Vector2(0.0f, 0.0f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    // 아이템 드랍 함수
    private void ItemDrop(int itemNumber, int itemCount)
    {
        GameObject item;
        Vector2 spawnPosition;

        for (int i = 0; i < itemCount; i++)
        {
            spawnPosition = new Vector2(transform.position.x + Random.Range(-0.4f, 0.4f), transform.position.y + Random.Range(-0.6f, 0.6f));
            item = itemPool.GetChild(0).gameObject;
            item.SetActive(true);
            item.transform.position = spawnPosition;
            item.transform.SetParent(itemParent);
            
            ItemStatus itemStatus = item.GetComponent<ItemStatus>();
            BoxCollider2D boxCollider2D = item.GetComponent<BoxCollider2D>();
            SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
            switch (itemNumber)
            {
                case 0:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_SMALL);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_POWER);
                    boxCollider2D.size = new Vector2(0.1f, 0.1f);
                    spriteRenderer.sprite = gameManager.itemSprite[0];
                    break;
                case 1:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_SMALL);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_SCORE);
                    boxCollider2D.size = new Vector2(0.1f, 0.1f);
                    spriteRenderer.sprite = gameManager.itemSprite[1];
                    break;
                case 2:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_MEDIUM);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_POWER);
                    boxCollider2D.size = new Vector2(0.15f, 0.15f);
                    spriteRenderer.sprite = gameManager.itemSprite[2];
                    break;
                case 3:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_MEDIUM);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_SCORE);
                    boxCollider2D.size = new Vector2(0.15f, 0.15f);
                    spriteRenderer.sprite = gameManager.itemSprite[3];
                    break;
                case 4:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_MEDIUM);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_FULLPOWER);
                    boxCollider2D.size = new Vector2(0.15f, 0.15f);
                    spriteRenderer.sprite = gameManager.itemSprite[4];
                    break;
                case 5:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_POWER);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[5];
                    break;
                case 6:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_LIFE);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[6];
                    break;
                case 7:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_LIFEFRAGMENT);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[7];
                    break;
                case 8:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_SPELL);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[8];
                    break;
                case 9:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_SPELLFRAGMENT);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[9];
                    break;
                case 10:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_FULLPOWER);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[10];
                    break;
                default:
                    break;
            }
        }
    }
}
