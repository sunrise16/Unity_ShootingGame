using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private GameManager gameManager;
    private Transform bulletTransform;
    private Transform bulletManagerTransform;

    private float playerBulletSpeed;

	void Start()
    {
        gameManager = GameObject.Find("MANAGER").transform.Find("GameManager").GetComponent<GameManager>();
        bulletTransform = GameObject.Find("BULLET").transform;
        bulletManagerTransform = GameObject.Find("BulletManager").transform;

        playerBulletSpeed = 24.0f;
	}

    void Update()
    {
        transform.Translate(Vector2.right * playerBulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ENEMY") == true)
        {
            collision.gameObject.GetComponent<EnemyDatabase>().enemyCurrentHp -= 1.0f;
            if (collision.gameObject.GetComponent<EnemyDatabase>().enemyCurrentHp <= 0.0f)
            {
                StartCoroutine(gameManager.StageClear());
            }

            if (gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY"))
            {
                ClearPlayerBullet(1);
            }
            else if (gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_SECONDARY"))
            {
                ClearPlayerBullet(2);
            }
        }
        else if (gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY") &&
            (collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_INNER1") || collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_INNER2")))
        {
            ClearPlayerBullet(1);
        }
        else if (gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_SECONDARY") &&
            (collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_OUTER1") || collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_OUTER2")))
        {
            ClearPlayerBullet(2);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_ALL"))
        {
            if (gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY"))
            {
                ClearPlayerBullet(1);
            }
            else
            {
                ClearPlayerBullet(2);
            }
        }
    }

    public void ClearPlayerBullet(int bulletPoolIndex)
    {
        BulletManager bulletManager = bulletManagerTransform.GetChild(bulletPoolIndex).GetComponent<BulletManager>();
        bulletManager.bulletPool.Enqueue(gameObject);
        gameObject.transform.SetParent(bulletTransform.GetChild(bulletPoolIndex).transform);
        gameObject.transform.position = new Vector2(0.0f, 0.0f);
        gameObject.SetActive(false);
    }
}
