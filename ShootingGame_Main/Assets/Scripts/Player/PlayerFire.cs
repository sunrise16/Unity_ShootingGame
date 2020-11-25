using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    private BulletManager bulletManagerPrimary;
    private BulletManager bulletManagerSecondary;
    private Transform firePoint;
    private Transform playerBullet1;
    private Transform playerBullet2;

	void Start()
    {
        bulletManagerPrimary = GameObject.Find("BulletManager").transform.Find("PlayerBullet1").GetComponent<BulletManager>();
        bulletManagerSecondary = GameObject.Find("BulletManager").transform.Find("PlayerBullet2").GetComponent<BulletManager>();
        firePoint = GameObject.Find("PLAYER").transform.Find("AttackPoint").transform;
        playerBullet1 = GameObject.Find("BULLET").transform.Find("PlayerBullet1");
        playerBullet2 = GameObject.Find("BULLET").transform.Find("PlayerBullet2");

        StartCoroutine(Fire());
	}
	
	private IEnumerator Fire()
    {
        while (true)
        {
            if (GetComponent<PlayerMove>().isDamaged.Equals(false))
            {
                if (Input.GetKey(KeyCode.Z))
                {
                    // 주무기 발사
                    for (int i = 0; i < 2; i++)
                    {
                        if (bulletManagerPrimary.bulletPool.Count > 0)
                        {
                            GameObject bullet = bulletManagerPrimary.bulletPool.Dequeue();
                            bullet.SetActive(true);
                            bullet.transform.position = firePoint.Find("PrimaryPoint" + (i + 1).ToString()).transform.position;
                            bullet.gameObject.tag = "BULLET_PLAYER";
                            bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY");
                            bullet.transform.SetParent(playerBullet1);
                            bullet.transform.GetChild(0).gameObject.SetActive(true);
                        }
                        // else
                        // {
                        //     GameObject bullet = Instantiate(bulletManagerPrimary.bulletObject);
                        //     bullet.SetActive(false);
                        //     bullet.transform.SetParent(bulletManagerPrimary.bulletParent.transform);
                        //     bulletManagerPrimary.bulletPool.Enqueue(bullet);
                        // }
                    }

                    // 보조무기 발사
                    for (int i = 0; i < (int)(GlobalData.currentPower * 0.5f) * 2; i++)
                    {
                        if (bulletManagerSecondary.bulletPool.Count > 0)
                        {
                            GameObject bullet = bulletManagerSecondary.bulletPool.Dequeue();
                            bullet.SetActive(true);
                            bullet.transform.position = firePoint.Find("SecondaryPoint" + (i + 1).ToString()).transform.position;
                            bullet.gameObject.tag = "BULLET_PLAYER";
                            bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_PLAYER_SECONDARY");
                            bullet.transform.SetParent(playerBullet2);
                        }
                        // else
                        // {
                        //     GameObject bullet = Instantiate(bulletManagerSecondary.bulletObject);
                        //     bullet.SetActive(false);
                        //     bullet.transform.SetParent(bulletManagerSecondary.bulletParent.transform);
                        //     bulletManagerSecondary.bulletPool.Enqueue(bullet);
                        // }
                    }

                    yield return new WaitForSeconds(0.08f);
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
