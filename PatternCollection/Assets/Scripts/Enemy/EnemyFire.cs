using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    private BulletManager bulletManager;

    public void Fire(int stageNumber)
    {
        switch (stageNumber)
        {
            case 1:
                StartCoroutine(Stage7PatternAttack());
                break;
            case 2:
                StartCoroutine(Stage2PatternAttack());
                break;
            case 3:
                StartCoroutine(Stage3PatternAttack());
                break;
            case 4:
                StartCoroutine(Stage4PatternAttack());
                break;
            case 5:
                StartCoroutine(Stage5PatternAttack());
                break;
            case 6:
                StartCoroutine(Stage6PatternAttack());
                break;
            case 7:
                StartCoroutine(Stage7PatternAttack());
                break;
            case 8:
                // StartCoroutine(Stage8PatternAttack1());
                // StartCoroutine(Stage8PatternAttack2());
                break;
            case 9:
                // StartCoroutine(Stage9PatternAttack());
                break;
            case 10:
                // StartCoroutine(Stage10PatternAttack());
                break;
            default:
                break;
        }
    }

    #region 패턴 1 (동방지령전 4스테이지 - 코메이지 사토리 마리사A 4스펠 "리턴 인애니메이트니스" 열화판)
    public IEnumerator Stage1PatternAttack()
    {
        while (true)
        {
            // 탄막 1 발사 (파란색 환탄) (조준 방사탄)
            for (int i = 0; i < 16; i++)
            {
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet1").transform.Find("Bullet1_4").GetComponent<BulletManager>();

                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet1").transform.Find("Bullet1_4"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    if (!bullet.GetComponent<Stage1BulletFragmentation>()) bullet.AddComponent<Stage1BulletFragmentation>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 3;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 3;
                    if (i < 8)
                    {
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 4.0f;
                        bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeed = 0.1f;
                    }
                    else
                    {
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 8.0f;
                        bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeed = 0.2f;
                    }
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (45.0f * (i % 8)));
                }
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    bulletManager.bulletPool.Enqueue(bullet);
                }
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
    #endregion

    #region 패턴 2 (자작)
    public IEnumerator Stage2PatternAttack()
    {
        while (true)
        {
            bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet3").transform.Find("Bullet3_3").GetComponent<BulletManager>();

            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (좌측 상단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = new Vector2(Random.Range(-6.0f, -5.0f), Random.Range(-4.5f, 1.5f));
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE2");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet3").transform.Find("Bullet3_3"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 5;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 2;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 6.0f;
                    bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeed = 0.1f;
                    bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeedMin = 2.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetBulletDestination
                        (new Vector2(bullet.transform.position.x + Random.Range(0.5f, 3.0f), bullet.transform.position.y - 1.0f));
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
            }
            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (좌측 하단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = new Vector2(Random.Range(-6.0f, -5.0f), Random.Range(1.5f, 7.5f));
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE2");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet3").transform.Find("Bullet3_3"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 5;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 2;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 6.0f;
                    bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeed = 0.1f;
                    bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeedMin = 2.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetBulletDestination
                        (new Vector2(bullet.transform.position.x + Random.Range(0.5f, 3.0f), bullet.transform.position.y - 1.0f));
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
            }
            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (우측 상단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = new Vector2(Random.Range(5.0f, 6.0f), Random.Range(-4.5f, 1.5f));
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE2");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet3").transform.Find("Bullet3_3"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 5;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 2;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 6.0f;
                    bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeed = 0.1f;
                    bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeedMin = 2.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetBulletDestination
                        (new Vector2(bullet.transform.position.x - Random.Range(0.5f, 3.05f), bullet.transform.position.y - 1.0f));
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
            }
            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (우측 하단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = new Vector2(Random.Range(5.0f, 6.0f), Random.Range(1.5f, 7.5f));
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE2");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet3").transform.Find("Bullet3_3"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 5;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 2;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 6.0f;
                    bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeed = 0.1f;
                    bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeedMin = 2.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetBulletDestination
                        (new Vector2(bullet.transform.position.x - Random.Range(0.5f, 3.0f), bullet.transform.position.y - 1.0f));
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
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
    #endregion

    #region 패턴 3 (자작)
    public IEnumerator Stage3PatternAttack()
    {
        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                // 빈 탄막 발사
                bulletManager = GameObject.Find("BulletManager").transform.Find("EmptyBullet").GetComponent<BulletManager>();
                GameObject emptyBullet = bulletManager.bulletPool.Dequeue();

                if (bulletManager.bulletPool.Count > 0)
                {
                    emptyBullet.SetActive(true);
                    emptyBullet.transform.position = transform.position;
                    emptyBullet.gameObject.tag = "BULLET_EMPTY";
                    emptyBullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE2");
                    emptyBullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EmptyBullet"));
                    if (!emptyBullet.GetComponent<InitializeBullet>()) emptyBullet.AddComponent<InitializeBullet>();
                    if (!emptyBullet.GetComponent<MovingBullet>()) emptyBullet.AddComponent<MovingBullet>();
                    if (!emptyBullet.GetComponent<EraseBullet>()) emptyBullet.AddComponent<EraseBullet>();
                    emptyBullet.GetComponent<InitializeBullet>().bulletObject = emptyBullet.gameObject;
                    emptyBullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    emptyBullet.GetComponent<InitializeBullet>().isGrazed = true;
                    emptyBullet.GetComponent<InitializeBullet>().bulletPoolIndex = 0;
                    emptyBullet.GetComponent<MovingBullet>().bulletMoveSpeed = 2.0f;
                    emptyBullet.GetComponent<MovingBullet>().bulletRotateSpeed = 60.0f;
                    emptyBullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    emptyBullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    emptyBullet.GetComponent<MovingBullet>().bulletDestination = emptyBullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                    float angle = Mathf.Atan2(emptyBullet.GetComponent<MovingBullet>().bulletDestination.y, emptyBullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    emptyBullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 135.0f + (22.5f * i));
                }
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    bulletManager.bulletPool.Enqueue(bullet);
                }

                // 탄막 발사 (분홍색 나비탄) (빈 탄막을 중심으로 회전)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet3").transform.Find("Bullet3_5").GetComponent<BulletManager>();

                for (int j = 0; j < 12; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(emptyBullet.transform);
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = emptyBullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 5;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 4;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 2.0f;
                        bullet.GetComponent<MovingBullet>().bulletRotateSpeed = 180.0f;
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (30.0f * j));
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
    #endregion

    #region 패턴 4 (자작)
    public IEnumerator Stage4PatternAttack()
    {
        int angleMultiply = 1;

        while (true)
        {
            for (int i = 0; i < 18; i++)
            {
                // 탄막 1 발사 (무작위 색상 원탄) (랜덤탄)
                for (int j = 0; j < 18; j++)
                {
                    int bulletTextureIndex1 = Random.Range(1, 16);
                    bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet2").transform.Find("Bullet2_" + bulletTextureIndex1.ToString()).GetComponent<BulletManager>();

                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = gameObject.transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet2").transform.Find("Bullet2_" + bulletTextureIndex1.ToString()));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 4;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = bulletTextureIndex1 - 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = Random.Range(2.0f, 8.0f);
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetRandomAimedBulletDestination();
                        float angle = (Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x)
                            * Mathf.Rad2Deg) - 90.0f;
                        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }
                
                yield return new WaitForSeconds(0.04f);
            }

            yield return new WaitForSeconds(0.2f);

            int bulletTextureIndex2 = Random.Range(1, 16);
            Vector2 targetPosition = GameObject.Find("PLAYER").transform.position;
            bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet4").transform.Find("Bullet4_" + bulletTextureIndex2.ToString()).GetComponent<BulletManager>();

            // 탄막 2 발사 (무작위 색상 쐐기탄) (조준 방사탄)
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet4").transform.Find("Bullet4_" + bulletTextureIndex2.ToString()));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 6;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = bulletTextureIndex2 - 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 9.0f;
                        bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeed = 0.3f;
                        bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeedMin = 3.0f;
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(targetPosition);
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (22.5f * j) + (1.5f * i) * angleMultiply);
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }

                yield return new WaitForSeconds(0.05f);
            }

            if (angleMultiply == 1)
            {
                angleMultiply = -1;
            }
            else
            {
                angleMultiply = 1;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion

    #region 패턴 5 (동방홍마향 6스테이지 - 레밀리아 스칼렛 5스펠 "홍색의 환상향" 리메이크)
    public IEnumerator Stage5PatternAttack()
    {
        int angleMultiply = 1;

        while (true)
        {
            // 대옥탄에서 생성된 탄 활성화
            for (int i = 0; i < GameObject.Find("BULLET").transform.Find("Bullet2").transform.Find("Bullet2_2").childCount; i++)
            {
                GameObject bullet = GameObject.Find("BULLET").transform.Find("Bullet2").transform.Find("Bullet2_2").transform.GetChild(i).gameObject;
                if (bullet.activeSelf == true)
                {
                    bullet.GetComponent<Stage5BulletCloneFire>().BulletFire();
                }
            }

            // 탄막 발사 (빨간색 대옥탄) (조준 회전탄)
            for (int i = 0; i < 32; i++)
            {
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet5").transform.Find("Bullet5_1").GetComponent<BulletManager>();

                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet5").transform.Find("Bullet5_1"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    if (!bullet.GetComponent<Stage5BulletCreate>()) bullet.AddComponent<Stage5BulletCreate>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 7;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 0;
                    if (i < 16)
                    {
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f;
                        bullet.GetComponent<MovingBullet>().bulletRotateLimit = 1.2f;
                        bullet.GetComponent<MovingBullet>().bulletRotateSpeed = 30.0f;
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    }
                    else
                    {
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 4.0f;
                        bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeed = 0.03f;
                        bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeedMin = 3.0f;
                        bullet.GetComponent<MovingBullet>().bulletRotateLimit = 0.7f;
                        bullet.GetComponent<MovingBullet>().bulletRotateSpeed = 60.0f;
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                    }
                    bullet.GetComponent<MovingBullet>().bulletRotateTime = 0.0f;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (22.5f * (i % 16)) * angleMultiply);
                }
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    bulletManager.bulletPool.Enqueue(bullet);
                }
            }

            if (angleMultiply == 1)
            {
                angleMultiply = -1;
            }
            else
            {
                angleMultiply = 1;
            }

            // 랜덤한 지점으로 이동
            Vector3 targetPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);
            StartCoroutine(MoveToDestination(targetPosition, 1.5f));
            if (targetPosition.x <= transform.position.x)
            {
                transform.Find("Body").GetComponent<SpriteRenderer>().flipX = true;
                transform.Find("Body").GetComponent<EnemySprite>().isLeftMove = true;
            }
            else
            {
                transform.Find("Body").GetComponent<EnemySprite>().isRightMove = true;
            }

            yield return new WaitForSeconds(3.0f);
        }
    }
    #endregion

    #region 패턴 6 (동방홍마향 1스테이지 - 루미아 1통상 리메이크)
    public IEnumerator Stage6PatternAttack()
    {
        int bulletIndex = 2;

        while (true)
        {
            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                {
                    bulletIndex = 2;
                    bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet2").transform.Find("Bullet2_2").GetComponent<BulletManager>();
                }
                else
                {
                    bulletIndex = 3;
                    bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet2").transform.Find("Bullet2_3").GetComponent<BulletManager>();
                }

                for (int j = 0; j < 8 * (i + 1); j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet2").transform.Find("Bullet2_" + bulletIndex.ToString()));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 4;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = bulletIndex - 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f + (0.8f * (j % 8));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (10.0f * (j / 8)) - (5.0f * i));
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }

            // 랜덤한 지점으로 이동
            Vector3 targetPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);
            StartCoroutine(MoveToDestination(targetPosition, 1.8f));
            if (targetPosition.x <= transform.position.x)
            {
                transform.Find("Body").GetComponent<SpriteRenderer>().flipX = true;
                transform.Find("Body").GetComponent<EnemySprite>().isLeftMove = true;
            }
            else
            {
                transform.Find("Body").GetComponent<EnemySprite>().isRightMove = true;
            }

            yield return new WaitForSeconds(2.0f);
        }
    }
    #endregion

    #region 패턴 7 (동방홍마향 6스테이지 - 레밀리아 스칼렛 4스펠 "스칼렛 마이스터")
    public IEnumerator Stage7PatternAttack()
    {
        while (true)
        {
            // 플레이어 조준 발사 6회
            for (int i = 0; i < 6; i++)
            {
                // 탄막 1 발사 (빨간색 대옥탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet5").transform.Find("Bullet5_1").GetComponent<BulletManager>();

                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet2").transform.Find("Bullet2_3"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 7;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 0;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 5.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
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

                // 탄막 2 발사 (빨간색 환탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet1").transform.Find("Bullet1_2").GetComponent<BulletManager>();

                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet1").transform.Find("Bullet1_2"));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 4;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.5f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (Random.Range(-15.0f, 15.0f)));
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet7").transform.Find("Bullet7_2").GetComponent<BulletManager>();

                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet7").transform.Find("Bullet7_2"));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 9;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (Random.Range(-30.0f, 30.0f)));
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }

                yield return new WaitForSeconds(0.07f);
            }

            // 랜덤한 지점으로 이동
            Vector3 targetPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);
            StartCoroutine(MoveToDestination(targetPosition, 1.0f));
            if (targetPosition.x <= transform.position.x)
            {
                transform.Find("Body").GetComponent<SpriteRenderer>().flipX = true;
                transform.Find("Body").GetComponent<EnemySprite>().isLeftMove = true;
            }
            else
            {
                transform.Find("Body").GetComponent<EnemySprite>().isRightMove = true;
            }

            Vector2 playerPosition = GameObject.Find("PLAYER").transform.position;

            // 시계 방향 발사 11회
            for (int i = 0; i < 11; i++)
            {
                // 탄막 1 발사 (빨간색 대옥탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet5").transform.Find("Bullet5_1").GetComponent<BulletManager>();

                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet2").transform.Find("Bullet2_3"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 7;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 0;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 5.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f - (30.0f * (i + 1)));
                }
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    bulletManager.bulletPool.Enqueue(bullet);
                }

                // 탄막 2 발사 (빨간색 환탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet1").transform.Find("Bullet1_2").GetComponent<BulletManager>();

                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet1").transform.Find("Bullet1_2"));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 4;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.5f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f - (30.0f * (i + 1)) + (Random.Range(-30.0f, 30.0f)));
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet7").transform.Find("Bullet7_2").GetComponent<BulletManager>();

                for (int j = 0; j < 16; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet7").transform.Find("Bullet7_2"));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 9;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f - (30.0f * (i + 1)) + (Random.Range(-45.0f, 45.0f)));
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }

                yield return new WaitForSeconds(0.07f);
            }

            yield return new WaitForSeconds(1.0f);

            // 플레이어 조준 발사 3회
            for (int i = 0; i < 3; i++)
            {
                // 탄막 1 발사 (빨간색 대옥탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet5").transform.Find("Bullet5_1").GetComponent<BulletManager>();

                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet2").transform.Find("Bullet2_3"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 7;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 0;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 5.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
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

                // 탄막 2 발사 (빨간색 환탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet1").transform.Find("Bullet1_2").GetComponent<BulletManager>();

                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet1").transform.Find("Bullet1_2"));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 4;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.5f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (Random.Range(-15.0f, 15.0f)));
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet7").transform.Find("Bullet7_2").GetComponent<BulletManager>();

                for (int j = 0; j < 12; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet7").transform.Find("Bullet7_2"));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 9;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (Random.Range(-30.0f, 30.0f)));
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }

                yield return new WaitForSeconds(0.06f);
            }

            // 랜덤한 지점으로 이동
            targetPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);
            StartCoroutine(MoveToDestination(targetPosition, 1.0f));
            if (targetPosition.x <= transform.position.x)
            {
                transform.Find("Body").GetComponent<SpriteRenderer>().flipX = true;
                transform.Find("Body").GetComponent<EnemySprite>().isLeftMove = true;
            }
            else
            {
                transform.Find("Body").GetComponent<EnemySprite>().isRightMove = true;
            }

            playerPosition = GameObject.Find("PLAYER").transform.position;

            // 반시계 방향 발사 11회
            for (int i = 0; i < 11; i++)
            {
                // 탄막 1 발사 (빨간색 대옥탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet5").transform.Find("Bullet5_1").GetComponent<BulletManager>();

                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet2").transform.Find("Bullet2_3"));
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 7;
                    bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 0;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 5.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (30.0f * (i + 1)));
                }
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    bulletManager.bulletPool.Enqueue(bullet);
                }

                // 탄막 2 발사 (빨간색 환탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet1").transform.Find("Bullet1_2").GetComponent<BulletManager>();

                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet1").transform.Find("Bullet1_2"));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 4;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.5f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (30.0f * (i + 1)) + (Random.Range(-30.0f, 30.0f)));
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                bulletManager = GameObject.Find("BulletManager").transform.Find("Bullet7").transform.Find("Bullet7_2").GetComponent<BulletManager>();

                for (int j = 0; j < 24; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("Bullet7").transform.Find("Bullet7_2"));
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<InitializeBullet>().bulletPoolIndex = 9;
                        bullet.GetComponent<InitializeBullet>().bulletPoolChildIndex = 1;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (30.0f * (i + 1)) + (Random.Range(-45.0f, 45.0f)));
                    }
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        bulletManager.bulletPool.Enqueue(bullet);
                    }
                }

                yield return new WaitForSeconds(0.06f);
            }

            yield return new WaitForSeconds(2.0f);
        }
    }
    #endregion

    #region 패턴 8 (동방홍마향 1스테이지 - 루미아 중간보스 1스펠 "문라이트 레이")
    
    #endregion

    #region 기타 함수

    #region 지정 위치로 적 이동 함수
    public IEnumerator MoveToDestination(Vector3 targetPosition, float delayLimit)
    {
        Vector3 vector = Vector3.zero;
        float delay = 0.0f;
        transform.Find("Body").GetComponent<EnemySprite>().spriteIndexNumber = 0;

        while (true)
        {
            delay += Time.deltaTime;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vector, 0.5f);

            if (delay >= delayLimit)
            {
                transform.Find("Body").GetComponent<EnemySprite>().isSpriteReturn = true;
                StopCoroutine(MoveToDestination(targetPosition, delayLimit));
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #endregion
}