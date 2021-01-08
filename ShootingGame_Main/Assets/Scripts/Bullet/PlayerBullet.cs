using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private GameManager gameManager;
    private Transform bulletTransform;
    private Transform bulletPoolTransform;
    private Transform playerPrimaryBulletParent;
    private Transform playerSecondaryBulletParent;

    private float playerBulletSpeed;                    // 플레이어 탄막 속도

	private void Start()
    {
        gameManager = GameObject.Find("MANAGER").transform.Find("GameManager").GetComponent<GameManager>();
        bulletTransform = GameObject.Find("BULLET").transform;
        bulletPoolTransform = GameObject.Find("BulletPool").transform;
        playerPrimaryBulletParent = GameObject.Find("BULLET").transform.Find("PlayerBullet1");
        playerSecondaryBulletParent = GameObject.Find("BULLET").transform.Find("PlayerBullet2");

        playerBulletSpeed = 24.0f;
	}

    private void Update()
    {
        // 주기적으로 위로 전진
        transform.Translate(Vector2.right * playerBulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌 대상이 적 몸체일 경우
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("ENEMY_BODY")))
        {
            // 탄막 제거와 동시에 적에게 데미지를 가하기
            EnemyStatus enemyStatus = collision.gameObject.GetComponent<EnemyStatus>();
            enemyStatus.SetEnemyCurrentHP(enemyStatus.GetEnemyCurrentHP() -
                ((1.0f + GameData.currentPower) * (gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY")) ? 1.0f : 0.5f)));

            // 효과음 재생
            if (enemyStatus.GetEnemyType().Equals(EnemyType.ENEMYTYPE_LMINION) || enemyStatus.GetEnemyType().Equals(EnemyType.ENEMYTYPE_BOSS))
            {
                if (enemyStatus.GetEnemyCurrentHPRate() < 0.1f)
                {
                    SoundManager.instance.PlaySE(12);
                }
                else
                {
                    SoundManager.instance.PlaySE(11);
                }
            }
            else
            {
                SoundManager.instance.PlaySE(11);
            }

            // 점수 증가
            GameData.currentScore += 10;

            // 플레이어 탄막 제거
            ClearPlayerBullet(gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY")) ? 1 : 2);
        }
        // 충돌 대상이 화면 바깥의 플레이어 탄막 제거 영역일 경우
        else if (collision.gameObject.name.Equals("PLAYERBULLETDESTROYZONE"))
        {
            // 플레이어 탄막 제거
            ClearPlayerBullet(gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY")) ? 1 : 2);
        }
    }

    // 플레이어 탄막 제거 함수
    private void ClearPlayerBullet(int bulletType)
    {
        switch (bulletType)
        {
            case 1:
                transform.SetParent(playerPrimaryBulletParent);
                transform.GetChild(0).gameObject.SetActive(false);
                break;
            case 2:
                transform.SetParent(playerSecondaryBulletParent);
                break;
            default:
                break;
        }
        transform.position = new Vector2(0.0f, 0.0f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
        gameObject.SetActive(false);
    }
}
