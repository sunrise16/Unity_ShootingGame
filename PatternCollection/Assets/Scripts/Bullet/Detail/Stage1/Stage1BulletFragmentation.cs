using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Stage1BulletFragmentation : MonoBehaviour
{
    private Stage1BulletFragmentation stage1BulletFragmentation;
    private Transform enemyBullet;
    private GameObject playerObject;
    private BulletManager bulletManager;
    private MovingBullet movingBullet;
    private EnemyFire enemyFire;
    
    void Start()
    {
        stage1BulletFragmentation = GetComponent<Stage1BulletFragmentation>();
        enemyBullet = GameObject.Find("BULLET").transform.Find("EnemyBullet");
        playerObject = GameObject.Find("PLAYER");
        bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();
        movingBullet = GetComponent<MovingBullet>();
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
                        bullet.GetComponent<SpriteRenderer>().sprite = enemyFire.spriteCollection[23];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.04f;
                        bullet.GetComponent<CircleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = Random.Range(4.0f, 6.0f);
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetRandomAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f);
                    }
                    
                    bulletManager.bulletPool.Enqueue(gameObject);
                    transform.SetParent(enemyBullet);
                    Destroy(stage1BulletFragmentation);
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
