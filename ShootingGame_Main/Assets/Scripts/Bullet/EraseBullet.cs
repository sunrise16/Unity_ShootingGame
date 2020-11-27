using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EraseBullet : MonoBehaviour
{
    private ObjectPool circleBullet;
    private ObjectPool capsuleBullet;
    private ObjectPool boxBullet;

    private Transform circleBulletParent;
    private Transform capsuleBulletParent;
    private Transform boxBulletParent;

    private void Start()
    {
        circleBullet = GameObject.Find("BulletPool").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Circle").GetComponent<ObjectPool>();
        capsuleBullet = GameObject.Find("BulletPool").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Capsule").GetComponent<ObjectPool>();
        boxBullet = GameObject.Find("BulletPool").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Rectangle").GetComponent<ObjectPool>();

        circleBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Circle");
        capsuleBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Capsule");
        boxBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Rectangle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BULLET_ENEMY"))
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
        else if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_ALL")) && collision.CompareTag("BULLET_ENEMY"))
        {
            ClearBulletAll(collision.gameObject);
        }
    }

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

    private void ClearBulletAll(GameObject bullet)
    {

    }
}
