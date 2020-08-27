using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private BulletManager bulletManager;

    private float playerBulletSpeed;

	void Start()
    {
        playerBulletSpeed = 24.0f;
	}
	
	void Update()
    {
        transform.Translate(Vector2.right * playerBulletSpeed * Time.deltaTime);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ENEMY")
        {
            if (gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY"))
            {
                ClearPlayerBullet(1);
            }
            else if (gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_SECONDARY"))
            {
                ClearPlayerBullet(2);
            }
            collision.gameObject.GetComponent<EnemyDatabase>().enemyCurrentHp -= 1.0f;
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
        bulletManager = GameObject.Find("BulletManager").transform.GetChild(bulletPoolIndex).GetComponent<BulletManager>();
        // bulletManager.bulletPool.Enqueue(gameObject);
        bulletManager.bulletPool.Push(gameObject);
        gameObject.transform.SetParent(GameObject.Find("BULLET").transform.GetChild(bulletPoolIndex).transform);
        gameObject.transform.position = new Vector2(0.0f, 0.0f);
        gameObject.SetActive(false);
    }
}
