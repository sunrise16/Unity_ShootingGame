using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    private Transform firePoint;
    private Transform playerBullet1;
    private Transform playerBullet2;
    private Transform playerBullet1Parent;
    private Transform playerBullet2Parent;

    private float fireDelay;

	private void Start()
    {
        firePoint = GameObject.Find("AttackPoint").transform;
        playerBullet1 = GameObject.Find("BULLET").transform.Find("PlayerBullet1");
        playerBullet2 = GameObject.Find("BULLET").transform.Find("PlayerBullet2");
        playerBullet1Parent = GameObject.Find("BULLET").transform.Find("PlayerBullet_Temp").transform.Find("PlayerBullet_Temp1");
        playerBullet2Parent = GameObject.Find("BULLET").transform.Find("PlayerBullet_Temp").transform.Find("PlayerBullet_Temp2");

        fireDelay = 0.0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            fireDelay = 0.08f;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            fireDelay += Time.deltaTime;
            if (fireDelay >= 0.08f)
            {
                // 주무기 발사
                for (int i = 0; i < 2; i++)
                {
                    GameObject bullet = playerBullet1.GetChild(i).gameObject;
                    bullet.SetActive(true);
                    bullet.transform.position = firePoint.Find("PrimaryPoint" + (i + 1).ToString()).transform.position;
                    bullet.gameObject.tag = "BULLET_PLAYER";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY");
                    bullet.transform.SetParent(playerBullet1Parent);
                    bullet.transform.GetChild(0).gameObject.SetActive(true);
                }

                // 보조무기 발사
                for (int i = 0; i < (int)(GameData.currentPower * 0.5f) * 2; i++)
                {
                    GameObject bullet = playerBullet2.GetChild(i).gameObject;
                    bullet.SetActive(true);
                    bullet.transform.position = firePoint.Find("SecondaryPoint" + (i + 1).ToString()).transform.position;
                    bullet.gameObject.tag = "BULLET_PLAYER";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_PLAYER_SECONDARY");
                    bullet.transform.SetParent(playerBullet2Parent);
                }

                fireDelay = 0.0f;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            fireDelay = 0.0f;
        }
    }
}
