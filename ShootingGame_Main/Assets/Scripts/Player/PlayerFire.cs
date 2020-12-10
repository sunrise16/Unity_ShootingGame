using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private Transform player;
    private Transform[] primaryFirePoint;
    private Transform[] secondaryFirePoint;
    private Transform playerBullet1;
    private Transform playerBullet2;
    private Transform playerBullet1Parent;
    private Transform playerBullet2Parent;

    private float fireDelay;

	private void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
        player = GameObject.Find("CHARACTER").transform.Find("Player");
        primaryFirePoint = new Transform[2] { player.Find("AttackPoint").transform.Find("PrimaryPoint1"), player.Find("AttackPoint").transform.Find("PrimaryPoint2") };
        secondaryFirePoint = new Transform[4] { player.Find("AttackPoint").transform.Find("SecondaryPoint1"), player.Find("AttackPoint").transform.Find("SecondaryPoint2"),
            player.Find("AttackPoint").transform.Find("SecondaryPoint3"), player.Find("AttackPoint").transform.Find("SecondaryPoint4")};
        playerBullet1 = GameObject.Find("BULLET").transform.Find("PlayerBullet1");
        playerBullet2 = GameObject.Find("BULLET").transform.Find("PlayerBullet2");
        playerBullet1Parent = GameObject.Find("BULLET").transform.Find("PlayerBullet_Temp").transform.Find("PlayerBullet_Temp1");
        playerBullet2Parent = GameObject.Find("BULLET").transform.Find("PlayerBullet_Temp").transform.Find("PlayerBullet_Temp2");

        fireDelay = 0.0f;
    }

    private void Update()
    {
        if (playerStatus.GetSpriteOff().Equals(false) && playerStatus.GetRespawn().Equals(false))
        {
            // Z키 누르고 있을 때 플레이어 공격 (키버튼은 나중에 옵션 구현하면 바꿀수 있도록 할 예정)
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
                        bullet.transform.position = primaryFirePoint[i].position;
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
                        bullet.transform.position = secondaryFirePoint[i].position;
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
        else
        {
            fireDelay = 0.0f;
        }
    }
}
