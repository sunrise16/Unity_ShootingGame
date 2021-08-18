using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    private GameManager gameManager;
    private Transform player;
    private SpriteRenderer playerBodySprite;
    private SpriteRenderer playerHitPointSprite;
    private PlayerStatus playerStatus;

    private Transform circleBulletParent;
    private Transform capsuleBulletParent;
    private Transform boxBulletParent;
    private Transform itemPool;
    private Transform itemParent;

    private int itemCount;

    private void Start()
    {
        gameManager = GameObject.Find("MANAGER").transform.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("CHARACTER").transform.Find("Player");
        playerBodySprite = GetComponent<SpriteRenderer>();
        playerHitPointSprite = player.transform.Find("HitPoint").GetComponent<SpriteRenderer>();
        playerStatus = player.GetComponent<PlayerStatus>();

        circleBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Circle");
        capsuleBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Capsule");
        boxBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Rectangle");
        itemPool = GameObject.Find("ITEM").transform.Find("Item");
        itemParent = GameObject.Find("ITEM").transform.Find("Item_Temp");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 무적 상태가 아닌 상황에서 적 또는 적 탄막에 피격당했을 경우
        if ((collision.CompareTag("ENEMY") || collision.CompareTag("BULLET_ENEMY")) && playerStatus.GetInvincible().Equals(false))
        {
            // 탄막 제거
            if (collision.CompareTag("BULLET_ENEMY"))
            {
                ClearBullet(collision.gameObject);
            }

            // 사망 처리
            StartCoroutine(PlayerDieStart());
        }
    }

    // 플레이어 사망 처리 코루틴
    private IEnumerator PlayerDieStart()
    {
        // 효과음 재생
        SoundManager.instance.PlaySE(35);

        playerStatus.SetInvincible(true);
        playerStatus.SetSpriteOff(true);
        playerStatus.SetRespawn(true);
        playerBodySprite.enabled = false;
        playerHitPointSprite.enabled = false;
        GameData.currentPower -= 1.0f;
        if (GameData.currentPower <= 0.0f)
        {
            GameData.currentPower = 0.0f;
        }
        itemCount = itemParent.childCount;
        for (int i = 0; i < itemCount; i++)
        {
            GameObject item = itemParent.GetChild(i).gameObject;
            ItemStatus itemStatus = item.GetComponent<ItemStatus>();
            itemStatus.SetPlayerFind(false);
        }
        ItemDrop();

        yield return new WaitForSeconds(1.0f);

        if (GameData.currentPlayerLife <= 0)
        {
            // 게임 종료
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit()
#endif
        }
        else
        {
            GameData.currentPlayerLife--;
        }
        player.transform.position = new Vector3(0.0f, -5.0f, 0.0f);
        playerBodySprite.enabled = true;
        playerHitPointSprite.enabled = true;
        iTween.MoveTo(player.gameObject, iTween.Hash("position", new Vector3(0.0f, -3.5f, 0.0f), "easetype", iTween.EaseType.linear, "time", 1.0f));
        playerStatus.SetSpriteOff(false);
        playerStatus.SetBlinking(true);

        yield return new WaitForSeconds(1.0f);

        playerStatus.SetRespawn(false);

        yield return new WaitForSeconds(4.0f);

        playerStatus.SetInvincible(false);
        playerStatus.SetBlinking(false);
    }

    // 아이템 드랍 함수 (플레이어 피탄 시)
    private void ItemDrop()
    {
        GameObject item;
        Vector2 spawnPosition;

        // 중 사이즈 파워 아이템 7개 드랍
        for (int i = 0; i < 7; i++)
        {
            spawnPosition = new Vector2(transform.position.x + Random.Range(-0.6f, 0.6f), transform.position.y + Random.Range(0.0f, 0.4f));
            item = itemPool.GetChild(0).gameObject;
            item.SetActive(true);
            item.transform.position = spawnPosition;
            item.transform.SetParent(itemParent);

            ItemStatus itemStatus = item.GetComponent<ItemStatus>();
            BoxCollider2D boxCollider2D = item.GetComponent<BoxCollider2D>();
            SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
            itemStatus.SetItemSize(ItemSize.ITEMSIZE_MEDIUM);
            itemStatus.SetItemType(ItemType.ITEMTYPE_POWER);
            boxCollider2D.size = new Vector2(0.15f, 0.15f);
            spriteRenderer.sprite = gameManager.itemSprite[2];
        }
    }

    // 탄막 제거 함수
    private void ClearBullet(GameObject bullet)
    {
        if (bullet.GetComponent<CircleCollider2D>())
        {
            bullet.transform.SetParent(circleBulletParent);
        }
        else if (bullet.GetComponent<CapsuleCollider2D>())
        {
            bullet.transform.SetParent(capsuleBulletParent);
        }
        else if (bullet.GetComponent<BoxCollider2D>())
        {
            bullet.transform.SetParent(boxBulletParent);
        }
        bullet.transform.position = new Vector2(0.0f, 0.0f);
        bullet.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        bullet.transform.localScale = new Vector3(1.8f, 1.8f, 1.0f);

        bullet.SetActive(false);
    }
}
