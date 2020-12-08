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

    private float playerBulletSpeed;

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
        transform.Translate(Vector2.right * playerBulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("ENEMY_BODY")))
        {
            EnemyStatus enemyStatus = collision.gameObject.GetComponent<EnemyStatus>();
            collision.gameObject.GetComponent<EnemyStatus>().SetEnemyCurrentHP
                (collision.gameObject.GetComponent<EnemyStatus>().GetEnemyCurrentHP() -
                (1.0f + GameData.currentPower) * (gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY")) ? 1.0f : 0.5f));
            ClearPlayerBullet(gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY")) ? 1 : 2);
        }
        else if (collision.gameObject.name.Equals("PLAYERBULLETDESTROYZONE"))
        {
            ClearPlayerBullet(gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY")) ? 1 : 2);
        }
    }

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
