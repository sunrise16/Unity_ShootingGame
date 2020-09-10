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
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
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
                bullet.GetComponent<SpriteRenderer>().sprite = enemyFire.spriteCollection[18];
                bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                bullet.GetComponent<CircleCollider2D>().radius = 0.04f;
                bullet.GetComponent<CircleCollider2D>().enabled = false;
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                bullet.GetComponent<InitializeBullet>().isGrazed = false;
                bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 0.0f;
                bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetRandomAimedBulletDestination();
                float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f);
            }
            else enemyFire.AddBulletPool();

            yield return new WaitForSeconds(0.2f);
        }
    }
}
