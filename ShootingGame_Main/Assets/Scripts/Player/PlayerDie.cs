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
        itemPool = GameObject.Find("ITEM").transform.Find("Item");
        itemParent = GameObject.Find("ITEM").transform.Find("Item_Temp");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("ENEMY") || collision.CompareTag("BULLET_ENEMY")) && playerStatus.GetInvincible().Equals(false))
        {
            StartCoroutine(PlayerDieStart());
        }
    }

    private IEnumerator PlayerDieStart()
    {
        playerStatus.SetInvincible(true);
        playerStatus.SetSpriteOff(true);
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
        player.transform.position = new Vector3(0.0f, -5.0f, 0.0f);

        yield return new WaitForSeconds(1.0f);

        if (GameData.currentPlayerLife <= 0)
        {
            // 게임오버 처리
        }
        else
        {
            GameData.currentPlayerLife--;
        }
        playerBodySprite.enabled = true;
        playerHitPointSprite.enabled = true;
        iTween.MoveTo(player.gameObject, iTween.Hash("position", new Vector3(0.0f, -3.5f, 0.0f), "easetype", iTween.EaseType.linear, "time", 1.0f));
        playerStatus.SetSpriteOff(false);
        playerStatus.SetRespawn(true);
        playerStatus.SetBlinking(true);

        yield return new WaitForSeconds(1.0f);

        playerStatus.SetRespawn(false);

        yield return new WaitForSeconds(4.0f);

        playerStatus.SetInvincible(false);
        playerStatus.SetBlinking(false);
    }

    private void ItemDrop()
    {
        GameObject item;
        Vector2 spawnPosition;

        for (int i = 0; i < 7; i++)
        {
            spawnPosition = new Vector2(transform.position.x + Random.Range(-0.4f, 0.4f), transform.position.y + Random.Range(0.0f, 0.4f));
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
}
