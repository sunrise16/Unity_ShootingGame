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
        movingBullet = gameObject.GetComponent<MovingBullet>();

        // 탄막 2 발사 (파란색 원탄) (랜덤탄)
        if (movingBullet.bulletMoveSpeed <= 0.0f)
        {
            bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet2").transform.Find("Bullet2_7").GetComponent<BulletManager>();

            if (bulletManager.bulletPool.Count > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = gameObject.transform.position;
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet2").transform.Find("Bullet2_7"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 4;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 6;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = Random.Range(4.0f, 6.0f);
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetRandomAimedBulletDestination();
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f);
                }

                bulletManager = GameObject.Find("BulletManager").transform.GetChild(0).GetComponent<BulletManager>();
                bulletManager.bulletPool.Enqueue(gameObject);
                gameObject.SetActive(false);
            }
            else
            {
                GameObject bullet = Instantiate(bulletManager.bulletObject);
                bullet.SetActive(false);
                bullet.transform.SetParent(bulletManager.bulletParent.transform);
                bulletManager.bulletPool.Enqueue(bullet);
            }
        }
    }
}
