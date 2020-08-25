using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Stage5BulletCreate : MonoBehaviour
{
    private BulletManager bulletManager;

    void Start()
    {
        StartCoroutine(CreateBullet());
	}

    private IEnumerator CreateBullet()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet2").transform.Find("Bullet2_2").GetComponent<BulletManager>();

            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                bullet.transform.position = transform.position;
                bullet.gameObject.tag = "BULLET";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet2").transform.Find("Bullet2_2"));
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                if (!bullet.GetComponent<Stage5BulletCloneFire>()) bullet.AddComponent<Stage5BulletCloneFire>();
                bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                bullet.GetComponent<InitializeBullet>().isGrazed = false;
                bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 4;
                bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 1;
                bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 0.0f;
                bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetRandomAimedBulletDestination();
                float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f);
            }
            else
            {
                GameObject bullet = Instantiate(bulletManager.bulletObject);
                bullet.SetActive(false);
                bullet.transform.SetParent(bulletManager.bulletParent.transform);
                bulletManager.bulletPool.Enqueue(bullet);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
