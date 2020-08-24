using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    private BulletManager bulletManager;

	void Start()
    {
        StartCoroutine(Fire());
	}
	
	private IEnumerator Fire()
    {
        while (true)
        {
            if (GetComponent<PlayerMove>().isDamaged == false)
            {
                if (Input.GetKey(KeyCode.Z))
                {
                    // 주무기 발사
                    bulletManager = GameObject.Find("BulletManager").transform.Find("PlayerBullet1").GetComponent<BulletManager>();

                    for (int i = 0; i < 2; i++)
                    {
                        if (bulletManager.bulletPool.Count > 0)
                        {
                            GameObject bullet = bulletManager.bulletPool.Dequeue();
                            bullet.SetActive(true);
                            bullet.transform.position = GameObject.Find("PLAYER").transform.Find("AttackPoint").transform.Find("PrimaryPoint" + (i + 1).ToString()).transform.position;
                            bullet.gameObject.tag = "BULLET";
                            bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY");
                            bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("PlayerBullet1"));
                            bullet.transform.GetChild(0).GetComponent<PlayerBulletTailAlpha>().tailAlpha = 0.0f;
                            if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        }
                        else
                        {
                            GameObject bullet = Instantiate(bulletManager.bulletObject);
                            bullet.SetActive(false);
                            bullet.transform.SetParent(bulletManager.bulletParent.transform);
                            bulletManager.bulletPool.Enqueue(bullet);
                        }
                    }

                    // 보조무기 발사
                    bulletManager = GameObject.Find("BulletManager").transform.Find("PlayerBullet2").GetComponent<BulletManager>();

                    for (int i = 0; i < 4; i++)
                    {
                        if (bulletManager.bulletPool.Count > 0)
                        {
                            GameObject bullet = bulletManager.bulletPool.Dequeue();
                            bullet.SetActive(true);
                            bullet.transform.position = GameObject.Find("PLAYER").transform.Find("AttackPoint").transform.Find("Yinyang" + (i + 1).ToString()).transform.position;
                            bullet.gameObject.tag = "BULLET";
                            bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_PLAYER_SECONDARY");
                            bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("PlayerBullet2"));
                            if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        }
                        else
                        {
                            GameObject bullet = Instantiate(bulletManager.bulletObject);
                            bullet.SetActive(false);
                            bullet.transform.SetParent(bulletManager.bulletParent.transform);
                            bulletManager.bulletPool.Enqueue(bullet);
                        }
                    }

                    yield return new WaitForSeconds(0.08f);
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
