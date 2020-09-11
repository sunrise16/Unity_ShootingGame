using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Stage5BulletCreate : MonoBehaviour
{
    private Transform enemyBullet;
    private GameObject playerObject;
    private BulletManager bulletManager;
    private EnemyFire enemyFire;

    void Start()
    {
        enemyBullet = GameObject.Find("BULLET").transform.GetChild(4);
        playerObject = GameObject.Find("PLAYER");
        bulletManager = GameObject.Find("BulletManager").transform.GetChild(0).GetComponent<BulletManager>();
        enemyFire = GameObject.Find("ENEMY").GetComponent<EnemyFire>();

        StartCoroutine(CreateBullet());
	}

    private IEnumerator CreateBullet()
    {
        yield return new WaitForSeconds(0.075f);

        while (true)
        {
            // 탄막 2 생성
            if (bulletManager.bulletPool.Count > 0)
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
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                spriteRenderer.sprite = enemyFire.spriteCollection[18];
                spriteRenderer.sortingOrder = 3;
                circleCollider2D.isTrigger = true;
                circleCollider2D.radius = 0.04f;
                circleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 0.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f);
            }
            else enemyFire.AddBulletPool();

            yield return new WaitForSeconds(0.15f);
        }
    }
}
