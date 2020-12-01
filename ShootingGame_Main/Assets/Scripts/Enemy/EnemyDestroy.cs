using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyDestroy : MonoBehaviour
{
    private GameManager gameManager;
    private EnemyStatus enemyStatus;
    private EnemyFire enemyFire;
    private Transform enemyParent;
    private Transform itemPool;
    private Transform itemParent;

    private void Start()
    {
        gameManager = GameObject.Find("MANAGER").transform.Find("GameManager").GetComponent<GameManager>();
        enemyStatus = GetComponent<EnemyStatus>();
        enemyFire = GetComponent<EnemyFire>();
        enemyParent = GameObject.Find("CHARACTER").transform.Find("Enemy");
        itemPool = GameObject.Find("ITEM").transform.Find("Item");
        itemParent = GameObject.Find("ITEM").transform.Find("Item_Temp");
    }

    private void Update()
    {
        if (enemyStatus.GetEnemyCurrentHP() <= 0.0f)
        {
            // 모든 적 코루틴 제거
            StopAllCoroutines();
            enemyFire.StopAllCoroutines();
            
            Destroy(false);
        }
    }

    public void Destroy(bool autoDestroy)
    {
        EnemyStatus enemyStatus = GetComponent<EnemyStatus>();
        // 아이템 드랍
        if (autoDestroy.Equals(false))
        {
            for (int i = 0; i < 11; i++)
            {
                if (enemyStatus.GetEnemyItem(i) > 0)
                {
                    ItemDrop(i, enemyStatus.GetEnemyItem(i));
                }
            }
        }
        enemyStatus.SetEnemyType(EnemyType.ENEMYTYPE_NONE);
        enemyStatus.SetEnemyMaxHP(1.0f);
        enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyMaxHP());

        transform.position = new Vector2(0.0f, 0.0f);
        gameObject.layer = LayerMask.NameToLayer("Default");
        transform.SetParent(enemyParent);
        transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
        GetComponent<CircleCollider2D>().radius = 0.5f;
        GetComponent<Animator>().runtimeAnimatorController = null;

        gameObject.SetActive(false);
    }

    public IEnumerator EnemyAutoDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(true);
    }

    private void ItemDrop(int itemNumber, int itemCount)
    {
        GameObject item;
        Vector2 spawnPosition;

        for (int i = 0; i < itemCount; i++)
        {
            spawnPosition = new Vector2(transform.position.x + Random.Range(-1.0f, 1.0f), transform.position.y + Random.Range(-1.0f, 1.0f));
            item = itemPool.GetChild(0).gameObject;
            item.SetActive(true);
            item.transform.position = spawnPosition;
            item.transform.SetParent(itemParent);
            
            ItemStatus itemStatus = item.GetComponent<ItemStatus>();
            BoxCollider2D boxCollider2D = item.GetComponent<BoxCollider2D>();
            SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
            switch (itemNumber)
            {
                case 1:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_SMALL);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_POWER);
                    boxCollider2D.size = new Vector2(0.1f, 0.1f);
                    spriteRenderer.sprite = gameManager.itemSprite[0];
                    break;
                case 2:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_SMALL);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_SCORE);
                    boxCollider2D.size = new Vector2(0.1f, 0.1f);
                    spriteRenderer.sprite = gameManager.itemSprite[1];
                    break;
                case 3:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_MEDIUM);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_POWER);
                    boxCollider2D.size = new Vector2(0.15f, 0.15f);
                    spriteRenderer.sprite = gameManager.itemSprite[2];
                    break;
                case 4:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_MEDIUM);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_SCORE);
                    boxCollider2D.size = new Vector2(0.15f, 0.15f);
                    spriteRenderer.sprite = gameManager.itemSprite[3];
                    break;
                case 5:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_MEDIUM);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_FULLPOWER);
                    boxCollider2D.size = new Vector2(0.15f, 0.15f);
                    spriteRenderer.sprite = gameManager.itemSprite[4];
                    break;
                case 6:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_POWER);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[5];
                    break;
                case 7:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_LIFE);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[6];
                    break;
                case 8:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_LIFEFRAGMENT);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[7];
                    break;
                case 9:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_SPELL);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[8];
                    break;
                case 10:
                    itemStatus.SetItemSize(ItemSize.ITEMSIZE_LARGE);
                    itemStatus.SetItemType(ItemType.ITEMTYPE_SPELLFRAGMENT);
                    boxCollider2D.size = new Vector2(0.3f, 0.3f);
                    spriteRenderer.sprite = gameManager.itemSprite[9];
                    break;
                case 11:
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
