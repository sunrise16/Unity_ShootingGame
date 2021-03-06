﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Stage1BulletFragmentation : MonoBehaviour
{
    private Transform enemyBullet;
    private GameObject playerObject;
    private BulletManager bulletManager;
    private MovingBullet movingBullet;
    private EraseBullet eraseBullet;
    private EnemyFire enemyFire;
    
    void Start()
    {
        enemyBullet = GameObject.Find("BULLET").transform.GetChild(4);
        playerObject = GameObject.Find("PLAYER");
        bulletManager = GameObject.Find("BulletManager").transform.GetChild(0).GetComponent<BulletManager>();
        movingBullet = GetComponent<MovingBullet>();
        eraseBullet = GetComponent<EraseBullet>();
        enemyFire = GameObject.Find("ENEMY").GetComponent<EnemyFire>();

        StartCoroutine(Fragmentation());
    }

    public IEnumerator Fragmentation()
    {
        while (true)
        {
            // 탄막 2 발사 (파란색 원탄) (랜덤탄)
            if (movingBullet.bulletMoveSpeed <= 0.0f)
            {
                Vector2 bulletFirePosition = transform.position;

                // 탄막 2 이펙트
                StartCoroutine(enemyFire.CreateBulletFireEffect(301, 1.0f, 12.0f, 0.4f, 0.1f, 0.35f, bulletFirePosition));

                if (bulletManager.bulletPool.Count > 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        EnemyFire.ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(enemyBullet);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                        CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                        InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                        MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                        spriteRenderer.sprite = enemyFire.spriteCollection[23];
                        spriteRenderer.sortingOrder = 3;
                        circleCollider2D.isTrigger = true;
                        circleCollider2D.radius = 0.04f;
                        circleCollider2D.enabled = false;
                        initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                        initializeBullet.bulletObject = bullet.gameObject;
                        initializeBullet.targetObject = playerObject;
                        initializeBullet.isGrazed = false;
                        movingBullet.bulletMoveSpeed = Random.Range(4.0f, 6.0f);
                        movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                        float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                        movingBullet.ChangeRotateAngle(angle - 90.0f);
                    }
                    
                    bulletManager.bulletPool.Enqueue(gameObject);
                    transform.SetParent(enemyBullet);
                    eraseBullet.ClearBullet();
                    gameObject.SetActive(false);
                    break;
                }
                else
                {
                    enemyFire.AddBulletPool();
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
