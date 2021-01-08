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

            // 반격탄 발사 설정이 되어있을 경우
            if (enemyStatus.GetCounter().Equals(true))
            {
                enemyFire.EnemyCounter(enemyFire.GetEnemyCounterPatternNumber());
            }

            // 적 제거 실행
            StartCoroutine(Destroy(false));
            enemyStatus.SetEnemyCurrentHP(1.0f);
        }
    }

    // 적 제거 함수
    public IEnumerator Destroy(bool autoDestroy)
    {
        // 아이템 드랍 (적이 자동으로 제거되지 않고 플레이어의 탄에 죽은 경우)
        if (autoDestroy.Equals(false))
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

        // 충돌 판정 및 스프라이트 애니메이션 비활성화
        circleCollider2D.radius = 0.5f;
        circleCollider2D.enabled = false;
        animator.runtimeAnimatorController = null;

        yield return new WaitForSeconds(1.5f);

        // 적의 상태, 위치값 등의 정보 초기화
        InitEnemyState();

        // 비활성화
        gameObject.SetActive(false);
    }

    // 일정 시간 경과 후 적 자동 제거 함수
    public IEnumerator EnemyAutoDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (gameObject.activeSelf.Equals(true))
        {
            StartCoroutine(Destroy(true));
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
