using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Stage1BulletFragmentation : MonoBehaviour
{
    private BulletManager bulletManager;
    private MovingBullet movingBullet;
    
    void Update()
    {
        Fragmentation();
    }

    public void Fragmentation()
    {
        bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();
        movingBullet = gameObject.GetComponent<MovingBullet>();

        // 탄막 2 발사 (파란색 원탄) (랜덤탄)
        if (movingBullet.bulletMoveSpeed <= 0.0f)
        {
            if (bulletManager.bulletPool.Count > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    EnemyFire.ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBulletTemp1"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = GameObject.Find("ENEMY").GetComponent<EnemyFire>().spriteCollection[23];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.04f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = Random.Range(4.0f, 6.0f);
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetRandomAimedBulletDestination();
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f);
                }

                bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();
                // bulletManager.bulletPool.Enqueue(gameObject);
                bulletManager.bulletPool.Push(gameObject);
                transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                gameObject.SetActive(false);
            }
            else
            {
                GameObject bullet = Instantiate(bulletManager.bulletObject);
                bullet.SetActive(false);
                bullet.transform.SetParent(bulletManager.bulletParent.transform);
                // bulletManager.bulletPool.Enqueue(bullet);
                bulletManager.bulletPool.Push(bullet);
            }
        }
    }
}
