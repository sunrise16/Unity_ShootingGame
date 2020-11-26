using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    private ObjectPool bulletPrimary;
    private ObjectPool bulletSecondary;
    private Transform firePoint;
    private Transform playerBullet1;
    private Transform playerBullet2;
    private Transform playerBullet1Parent;
    private Transform playerBullet2Parent;

	private void Start()
    {
        bulletPrimary = GameObject.Find("BulletPool").transform.Find("PlayerBullet1").GetComponent<ObjectPool>();
        bulletSecondary = GameObject.Find("BulletPool").transform.Find("PlayerBullet2").GetComponent<ObjectPool>();
        firePoint = GameObject.Find("AttackPoint").transform;
        playerBullet1 = GameObject.Find("BULLET").transform.Find("PlayerBullet1");
        playerBullet2 = GameObject.Find("BULLET").transform.Find("PlayerBullet2");
        playerBullet1Parent = GameObject.Find("BULLET").transform.Find("PlayerBullet_Temp").transform.Find("PlayerBullet_Temp1");
        playerBullet2Parent = GameObject.Find("BULLET").transform.Find("PlayerBullet_Temp").transform.Find("PlayerBullet_Temp2");

        StartCoroutine(Fire());
	}
	
	private IEnumerator Fire()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                // 주무기 발사
                for (int i = 0; i < 2; i++)
                {
                    if (bulletPrimary.objectPool.Count > 0)
                    {
                        GameObject bullet = bulletPrimary.objectPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = firePoint.Find("PrimaryPoint" + (i + 1).ToString()).transform.position;
                        bullet.gameObject.tag = "BULLET_PLAYER";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY");
                        bullet.transform.SetParent(playerBullet1Parent);
                        bullet.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    // else
                    // {
                    //     GameObject bullet = Instantiate(bulletPrimary.poolObject);
                    //     bullet.SetActive(false);
                    //     bullet.transform.SetParent(bulletPrimary.poolParent.transform);
                    //     bulletPrimary.objectPool.Enqueue(bullet);
                    // }
                }

                // 보조무기 발사
                for (int i = 0; i < (int)(GlobalData.currentPower * 0.5f) * 2; i++)
                {
                    if (bulletSecondary.objectPool.Count > 0)
                    {
                        GameObject bullet = bulletSecondary.objectPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = firePoint.Find("SecondaryPoint" + (i + 1).ToString()).transform.position;
                        bullet.gameObject.tag = "BULLET_PLAYER";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_PLAYER_SECONDARY");
                        bullet.transform.SetParent(playerBullet2Parent);
                    }
                    // else
                    // {
                    //     GameObject bullet = Instantiate(bulletSecondary.poolObject);
                    //     bullet.SetActive(false);
                    //     bullet.transform.SetParent(bulletSecondary.poolParent.transform);
                    //     bulletSecondary.objectPool.Enqueue(bullet);
                    // }
                }

                yield return new WaitForSeconds(0.08f);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
