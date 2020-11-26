using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EraseBullet : MonoBehaviour
{
    private ObjectPool playerPrimaryBullet;
    private ObjectPool playerSecondaryBullet;
    private ObjectPool circleBullet;
    private ObjectPool capsuleBullet;
    private ObjectPool boxBullet;

    private Transform playerPrimaryBulletParent;
    private Transform playerSecondaryBulletParent;
    private Transform circleBulletParent;
    private Transform capsuleBulletParent;
    private Transform boxBulletParent;

    private void Start()
    {
        playerPrimaryBullet = GameObject.Find("BulletPool").transform.Find("PlayerBullet1").GetComponent<ObjectPool>();
        playerSecondaryBullet = GameObject.Find("BulletPool").transform.Find("PlayerBullet2").GetComponent<ObjectPool>();
        circleBullet = GameObject.Find("BulletPool").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Circle").GetComponent<ObjectPool>();
        capsuleBullet = GameObject.Find("BulletPool").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Capsule").GetComponent<ObjectPool>();
        boxBullet = GameObject.Find("BulletPool").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Rectangle").GetComponent<ObjectPool>();

        playerPrimaryBulletParent = GameObject.Find("BULLET").transform.Find("PlayerBullet1");
        playerSecondaryBulletParent = GameObject.Find("BULLET").transform.Find("PlayerBullet2");
        circleBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Circle");
        capsuleBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Capsule");
        boxBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Rectangle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BULLET_PLAYER"))
        {
            ClearPlayerBullet(collision.gameObject, (collision.gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY"))) ? 1 : 2);
        }
        else if (collision.CompareTag("BULLET_ENEMY"))
        {
            if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_INNER1")) &&
            collision.gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_ENEMY_INNER1")))
            {
                ClearBullet(collision.gameObject);
            }
            else if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_INNER2")) &&
                collision.gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_ENEMY_INNER2")))
            {
                ClearBullet(collision.gameObject);
            }
            else if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_OUTER1")) &&
                collision.gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_ENEMY_OUTER1")))
            {
                ClearBullet(collision.gameObject);
            }
            else if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_OUTER2")) &&
                collision.gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_ENEMY_OUTER2")))
            {
                ClearBullet(collision.gameObject);
            }
        }
        else if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_ALL")) &&
            (collision.CompareTag("BULLET_ENEMY") || collision.CompareTag("BULLET_PLAYER")))
        {
            ClearBulletAll(collision.gameObject);
        }
    }

    private void ClearPlayerBullet(GameObject bullet, int bulletType)
    {
        switch (bulletType)
        {
            case 1:
                playerPrimaryBullet.objectPool.Enqueue(bullet);
                bullet.transform.SetParent(playerPrimaryBulletParent);
                break;
            case 2:
                playerSecondaryBullet.objectPool.Enqueue(bullet);
                bullet.transform.SetParent(playerSecondaryBulletParent);
                break;
            default:
                break;
        }
        bullet.transform.position = new Vector2(0.0f, 0.0f);
        bullet.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        bullet.transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);

        bullet.SetActive(false);
    }

    private void ClearBullet(GameObject bullet)
    {
        if (bullet.GetComponent<CircleCollider2D>())
        {
            circleBullet.objectPool.Enqueue(bullet);
            bullet.transform.SetParent(circleBulletParent);
        }
        else if (bullet.GetComponent<CapsuleCollider2D>())
        {
            capsuleBullet.objectPool.Enqueue(bullet);
            bullet.transform.SetParent(capsuleBulletParent);
        }
        else if (bullet.GetComponent<BoxCollider2D>())
        {
            boxBullet.objectPool.Enqueue(bullet);
            bullet.transform.SetParent(boxBulletParent);
        }
        bullet.transform.position = new Vector2(0.0f, 0.0f);
        bullet.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        bullet.transform.localScale = new Vector3(1.8f, 1.8f, 1.0f);

        bullet.SetActive(false);
    }

    private void ClearBulletAll(GameObject bullet)
    {

    }
}
