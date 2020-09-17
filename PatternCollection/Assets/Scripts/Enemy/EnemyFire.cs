using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#region 탄막 구현에 대한 설정

// 1. BULLET 프리팹 하나만 두고, BulletManager로 1만개 정도만 우선 생성
// 2. 패턴 시작할 때마다 메모리 풀에서 꺼내 쓰는것은 같지만, 기존 스크립트 외에 추가로 이하의 컴포넌트들을 붙임
//  * 자체 이미지 회전이 필요하지 않은 탄막은 1개만 꺼낸 후 Sprite Renderer, *** Collider 2D,
//  * 자체 이미지 회전이 필요한 탄막은 본체와 차일드 각각 1개씩 꺼낸 후 본체에 *** Collider 2D, 차일드에 Sprite Renderer
// 3. Sprite Renderer는 각각 사용할 탄막 이미지를 Sprite 패널에 넣고, Order in Layer를 3으로 설정
// 4. 자체 이미지 회전하는 탄막은 차일드에 Object Rotate 스크립트를 컴포넌트로 넣고, Rotate Speed를 설정(기본 120.0f)
// 5. *** Collider 2D는 Is Trigger를 체크하고, Size를 아래와 같이 설정
//  * (Index 0) 빈 탄막
//  * (Index 1 ~ 16)    콩알탄                : Circle Collider 2D, Radius 0.02
//  * (Index 17 ~ 32)   기본 원탄             : Circle Collider 2D, Radius 0.04
//  * (Index 33 ~ 48)   테두리 원탄           : Circle Collider 2D, Radius 0.03
//  * (Index 49 ~ 64)   쌀탄                  : Capsule Collider 2D, Size X 0.025, Y 0.05, Direction Vertical
//  * (Index 65 ~ 80)   보석탄                : Capsule Collider 2D, Size X 0.02, Y 0.05, Direction Vertical
//  * (Index 81 ~ 96)   쿠나이탄              : Capsule Collider 2D, Size X 0.02, Y 0.055, Direction Vertical
//  * (Index 97 ~ 112)  쐐기탄                : Capsule Collider 2D, Size X 0.04, Y 0.06, Direction Vertical
//  * (Index 113 ~ 128) 부적탄                : Box Collider 2D, Size X 0.025, Y 0.05
//  * (Index 129 ~ 144) 총알탄                : Box Collider 2D, Size X 0.02, Y 0.06
//  * (Index 145 ~ 160) 감주탄                : Capsule Collider 2D, Size X 0.02, Y 0.07, Direction Vertical
//  * (Index 161 ~ 176) 소형 별탄             : Circle Collider 2D, Radius 0.015, Offset X 0, Y -0.005
//  * (Index 177 ~ 192) 옥구슬탄              : Circle Collider 2D, Radius 0.04
//  * (Index 193 ~ 208) 소형 테두리 원탄       : Circle Collider 2D, Radius 0.015
//  * (Index 209 ~ 224) 소형 쌀탄             : Circle Collider 2D, Radius 0.015
//  * (Index 225 ~ 240) 소형 촉탄             : Circle Collider 2D, Radius 0.02, Offset X 0, Y 0.01
//  * (Index 241 ~ 244) 대옥탄                : Circle Collider 2D, Radius 0.135
//  * (Index 245 ~ 247) 엽전탄                : Circle Collider 2D, Radius 0.04
//  * (Index 258 ~ 265) 대형 환탄             : Circle Collider 2D, Radius 0.08
//  * (Index 266 ~ 273) 나비탄                : Circle Collider 2D, Radius 0.02
//  * (Index 274 ~ 281) 나이프탄              : Capsule Collider 2D, Size X 0.035, Y 0.2, Direction Vertical
//  * (Index 282 ~ 289) 알약탄                : Capsule Collider 2D, Size X 0.06, Y 0.18, Direction Vertical
//  * (Index 290 ~ 297) 대형 별탄             : Circle Collider 2D, Radius 0.045, Offset X 0, Y -0.01
//  * (Index 298 ~ 305) 탄막 발사 이펙트
//  * (Index 306 ~ 313) 대형 발광탄           : Circle Collider 2D, Radius 0.1
//  * (Index 314 ~ 321) 하트탄                : Circle Collider 2D, Radius 0.065, Offset X 0, Y -0.01
//  * (Index 322 ~ 329) 탄막 발사 이펙트
//  * (Index 330 ~ 337) 화살탄                : Capsule Collider 2D, Size X 0.01, Y 0.2, Direction Vertical
//  * (Index 338 ~ 349) 음표탄                : Circle Collider 2D, Radius 0.02, Offset X -0.01, Y -0.09, Sprite Animation 적용
//  * (Index 350 ~ 357) 쉼표탄                : Capsule Collider 2D, Size X 0.02, Y 0.18
//  * (Index 358 ~ 373) 고정 레이저탄          : Box Collider 2D, Size X 0.09, Y 0.13
//  * (Index 374 ~ 389) 무빙 레이저탄 (머리 1) : Box Collider 2D, Size X 0.52, Y 0.04, Offset X 0.02, Y 0
//  * (Index 390 ~ 405) 무빙 레이저탄 (머리 2) : Box Collider 2D, Size X 0.56, Y 0.04
//  * (Index 406 ~ 421) 무빙 레이저탄 (몸통)   : Box Collider 2D, Size X 0.24, Y 0.04
//  * (Index 422 ~ 437) 무빙 레이저탄 (꼬리 1) : Box Collider 2D, Size X 0.56, Y 0.04
//  * (Index 438 ~ 453) 무빙 레이저탄 (꼬리 2) : Box Collider 2D, Size X 0.52, Y 0.04, Offset X -0.02, Y 0

#endregion

public class EnemyFire : MonoBehaviour
{
    private BulletManager bulletManager;
    private BulletManager effectManager;
    private EnemyDatabase enemyDatabase;
    private GameObject playerObject;
    private Transform body;

    public Sprite[] spriteCollection;
    public Transform enemyBulletTemp1;
    public Transform enemyBulletTemp2;
    public Transform enemyBulletTemp3;

    void Start()
    {
        bulletManager = GameObject.Find("BulletManager").transform.GetChild(0).GetComponent<BulletManager>();
        effectManager = GameObject.Find("EffectManager").GetComponent<BulletManager>();
        enemyDatabase = GetComponent<EnemyDatabase>();
        playerObject = GameObject.Find("PLAYER");
        body = transform.Find("Body");
    }

    // 적 패턴 변경
    public void Fire(int stageNumber)
    {
        StartCoroutine("Pattern" + stageNumber.ToString());
    }

    #region 패턴 모음집

    #region 패턴 1 (동방홍마향 1스테이지 - 루미아 중간보스 통상)

    public IEnumerator Pattern1()
    {
        StartCoroutine(Pattern1_1());

        yield return null;
    }
    public IEnumerator Pattern1_1()
    {
        while (true)
        {
            StartCoroutine(EnemyMove(new Vector3(2.5f, 2.0f, 0.0f), 1.0f));

            yield return new WaitForSeconds(1.25f);

            for (int i = 0; i < 18; i++)
            {
                StartCoroutine(Pattern1_1Attack1(20.0f * i));
            }

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(EnemyMove(new Vector3(0.0f, 3.5f, 0.0f), 1.0f));

            yield return new WaitForSeconds(1.25f);

            Vector2 playerPosition = playerObject.transform.position;
            for (int i = 0; i < 10; i++)
            {
                StartCoroutine(Pattern1_1Attack2(i % 5, playerPosition, -2.0f * i));

                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(0.75f);

            StartCoroutine(EnemyMove(new Vector3(-2.5f, 2.5f, 0.0f), 1.0f));

            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    StartCoroutine(Pattern1_1Attack3(i, 22.5f * j));
                }

                yield return new WaitForSeconds(0.4f);
            }

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(EnemyMove(new Vector3(0.0f, 3.0f, 0.0f), 1.0f));

            yield return new WaitForSeconds(1.25f);

            for (int i = 0; i < 32; i++)
            {
                StartCoroutine(Pattern1_1Attack4(Random.Range(50, 64), Random.Range(8.0f, 12.0f)));

                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
    public IEnumerator Pattern1_1Attack1(float addRotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.25f, 0.5f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.5f);

        // 탄막 1 발사
        for (int i = 0; i < 8; i++)
        {
            StartCoroutine
                (BulletFire(bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.03f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 39, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 10.0f,
                0.0f, 0.0f,
                1.0f, 4.0f - (0.125f * i), false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f, 
                3, playerObject.transform.position, (3.0f * i) + addRotateAngle));
        }
    }
    public IEnumerator Pattern1_1Attack2(int spriteNumber, Vector2 playerPosition, float addRotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(304, 0.4f, 0.3f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.3f);

        // 탄막 2 발사
        for (int i = 0; i < 18; i++)
        {
            StartCoroutine
                (BulletFire(bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp2, 1, 0.02f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 10 + spriteNumber, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 8.0f,
                0.0f, 0.0f,
                0.8f, 3.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, playerPosition, (20.0f * i) + addRotateAngle));
        }
    }
    public IEnumerator Pattern1_1Attack3(int spriteNumber, float addRotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 3 이펙트
        StartCoroutine(CreateBulletFireEffect(304 - spriteNumber, 0.4f, 0.3f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.3f);

        // 탄막 3 발사
        for (int i = 0; i < 8; i++)
        {
            StartCoroutine
                (BulletFire(bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.03f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, spriteNumber.Equals(0) ? 46 : 43, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 12.0f,
                0.0f, 0.0f,
                1.0f, 4.5f - (0.25f * i), false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, playerObject.transform.position, addRotateAngle));
        }
    }
    public IEnumerator Pattern1_1Attack4(int spriteNumber, float bulletSpeed)
    {
        Vector2 bulletFirePosition = transform.position;
        int effectSpriteNumber = 0;

        // 탄막 4 이펙트
        if (spriteNumber >= 50 && spriteNumber <= 51)
        {
            effectSpriteNumber = 299;
        }
        if (spriteNumber >= 52 && spriteNumber <= 53)
        {
            effectSpriteNumber = 300;
        }
        else if (spriteNumber >= 54 && spriteNumber <= 55)
        {
            effectSpriteNumber = 301;
        }
        else if (spriteNumber >= 56 && spriteNumber <= 57)
        {
            effectSpriteNumber = 302;
        }
        else if (spriteNumber >= 58 && spriteNumber <= 60)
        {
            effectSpriteNumber = 303;
        }
        else if (spriteNumber >= 61 && spriteNumber <= 63)
        {
            effectSpriteNumber = 304;
        }
        StartCoroutine(CreateBulletFireEffect(effectSpriteNumber, 0.5f, 0.2f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 4; i++)
        {
            // 탄막 4 발사
            StartCoroutine
                (BulletFire(bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp3, 2, 0.00f,
                0.025f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, spriteNumber, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, bulletSpeed,
                0.0f, 0.0f,
                bulletSpeed * 0.1f, bulletSpeed * 0.3f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                2, playerObject.transform.position, 90.0f * i));
        }
    }

    #endregion

    #region 패턴 2 (동방홍마향 1스테이지 - 루미아 중간보스 스펠 "문라이트 레이")

    public IEnumerator Pattern2()
    {
        StartCoroutine(Pattern2_1());
        StartCoroutine(Pattern2_2());

        yield return null;
    }
    public IEnumerator Pattern2_1()
    {
        while (true)
        {
            StartCoroutine(Pattern2_1Attack());

            yield return new WaitForSeconds(3.0f);

            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 2.0f, 4.25f, 1.0f));

            yield return new WaitForSeconds(1.25f);
        }
    }
    public IEnumerator Pattern2_1Attack()
    {
        for (int i = 0; i < 2; i++)
        {
            // 빈 탄막 발사
            if (bulletManager.bulletPool.Count <= 0) AddBulletPool();
            GameObject emptyBullet = bulletManager.bulletPool.Dequeue();
            if (bulletManager.bulletPool.Count > 0)
            {
                emptyBullet.SetActive(true);
                ClearChild(emptyBullet);
                emptyBullet.transform.position = transform.position;
                emptyBullet.gameObject.tag = "BULLET_ENEMY_EMPTY";
                emptyBullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_LASER");
                emptyBullet.transform.SetParent(enemyBulletTemp1);
                if (!emptyBullet.GetComponent<SpriteRenderer>()) emptyBullet.AddComponent<SpriteRenderer>();
                if (!emptyBullet.GetComponent<CapsuleCollider2D>()) emptyBullet.AddComponent<CapsuleCollider2D>();
                if (!emptyBullet.GetComponent<InitializeBullet>()) emptyBullet.AddComponent<InitializeBullet>();
                if (!emptyBullet.GetComponent<MovingBullet>()) emptyBullet.AddComponent<MovingBullet>();
                if (!emptyBullet.GetComponent<LaserBullet>()) emptyBullet.AddComponent<LaserBullet>();
                if (!emptyBullet.GetComponent<EraseBullet>()) emptyBullet.AddComponent<EraseBullet>();
                SpriteRenderer spriteRenderer = emptyBullet.GetComponent<SpriteRenderer>();
                CapsuleCollider2D capsuleCollider2D = emptyBullet.GetComponent<CapsuleCollider2D>();
                InitializeBullet initializeBullet = emptyBullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = emptyBullet.GetComponent<MovingBullet>();
                LaserBullet laserBullet = emptyBullet.GetComponent<LaserBullet>();
                spriteRenderer.sprite = spriteCollection[0];
                spriteRenderer.sortingOrder = 3;
                capsuleCollider2D.isTrigger = true;
                capsuleCollider2D.size = new Vector2(0.05f, 0.05f);
                initializeBullet.bulletType = BulletType.BULLETTYPE_LASER_HOLD;
                initializeBullet.bulletObject = emptyBullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 0.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                laserBullet.laserWidth = 1.8f;
                laserBullet.laserEnableTime = 0.0f;
                laserBullet.laserEnableSpeed = 0.1f;
                laserBullet.laserDisableTime = 1.5f;
                laserBullet.laserDisableSpeed = 0.1f;
                laserBullet.laserRotateState = BulletRotateState.BULLETROTATESTATE_NORMAL;
                laserBullet.laserRotateSpeed = (i == 0 ? 30.0f : -30.0f);
                laserBullet.isLaserRotateEnabled = true;
                laserBullet.isLaserRotateDisabled = true;
            }
            else AddBulletPool();

            // 레이저 탄막 발사
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = new Vector3(transform.position.x, transform.position.y - 7.0f, transform.position.z);
                bullet.transform.localScale = new Vector3(1.8f, 100.0f, bullet.transform.localScale.z);
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_LASER");
                bullet.transform.SetParent(enemyBulletTemp2);
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                if (!bullet.GetComponent<LaserBullet>()) bullet.AddComponent<LaserBullet>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                InitializeBullet emptyInitializeBullet = emptyBullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                MovingBullet emptyMovingBullet = emptyBullet.GetComponent<MovingBullet>();
                LaserBullet laserBullet = bullet.GetComponent<LaserBullet>();
                spriteRenderer.sprite = spriteCollection[49];
                spriteRenderer.sortingOrder = 3;
                capsuleCollider2D.isTrigger = true;
                capsuleCollider2D.size = new Vector2(0.08f, 0.14f);
                initializeBullet.bulletType = BulletType.BULLETTYPE_LASER_HOLD;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 0.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                laserBullet.laserWidth = 1.8f;
                laserBullet.laserEnableTime = 0.0f;
                laserBullet.laserEnableSpeed = 0.1f;
                laserBullet.laserDisableTime = 1.5f;
                laserBullet.laserDisableSpeed = 0.1f;
                bullet.transform.SetParent(emptyBullet.transform);
                emptyMovingBullet.bulletDestination = emptyInitializeBullet.GetAimedBulletDestination(new Vector2(transform.position.x, transform.position.y - 7.0f));
                float angle = Mathf.Atan2(emptyMovingBullet.bulletDestination.y, emptyMovingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                emptyMovingBullet.ChangeRotateAngle(angle + 90.0f + (i == 0 ? -55.0f : 55.0f));
            }
            else AddBulletPool();
        }

        yield return null;
    }
    public IEnumerator Pattern2_2()
    {
        while (true)
        {
            StartCoroutine(Pattern2_2Attack());

            yield return new WaitForSeconds(0.4f);
        }
    }
    public IEnumerator Pattern2_2Attack()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(305, 0.5f, 0.25f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.25f);

        // 탄막 1 발사
        for (int i = 0; i < 64; i++)
        {
            StartCoroutine
                (BulletFire(bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                0.02f, 0.07f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 145, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 4.5f,
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, (5.625f * i)));
        }
    }

    #endregion

    #region 패턴 3 (동방홍마향 1스테이지 - 루미아 1통상)

    public IEnumerator Pattern3()
    {
        StartCoroutine(Pattern3_1());

        yield return null;
    }
    public IEnumerator Pattern3_1()
    {
        int fireCount = 0;
        int bulletCount = 8;

        while (true)
        {
            StartCoroutine(Pattern3_1Attack(fireCount, bulletCount));
            fireCount++;
            bulletCount += 8;

            if (fireCount.Equals(8))
            {
                fireCount = 0;
                bulletCount = 0;
                StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.5f));

                yield return new WaitForSeconds(1.75f);
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    public IEnumerator Pattern3_1Attack(int fireCount, int bulletCount)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(299, 0.8f, 0.1f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.1f);

        // 탄막 1 발사 (빨강색, 진홍색 원탄) (조준탄)
        for (int i = 0; i < bulletCount; i++)
        {
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = transform.position;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                bullet.transform.SetParent(enemyBulletTemp1);
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                spriteRenderer.sprite = spriteCollection[18 + ((i / 8) % 2)];
                spriteRenderer.sortingOrder = 3;
                circleCollider2D.isTrigger = true;
                circleCollider2D.radius = 0.04f;
                circleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 2.0f + (0.8f * (i % 8)) + (0.2f * fireCount);
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f + (10.0f * (i / 8)) - (5.0f * ((bulletCount - 1) / 8)));
            }
            else AddBulletPool();
        }

        yield return null;
    }

    #endregion

    #region 패턴 4 (동방홍마향 1스테이지 - 루미아 1스펠 "나이트 버드" 리메이크)

    public IEnumerator Pattern4()
    {
        StartCoroutine(Pattern4_1());

        yield return null;
    }
    public IEnumerator Pattern4_1()
    {
        Vector2 targetPosition;
        int fireCount = 0;
        float rotateAngle = 0.0f;
        int rotateDirection = 1;

        while (true)
        {
            targetPosition = playerObject.transform.position;
            if (fireCount.Equals(0))
            {
                StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.5f));
            }

            for (int i = 0; i < 16; i++)
            {
                StartCoroutine(Pattern4_1Attack(targetPosition, rotateAngle, rotateDirection));
                rotateAngle += 7.5f;

                yield return new WaitForSeconds(0.03f);
            }

            fireCount++;
            rotateAngle = 0.0f;
            rotateDirection *= -1;

            if (fireCount >= 8)
            {
                fireCount = 0;
                rotateDirection = 1;
                yield return new WaitForSeconds(2.0f);
            }
        }
    }
    public IEnumerator Pattern4_1Attack(Vector2 targetPosition, float rotateAngle, int rotateDirection)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(rotateDirection.Equals(1) ? 301 : 302, 0.6f, 0.1f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < 5; i++)
        {
            // 탄막 1 발사 (파란색 테두리원탄) (조준 방사탄)
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = transform.position;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                bullet.transform.SetParent(enemyBulletTemp1);
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                spriteRenderer.sprite = spriteCollection[rotateDirection.Equals(1) ? 39 : 41];
                spriteRenderer.sortingOrder = 3;
                circleCollider2D.isTrigger = true;
                circleCollider2D.radius = 0.03f;
                circleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 2.0f + (0.25f * i);
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(targetPosition);
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f + (rotateDirection.Equals(1) ? -30.0f + rotateAngle : 30.0f - rotateAngle) +
                    (rotateDirection.Equals(1) ? (-1.0f * i) : (1.0f * i)));
            }
            else AddBulletPool();
        }

        yield return null;
    }

    #endregion

    #region 패턴 5 (동방홍마향 1스테이지 - 루미아 2통상)



    #endregion

    #region 패턴 6 (동방홍마향 1스테이지 - 루미아 2스펠 "디마케이션")

    public IEnumerator Pattern6()
    {
        StartCoroutine(Pattern6_1());

        yield return null;
    }
    public IEnumerator Pattern6_1()
    {
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                // StartCoroutine(Pattern6_1Attack());

                yield return new WaitForSeconds(1.0f);
            }

            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.5f));

            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(Pattern6_2());

                yield return new WaitForSeconds(0.75f);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
    public IEnumerator Pattern6_2()
    {
        for (int i = 0; i < 30; i++)
        {
            // StartCoroutine(Pattern6_2Attack());

            yield return new WaitForSeconds(0.015f);
        }
    }

    #endregion

    #endregion

    #region 임시 패턴 보관함

    #region 패턴 1 (동방지령전 4스테이지 - 코메이지 사토리 마리사A 4스펠 "리턴 인애니메이트니스" 열화판)

    // public IEnumerator Stage1()
    // {
    //     StartCoroutine(Stage1Pattern1());
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage1Pattern1()
    // {
    //     int fireCount = 0;
    // 
    //     while (true)
    //     {
    //         fireCount++;
    //         StartCoroutine(Stage1Pattern1Attack(fireCount));
    // 
    //         if (fireCount.Equals(8))
    //         {
    //             yield return new WaitForSeconds(0.5f);
    //         }
    //         else if (fireCount.Equals(16))
    //         {
    //             fireCount = 0;
    //             StartCoroutine(Stage1EnemyMove());
    // 
    //             yield return new WaitForSeconds(1.5f);
    //         }
    //         else
    //         {
    //             yield return new WaitForSeconds(0.05f);
    //         }
    //     }
    // }
    // public IEnumerator Stage1EnemyMove()
    // {
    //     StartCoroutine(EnemyRandomMove(-1.0f, 1.0f, 1.5f, 3.5f, 1.0f));
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage1Pattern1Attack(int fireCount)
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 1 이펙트
    //     StartCoroutine(CreateBulletFireEffect(301, 0.8f, 0.1f, 0.4f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.1f);
    // 
    //     // 탄막 1 발사 (파란색 환탄) (조준 방사탄)
    //     if (bulletManager.bulletPool.Count > 0)
    //     {
    //         GameObject bullet = bulletManager.bulletPool.Dequeue();
    //         bullet.SetActive(true);
    //         ClearChild(bullet);
    //         bullet.transform.position = transform.position;
    //         bullet.gameObject.tag = "BULLET_ENEMY";
    //         bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
    //         bullet.transform.SetParent(enemyBulletTemp1);
    //         if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //         if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //         if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //         if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //         if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //         if (!bullet.GetComponent<Stage1BulletFragmentation>()) bullet.AddComponent<Stage1BulletFragmentation>();
    //         SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //         CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //         InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //         MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //         spriteRenderer.sprite = spriteCollection[261];
    //         spriteRenderer.sortingOrder = 3;
    //         circleCollider2D.isTrigger = true;
    //         circleCollider2D.radius = 0.08f;
    //         circleCollider2D.enabled = false;
    //         initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //         initializeBullet.bulletObject = bullet.gameObject;
    //         initializeBullet.targetObject = playerObject;
    //         initializeBullet.isGrazed = false;
    //         if (fireCount <= 8)
    //         {
    //             movingBullet.bulletMoveSpeed = 4.0f;
    //             movingBullet.bulletDecelerationMoveSpeed = 0.1f;
    //         }
    //         else
    //         {
    //             movingBullet.bulletMoveSpeed = 8.0f;
    //             movingBullet.bulletDecelerationMoveSpeed = 0.2f;
    //         }
    //         movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
    //         movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //         movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
    //         float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //         movingBullet.ChangeRotateAngle(angle - 90.0f + (45.0f * ((fireCount - 1) % 8)));
    //     }
    //     else AddBulletPool();
    // }

    #endregion

    #region 패턴 2 (자작)

    // public IEnumerator Stage2()
    // {
    //     StartCoroutine(Stage2Pattern1());
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage2Pattern1()
    // {
    //     while (true)
    //     {
    //         for (int i = 0; i < 8; i++)
    //         {
    //             StartCoroutine(Stage2Pattern1Attack1());
    //             StartCoroutine(Stage2Pattern1Attack2());
    //             StartCoroutine(Stage2Pattern1Attack3());
    //             StartCoroutine(Stage2Pattern1Attack4());
    //         }
    // 
    //         yield return new WaitForSeconds(1.0f);
    //     }
    // }
    // public IEnumerator Stage2Pattern1Attack1()
    // {
    //     Vector2 bulletFirePosition = new Vector2(Random.Range(-6.0f, -5.0f), Random.Range(-4.5f, 1.5f));
    // 
    //     // 탄막 1 발사
    //     if (bulletManager.bulletPool.Count > 0)
    //     {
    //         GameObject bullet = bulletManager.bulletPool.Dequeue();
    //         bullet.SetActive(true);
    //         ClearChild(bullet);
    //         bullet.transform.position = bulletFirePosition;
    //         bullet.gameObject.tag = "BULLET_ENEMY";
    //         bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
    //         bullet.transform.SetParent(enemyBulletTemp1);
    //         if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //         if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //         if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //         if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //         if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //         SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //         CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //         InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //         MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //         spriteRenderer.sprite = spriteCollection[268];
    //         spriteRenderer.sortingOrder = 3;
    //         circleCollider2D.isTrigger = true;
    //         circleCollider2D.radius = 0.02f;
    //         circleCollider2D.enabled = false;
    //         initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //         initializeBullet.bulletObject = bullet.gameObject;
    //         initializeBullet.targetObject = playerObject;
    //         initializeBullet.isGrazed = false;
    //         movingBullet.bulletMoveSpeed = 6.0f;
    //         movingBullet.bulletDecelerationMoveSpeed = 0.1f;
    //         movingBullet.bulletDecelerationMoveSpeedMin = 2.5f;
    //         movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
    //         movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //         movingBullet.bulletDestination = initializeBullet.GetBulletDestination
    //             (new Vector2(bullet.transform.position.x + Random.Range(0.5f, 3.0f), bullet.transform.position.y - 1.0f));
    //         float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //         movingBullet.ChangeRotateAngle(angle - 90.0f);
    //     }
    //     else AddBulletPool();
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage2Pattern1Attack2()
    // {
    //     Vector2 bulletFirePosition = new Vector2(Random.Range(-6.0f, -5.0f), Random.Range(1.5f, 7.5f));
    // 
    //     // 탄막 1 발사
    //     if (bulletManager.bulletPool.Count > 0)
    //     {
    //         GameObject bullet = bulletManager.bulletPool.Dequeue();
    //         bullet.SetActive(true);
    //         ClearChild(bullet);
    //         bullet.transform.position = bulletFirePosition;
    //         bullet.gameObject.tag = "BULLET_ENEMY";
    //         bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
    //         bullet.transform.SetParent(enemyBulletTemp1);
    //         if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //         if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //         if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //         if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //         if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //         SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //         CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //         InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //         MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //         spriteRenderer.sprite = spriteCollection[268];
    //         spriteRenderer.sortingOrder = 3;
    //         circleCollider2D.isTrigger = true;
    //         circleCollider2D.radius = 0.02f;
    //         circleCollider2D.enabled = false;
    //         initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //         initializeBullet.bulletObject = bullet.gameObject;
    //         initializeBullet.targetObject = playerObject;
    //         initializeBullet.isGrazed = false;
    //         movingBullet.bulletMoveSpeed = 6.0f;
    //         movingBullet.bulletDecelerationMoveSpeed = 0.1f;
    //         movingBullet.bulletDecelerationMoveSpeedMin = 2.5f;
    //         movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
    //         movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //         movingBullet.bulletDestination = initializeBullet.GetBulletDestination
    //             (new Vector2(bullet.transform.position.x + Random.Range(0.5f, 3.0f), bullet.transform.position.y - 1.0f));
    //         float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //         movingBullet.ChangeRotateAngle(angle - 90.0f);
    //     }
    //     else AddBulletPool();
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage2Pattern1Attack3()
    // {
    //     Vector2 bulletFirePosition = new Vector2(Random.Range(5.0f, 6.0f), Random.Range(-4.5f, 1.5f));
    // 
    //     // 탄막 1 발사
    //     if (bulletManager.bulletPool.Count > 0)
    //     {
    //         GameObject bullet = bulletManager.bulletPool.Dequeue();
    //         bullet.SetActive(true);
    //         ClearChild(bullet);
    //         bullet.transform.position = bulletFirePosition;
    //         bullet.gameObject.tag = "BULLET_ENEMY";
    //         bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
    //         bullet.transform.SetParent(enemyBulletTemp1);
    //         if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //         if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //         if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //         if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //         if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //         SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //         CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //         InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //         MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //         spriteRenderer.sprite = spriteCollection[268];
    //         spriteRenderer.sortingOrder = 3;
    //         circleCollider2D.isTrigger = true;
    //         circleCollider2D.radius = 0.02f;
    //         circleCollider2D.enabled = false;
    //         initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //         initializeBullet.bulletObject = bullet.gameObject;
    //         initializeBullet.targetObject = playerObject;
    //         initializeBullet.isGrazed = false;
    //         movingBullet.bulletMoveSpeed = 6.0f;
    //         movingBullet.bulletDecelerationMoveSpeed = 0.1f;
    //         movingBullet.bulletDecelerationMoveSpeedMin = 2.5f;
    //         movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
    //         movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //         movingBullet.bulletDestination = initializeBullet.GetBulletDestination
    //             (new Vector2(bullet.transform.position.x - Random.Range(0.5f, 3.0f), bullet.transform.position.y - 1.0f));
    //         float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //         movingBullet.ChangeRotateAngle(angle - 90.0f);
    //     }
    //     else AddBulletPool();
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage2Pattern1Attack4()
    // {
    //     Vector2 bulletFirePosition = new Vector2(Random.Range(5.0f, 6.0f), Random.Range(1.5f, 7.5f));
    // 
    //     // 탄막 1 발사
    //     if (bulletManager.bulletPool.Count > 0)
    //     {
    //         GameObject bullet = bulletManager.bulletPool.Dequeue();
    //         bullet.SetActive(true);
    //         ClearChild(bullet);
    //         bullet.transform.position = bulletFirePosition;
    //         bullet.gameObject.tag = "BULLET_ENEMY";
    //         bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
    //         bullet.transform.SetParent(enemyBulletTemp1);
    //         if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //         if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //         if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //         if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //         if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //         SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //         CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //         InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //         MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //         spriteRenderer.sprite = spriteCollection[268];
    //         spriteRenderer.sortingOrder = 3;
    //         circleCollider2D.isTrigger = true;
    //         circleCollider2D.radius = 0.02f;
    //         circleCollider2D.enabled = false;
    //         initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //         initializeBullet.bulletObject = bullet.gameObject;
    //         initializeBullet.targetObject = playerObject;
    //         initializeBullet.isGrazed = false;
    //         movingBullet.bulletMoveSpeed = 6.0f;
    //         movingBullet.bulletDecelerationMoveSpeed = 0.1f;
    //         movingBullet.bulletDecelerationMoveSpeedMin = 2.5f;
    //         movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
    //         movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //         movingBullet.bulletDestination = initializeBullet.GetBulletDestination
    //             (new Vector2(bullet.transform.position.x - Random.Range(0.5f, 3.0f), bullet.transform.position.y - 1.0f));
    //         float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //         movingBullet.ChangeRotateAngle(angle - 90.0f);
    //     }
    //     else AddBulletPool();
    // 
    //     yield return null;
    // }

    #endregion

    #region 패턴 3 (자작)

    // public IEnumerator Stage3()
    // {
    //     StartCoroutine(Stage3Pattern1());
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage3Pattern1()
    // {
    //     float rotateAngle = 0.0f;
    // 
    //     while (true)
    //     {
    //         for (int i = 0; i < 5; i++)
    //         {
    //             StartCoroutine(Stage3Pattern1Attack(rotateAngle));
    //             rotateAngle += 22.5f;
    //         }
    // 
    //         rotateAngle = 0.0f;
    // 
    //         yield return new WaitForSeconds(1.0f);
    //     }
    // }
    // public IEnumerator Stage3Pattern1Attack(float rotateAngle)
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 1 이펙트
    //     StartCoroutine(CreateBulletFireEffect(301, 0.4f, 0.2f, 0.4f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.2f);
    // 
    //     // 빈 탄막 1 발사
    //     if (bulletManager.bulletPool.Count <= 0) AddBulletPool();
    //     GameObject emptyBullet = bulletManager.bulletPool.Dequeue();
    //     if (bulletManager.bulletPool.Count > 0)
    //     {
    //         emptyBullet.SetActive(true);
    //         ClearChild(emptyBullet);
    //         emptyBullet.transform.position = transform.position;
    //         emptyBullet.gameObject.tag = "BULLET_ENEMY_EMPTY";
    //         emptyBullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
    //         emptyBullet.transform.SetParent(enemyBulletTemp1);
    //         if (!emptyBullet.GetComponent<SpriteRenderer>()) emptyBullet.AddComponent<SpriteRenderer>();
    //         if (!emptyBullet.GetComponent<CircleCollider2D>()) emptyBullet.AddComponent<CircleCollider2D>();
    //         if (!emptyBullet.GetComponent<InitializeBullet>()) emptyBullet.AddComponent<InitializeBullet>();
    //         if (!emptyBullet.GetComponent<MovingBullet>()) emptyBullet.AddComponent<MovingBullet>();
    //         if (!emptyBullet.GetComponent<EraseBullet>()) emptyBullet.AddComponent<EraseBullet>();
    //         SpriteRenderer spriteRenderer = emptyBullet.GetComponent<SpriteRenderer>();
    //         CircleCollider2D circleCollider2D = emptyBullet.GetComponent<CircleCollider2D>();
    //         InitializeBullet initializeBullet = emptyBullet.GetComponent<InitializeBullet>();
    //         MovingBullet movingBullet = emptyBullet.GetComponent<MovingBullet>();
    //         spriteRenderer.sprite = spriteCollection[0];
    //         spriteRenderer.sortingOrder = 3;
    //         circleCollider2D.isTrigger = true;
    //         circleCollider2D.radius = 0.02f;
    //         circleCollider2D.enabled = false;
    //         initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //         initializeBullet.bulletObject = emptyBullet.gameObject;
    //         initializeBullet.targetObject = playerObject;
    //         initializeBullet.isGrazed = true;
    //         movingBullet.bulletMoveSpeed = 2.0f;
    //         movingBullet.bulletRotateSpeed = 60.0f;
    //         movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //         movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //         movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
    //         float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //         movingBullet.ChangeRotateAngle(angle - 135.0f + rotateAngle);
    //     }
    //     else AddBulletPool();
    // 
    //     // 탄막 1 발사 (하늘색 나비탄) (빈 탄막을 중심으로 회전)
    //     for (int i = 0; i < 12; i++)
    //     {
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = emptyBullet.transform.position;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
    //             bullet.transform.SetParent(emptyBullet.transform);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[270];
    //             spriteRenderer.sortingOrder = 3;
    //             circleCollider2D.isTrigger = true;
    //             circleCollider2D.radius = 0.02f;
    //             circleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = emptyBullet.gameObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = 2.0f;
    //             movingBullet.bulletRotateSpeed = 180.0f;
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NORMAL;
    //             movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f + (30.0f * i));
    //         }
    //         else AddBulletPool();
    //     }
    // }

    #endregion

    #region 패턴 4 (자작)

    // public IEnumerator Stage4()
    // {
    //     StartCoroutine(Stage4Pattern1());
    //     StartCoroutine(Stage4Pattern2());
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage4Pattern1()
    // {
    //     int fireCount = 0;
    // 
    //     while (true)
    //     {
    //         fireCount++;
    //         StartCoroutine(Stage4Pattern1Attack());
    // 
    //         if (fireCount.Equals(16))
    //         {
    //             fireCount = 0;
    //             yield return new WaitForSeconds(0.75f);
    //         }
    //         else
    //         {
    //             yield return new WaitForSeconds(0.05f);
    //         }
    //     }
    // }
    // public IEnumerator Stage4Pattern2()
    // {
    //     Vector2 targetPosition = playerObject.transform.position;
    //     int angleMultiply = 1;
    //     int fireCount = 0;
    //     int bulletTextureIndex2 = Random.Range(97, 112);
    //     float rotateAngle = 0.0f;
    // 
    //     while (true)
    //     {
    //         fireCount++;
    //         StartCoroutine(Stage4Pattern2Attack(angleMultiply, rotateAngle, bulletTextureIndex2, targetPosition));
    //         rotateAngle += 1.5f;
    // 
    //         if (fireCount.Equals(16))
    //         {
    //             angleMultiply *= -1;
    //             fireCount = 0;
    //             rotateAngle = 0.0f;
    // 
    //             yield return new WaitForSeconds(1.0f);
    // 
    //             bulletTextureIndex2 = Random.Range(97, 112);
    //             targetPosition = playerObject.transform.position;
    //         }
    //         else
    //         {
    //             yield return new WaitForSeconds(0.05f);
    //         }
    //     }
    // }
    // public IEnumerator Stage4Pattern1Attack()
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 1 이펙트
    //     StartCoroutine(CreateBulletFireEffect(298, 0.8f, 0.1f, 0.4f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.1f);
    // 
    //     // 탄막 1 발사 (무작위 색상 원탄) (랜덤탄)
    //     for (int i = 0; i < 18; i++)
    //     {
    //         int bulletTextureIndex1 = Random.Range(17, 32);
    // 
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = gameObject.transform.position;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
    //             bullet.transform.SetParent(enemyBulletTemp1);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[bulletTextureIndex1];
    //             spriteRenderer.sortingOrder = 3;
    //             circleCollider2D.isTrigger = true;
    //             circleCollider2D.radius = 0.04f;
    //             circleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = Random.Range(2.0f, 8.0f);
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f);
    //         }
    //         else AddBulletPool();
    //     }
    // }
    // public IEnumerator Stage4Pattern2Attack(int angleMultiply, float rotateAngle, int bulletTextureIndex2, Vector2 targetPosition)
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 2 이펙트
    //     StartCoroutine(CreateBulletFireEffect(305, 0.8f, 0.1f, 0.4f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.1f);
    // 
    //     // 탄막 2 발사 (무작위 색상 쐐기탄) (조준 방사탄)
    //     for (int i = 0; i < 16; i++)
    //     {
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = transform.position;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
    //             bullet.transform.SetParent(enemyBulletTemp2);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[bulletTextureIndex2];
    //             spriteRenderer.sortingOrder = 3;
    //             capsuleCollider2D.isTrigger = true;
    //             capsuleCollider2D.size = new Vector2(0.04f, 0.06f);
    //             capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
    //             capsuleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = 9.0f;
    //             movingBullet.bulletDecelerationMoveSpeed = 0.3f;
    //             movingBullet.bulletDecelerationMoveSpeedMin = 3.0f;
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(targetPosition);
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle((angle - 90.0f + (22.5f * i) + rotateAngle) * angleMultiply);
    //         }
    //         else AddBulletPool();
    //     }
    // }

    #endregion

    #region 패턴 5 (동방홍마향 6스테이지 - 레밀리아 스칼렛 5스펠 "홍색의 환상향")

    // public IEnumerator Stage5()
    // {
    //     StartCoroutine(Stage5Pattern1());
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage5Pattern1()
    // {
    //     int angleMultiply = 1;
    // 
    //     while (true)
    //     {
    //         StartCoroutine(Stage5Pattern1Attack1());
    // 
    //         yield return new WaitForSeconds(3.0f);
    // 
    //         for (int i = 0; i < 3; i++)
    //         {
    //             Stage5BulletActive();
    //             StartCoroutine(Stage5Pattern1Attack2(angleMultiply));
    //             angleMultiply *= -1;
    //             if (!i.Equals(2)) StartCoroutine(Stage5EnemyMove());
    // 
    //             yield return new WaitForSeconds(3.0f);
    //         }
    // 
    //         Stage5BulletActive();
    //         StartCoroutine(Stage5EnemyMove());
    // 
    //         yield return new WaitForSeconds(1.25f);
    //     }
    // }
    // public IEnumerator Stage5EnemyMove()
    // {
    //     StartCoroutine(EnemyRandomMove(-1.25f, 1.25f, 1.0f, 3.0f, 1.0f));
    // 
    //     yield return null;
    // }
    // public void Stage5BulletActive()
    // {
    //     // 대옥탄에서 생성된 탄 활성화
    //     while (enemyBulletTemp2.childCount != 0)
    //     {
    //         GameObject bullet = enemyBulletTemp2.transform.GetChild(0).gameObject;
    //         SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //         MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    // 
    //         if (bullet.activeSelf.Equals(true))
    //         {
    //             bullet.transform.SetParent(enemyBulletTemp3);
    //             spriteRenderer.sprite = spriteCollection[19];
    //             movingBullet.bulletAccelerationMoveSpeed = 0.015f;
    //             movingBullet.bulletAccelerationMoveSpeedMax = 2.0f;
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
    //         }
    //     }
    // }
    // public IEnumerator Stage5Pattern1Attack1()
    // {
    //     // 탄막 1 발사 (빨간색 대옥탄) (조준 방사탄)
    //     for (int i = 0; i < 48; i++)
    //     {
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = transform.position;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
    //             bullet.transform.SetParent(enemyBulletTemp1);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             if (!bullet.GetComponent<Stage5BulletCreate>()) bullet.AddComponent<Stage5BulletCreate>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[241];
    //             spriteRenderer.sortingOrder = 3;
    //             spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.85f);
    //             circleCollider2D.isTrigger = true;
    //             circleCollider2D.radius = 0.135f;
    //             circleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = 3.0f + (0.5f * (i % 4));
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f + (7.5f * i));
    //         }
    //         else AddBulletPool();
    //     }
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage5Pattern1Attack2(int angleMultiply)
    // {
    //     // 탄막 1 발사 (빨간색 대옥탄) (조준 회전탄)
    //     for (int i = 0; i < 18; i++)
    //     {
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = transform.position;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
    //             bullet.transform.SetParent(enemyBulletTemp1);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             if (!bullet.GetComponent<Stage5BulletCreate>()) bullet.AddComponent<Stage5BulletCreate>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[241];
    //             spriteRenderer.sortingOrder = 3;
    //             spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.85f);
    //             circleCollider2D.isTrigger = true;
    //             circleCollider2D.radius = 0.135f;
    //             circleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = 4.0f;
    //             movingBullet.bulletRotateLimit = 1.2f;
    //             movingBullet.bulletRotateSpeed = 30.0f * angleMultiply;
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
    //             movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f + (20.0f * i));
    //         }
    //         else AddBulletPool();
    //     }
    // 
    //     yield return null;
    // }

    #endregion

    #region 패턴 7 (동방홍마향 6스테이지 - 레밀리아 스칼렛 4스펠 "스칼렛 마이스터")

    // public IEnumerator Stage7()
    // {
    //     StartCoroutine(Stage7Pattern1());
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage7Pattern1()
    // {
    //     Vector2 targetPosition = playerObject.transform.position;
    //     int fireCount = 0;
    //     float rotateAngle = 0.0f;
    //     bool isMoreBulletFire = false;
    // 
    //     while (fireCount < 18)
    //     {
    //         if (fireCount >= 6)
    //         {
    //             if (fireCount.Equals(6))
    //             {
    //                 StartCoroutine(Stage7EnemyMove(1.0f));
    //                 isMoreBulletFire = true;
    //             }
    //             rotateAngle -= 30.0f;
    //         }
    //         else
    //         {
    //             targetPosition = playerObject.transform.position;
    //         }
    // 
    //         fireCount++;
    //         StartCoroutine(Stage7PatternAttack(targetPosition, rotateAngle, isMoreBulletFire));
    // 
    //         yield return new WaitForSeconds(0.07f);
    //     }
    // 
    //     yield return new WaitForSeconds(1.0f);
    // 
    //     StartCoroutine(Stage7Pattern2());
    // }
    // public IEnumerator Stage7Pattern2()
    // {
    //     Vector2 targetPosition = playerObject.transform.position;
    //     int fireCount = 0;
    //     float rotateAngle = 0.0f;
    //     bool isMoreBulletFire = false;
    // 
    //     while (fireCount < 15)
    //     {
    //         if (fireCount >= 3)
    //         {
    //             if (fireCount.Equals(3))
    //             {
    //                 StartCoroutine(Stage7EnemyMove(1.5f));
    //                 isMoreBulletFire = true;
    //             }
    //             rotateAngle += 30.0f;
    //         }
    //         else
    //         {
    //             targetPosition = playerObject.transform.position;
    //         }
    // 
    //         fireCount++;
    //         StartCoroutine(Stage7PatternAttack(targetPosition, rotateAngle, isMoreBulletFire));
    // 
    //         yield return new WaitForSeconds(0.07f);
    //     }
    // 
    //     yield return new WaitForSeconds(1.75f);
    // 
    //     StartCoroutine(Stage7Pattern1());
    // }
    // public IEnumerator Stage7EnemyMove(float moveTime)
    // {
    //     StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, moveTime));
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage7PatternAttack(Vector2 targetPosition, float rotateAngle, bool isMoreBulletFire)
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 이펙트
    //     StartCoroutine(CreateBulletFireEffect(299, 0.6f, 0.1f, 0.4f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.1f);
    // 
    //     // 탄막 1 발사 (빨간색 대옥탄)
    //     if (bulletManager.bulletPool.Count > 0)
    //     {
    //         GameObject bullet = bulletManager.bulletPool.Dequeue();
    //         bullet.SetActive(true);
    //         ClearChild(bullet);
    //         bullet.transform.position = transform.position;
    //         bullet.gameObject.tag = "BULLET_ENEMY";
    //         bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
    //         bullet.transform.SetParent(enemyBulletTemp1);
    //         if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //         if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //         if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //         if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //         if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //         SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //         CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //         InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //         MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //         spriteRenderer.sprite = spriteCollection[241];
    //         spriteRenderer.sortingOrder = 3;
    //         spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.85f);
    //         circleCollider2D.isTrigger = true;
    //         circleCollider2D.radius = 0.135f;
    //         circleCollider2D.enabled = false;
    //         initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //         initializeBullet.bulletObject = bullet.gameObject;
    //         initializeBullet.targetObject = playerObject;
    //         initializeBullet.isGrazed = false;
    //         movingBullet.bulletMoveSpeed = 5.5f;
    //         movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //         movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //         movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(targetPosition);
    //         float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //         movingBullet.ChangeRotateAngle(angle - 90.0f + rotateAngle);
    //     }
    //     else AddBulletPool();
    // 
    //     // 탄막 2 발사 (빨간색 대형 환탄)
    //     for (int i = 0; i < (isMoreBulletFire.Equals(false) ? 6 : 8); i++)
    //     {
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = transform.position;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
    //             bullet.transform.SetParent(enemyBulletTemp2);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[259];
    //             spriteRenderer.sortingOrder = 3;
    //             circleCollider2D.isTrigger = true;
    //             circleCollider2D.radius = 0.08f;
    //             circleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = 3.5f + (Random.Range(-1.0f, 1.0f));
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(targetPosition);
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f + (Random.Range(-15.0f, 15.0f)) + rotateAngle);
    //         }
    //         else AddBulletPool();
    //     }
    // 
    //     // 탄막 3 발사 (빨간색 테두리 원탄)
    //     for (int i = 0; i < (isMoreBulletFire.Equals(false) ? 8 : 16); i++)
    //     {
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = transform.position;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
    //             bullet.transform.SetParent(enemyBulletTemp3);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[35];
    //             spriteRenderer.sortingOrder = 3;
    //             circleCollider2D.isTrigger = true;
    //             circleCollider2D.radius = 0.03f;
    //             circleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = 3.0f + (Random.Range(-1.0f, 1.0f));
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(targetPosition);
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f + (Random.Range(-30.0f, 30.0f)) + rotateAngle);
    //         }
    //         else AddBulletPool();
    //     }
    // 
    //     yield return null;
    // }

    #endregion

    #region 패턴 10 (동방홍마향 EX스테이지 - 플랑드르 스칼렛 10스펠 "495년의 파문")

    public IEnumerator Stage10()
    {
        while (true)
        {
            // 탄막 1 발사
            StartCoroutine(Stage10Pattern1Attack());

            if (enemyDatabase.GetEnemyCurrentHPRate() >= 0.6f)
            {
                yield return new WaitForSeconds(2.0f);
            }
            else if (enemyDatabase.GetEnemyCurrentHPRate() < 0.6f && enemyDatabase.GetEnemyCurrentHPRate() >= 0.4f)
            {
                yield return new WaitForSeconds(1.6f);
            }
            else if (enemyDatabase.GetEnemyCurrentHPRate() < 0.4f && enemyDatabase.GetEnemyCurrentHPRate() >= 0.2f)
            {
                yield return new WaitForSeconds(1.2f);
            }
            else
            {
                yield return new WaitForSeconds(0.8f);
            }
        }
    }
    public IEnumerator Stage10Pattern1Attack()
    {
        Vector2 bulletFirePosition = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f));

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.8f, 0.125f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.125f);

        // 탄막 1 발사 (파란색 쌀탄) (조준 방사탄) (1회 반사탄)
        for (int i = 0; i < 72; i++)
        {
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = bulletFirePosition;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                bullet.transform.SetParent(enemyBulletTemp1);
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                if (!bullet.GetComponent<ReflectBullet>()) bullet.AddComponent<ReflectBullet>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                ReflectBullet reflectBullet = bullet.GetComponent<ReflectBullet>();
                spriteRenderer.sprite = spriteCollection[55];
                spriteRenderer.sortingOrder = 3;
                capsuleCollider2D.isTrigger = true;
                capsuleCollider2D.size = new Vector2(0.025f, 0.05f);
                capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
                capsuleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletReflect = BulletReflect.BULLETREFLECT_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                if (enemyDatabase.GetEnemyCurrentHPRate() >= 0.6f)
                {
                    movingBullet.bulletMoveSpeed = 1.8f;
                }
                else if (enemyDatabase.GetEnemyCurrentHPRate() < 0.6f && enemyDatabase.GetEnemyCurrentHPRate() >= 0.4f)
                {
                    movingBullet.bulletMoveSpeed = 2.2f;
                }
                else if (enemyDatabase.GetEnemyCurrentHPRate() < 0.4f && enemyDatabase.GetEnemyCurrentHPRate() >= 0.2f)
                {
                    movingBullet.bulletMoveSpeed = 2.6f;
                }
                else
                {
                    movingBullet.bulletMoveSpeed = 3.0f;
                }
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector2(transform.position.x, transform.position.y - 10.0f));
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f + (5.0f * i));
                reflectBullet.reflectCount = 0;
                reflectBullet.reflectLimit = 1;
                reflectBullet.isSpriteChange = true;
                reflectBullet.isEffectOutput = true;
                reflectBullet.changeSpriteNumber = 49;
                reflectBullet.effectSpriteNumber = 325;
                reflectBullet.scaleDownSpeed = 0.72f;
                reflectBullet.scaleDownTime = 0.2f;
                reflectBullet.alphaUpSpeed = 0.4f;
            }
            else AddBulletPool();
        }
    }

    #endregion

    #region 패턴 11 (자작)

    // public IEnumerator Stage11()
    // {
    //     StartCoroutine(Stage11EnemyMove());
    //     StartCoroutine(Stage11Pattern1());
    //     StartCoroutine(Stage11Pattern2());
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage11EnemyMove()
    // {
    //     while (true)
    //     {
    //         Vector3 playerPosition = playerObject.transform.position;
    //         Vector3 randomPosition;
    // 
    //         if (playerPosition.x <= -3.0f)
    //         {
    //             randomPosition = new Vector3(Random.Range(playerPosition.x + 0.25f, playerPosition.x + 1.25f), Random.Range(1.5f, 3.0f), 0.0f);
    //         }
    //         else if (playerPosition.x >= 3.0f)
    //         {
    //             randomPosition = new Vector3(Random.Range(playerPosition.x - 1.25f, playerPosition.x - 0.25f), Random.Range(1.5f, 3.0f), 0.0f);
    //         }
    //         else
    //         {
    //             randomPosition = new Vector3(Random.Range(playerPosition.x - 0.75f, playerPosition.x + 0.75f), Random.Range(1.5f, 3.0f), 0.0f);
    //         }
    // 
    //         iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", 1.5f));
    //         StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, 1.5f));
    // 
    //         yield return new WaitForSeconds(2.5f);
    //     }
    // }
    // public IEnumerator Stage11Pattern1()
    // {
    //     int angleMultiply = 1;
    //     float rotateAngle = 0.0f;
    //     int rotateMultiply = 1;
    // 
    //     while (true)
    //     {
    //         StartCoroutine(Stage11Pattern1Attack(rotateAngle, angleMultiply));
    // 
    //         rotateAngle += (1.0f * rotateMultiply);
    //         rotateMultiply++;
    // 
    //         if (rotateAngle <= 720.0f)
    //         {
    //             yield return new WaitForSeconds(0.06f);
    //         }
    //         else
    //         {
    //             rotateAngle = 0.0f;
    //             angleMultiply *= -1;
    //             rotateMultiply = 0;
    //             yield return new WaitForSeconds(1.0f);
    //         }
    //     }
    // }
    // public IEnumerator Stage11Pattern1Attack(float rotateAngle, int angleMultiply)
    // {
    //     Vector2 playerPosition = playerObject.transform.position;
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 1 이펙트
    //     StartCoroutine(CreateBulletFireEffect(301, 0.8f, 0.15f, 0.3f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.15f);
    // 
    //     // 탄막 1 발사 (파란색, 하늘색 쐐기탄) (조준 방사탄)
    //     for (int i = 0; i < 16; i++)
    //     {
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = bulletFirePosition;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
    //             bullet.transform.SetParent(enemyBulletTemp1);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[i % 2 == 0 ? 103 : 105];
    //             spriteRenderer.sortingOrder = 3;
    //             capsuleCollider2D.isTrigger = true;
    //             capsuleCollider2D.size = new Vector2(0.04f, 0.06f);
    //             capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
    //             capsuleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = 9.0f;
    //             movingBullet.bulletDecelerationMoveSpeed = 0.3f;
    //             movingBullet.bulletDecelerationMoveSpeedMin = 3.0f;
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(playerPosition);
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f + (22.5f * i) + (angleMultiply == 1 ? rotateAngle : -rotateAngle));
    //         }
    //         else AddBulletPool();
    //     }
    // }
    // public IEnumerator Stage11Pattern2()
    // {
    //     int rotateMultiply = 1;
    // 
    //     while (true)
    //     {
    //         StartCoroutine(Stage11Pattern2Attack(rotateMultiply));
    //         rotateMultiply = (rotateMultiply == 1 ? -1 : 1);
    // 
    //         yield return new WaitForSeconds(0.25f);
    //     }
    // }
    // public IEnumerator Stage11Pattern2Attack(int rotateMultiply)
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 2 이펙트
    //     StartCoroutine(CreateBulletFireEffect(304, 0.6f, 0.225f, 0.3f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.225f);
    // 
    //     // 탄막 2 발사 (노란색, 주황색 부적탄) (조준 방사탄)
    //     for (int i = 0; i < 18; i++)
    //     {
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = bulletFirePosition;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
    //             bullet.transform.SetParent(enemyBulletTemp2);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[rotateMultiply == 1 ? 126 : 127];
    //             spriteRenderer.sortingOrder = 3;
    //             capsuleCollider2D.isTrigger = true;
    //             capsuleCollider2D.size = new Vector2(0.04f, 0.06f);
    //             capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
    //             capsuleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = 3.0f;
    //             movingBullet.bulletRotateSpeed = rotateMultiply == 1 ? 45.0f : -45.0f;
    //             movingBullet.bulletRotateLimit = 1.5f;
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
    //             movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f + (20.0f * i));
    //         }
    //         else AddBulletPool();
    //     }
    // }

    #endregion

    #region 패턴 12 (자작)

    // public IEnumerator Stage12()
    // {
    //     Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);
    // 
    //     iTween.MoveTo(gameObject, iTween.Hash("position", targetPosition, "easetype", iTween.EaseType.easeOutQuad, "time", 1.2f));
    //     StartCoroutine(EnemySpriteSet(targetPosition.x, transform.position.x, 1.2f));
    // 
    //     yield return new WaitForSeconds(1.5f);
    // 
    //     StartCoroutine(Stage12Pattern1());
    // }
    // public IEnumerator Stage12Pattern1()
    // {
    //     while (true)
    //     {
    //         StartCoroutine(Stage12Pattern1Attack());
    // 
    //         yield return new WaitForSeconds(0.25f);
    //     }
    // }
    // public IEnumerator Stage12Pattern1Attack()
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 1 이펙트
    //     StartCoroutine(CreateBulletFireEffect(303, 0.6f, 0.225f, 0.2f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.225f);
    // 
    //     // 탄막 1 발사 (초록색 화살탄) (각도 제한 랜덤탄)
    //     if (bulletManager.bulletPool.Count > 0)
    //     {
    //         GameObject bullet = bulletManager.bulletPool.Dequeue();
    //         bullet.SetActive(true);
    //         ClearChild(bullet);
    //         bullet.transform.position = bulletFirePosition;
    //         bullet.gameObject.tag = "BULLET_ENEMY";
    //         bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
    //         bullet.transform.SetParent(enemyBulletTemp1);
    //         if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //         if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
    //         if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //         if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //         if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //         if (!bullet.GetComponent<Stage12BulletFragmentation>()) bullet.AddComponent<Stage12BulletFragmentation>();
    //         SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //         CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
    //         InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //         MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //         spriteRenderer.sprite = spriteCollection[335];
    //         spriteRenderer.sortingOrder = 3;
    //         capsuleCollider2D.isTrigger = true;
    //         capsuleCollider2D.size = new Vector2(0.01f, 0.2f);
    //         capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
    //         capsuleCollider2D.enabled = false;
    //         initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //         initializeBullet.bulletObject = bullet.gameObject;
    //         initializeBullet.targetObject = playerObject;
    //         initializeBullet.isGrazed = false;
    //         movingBullet.bulletMoveSpeed = 5.0f;
    //         movingBullet.bulletDecelerationMoveSpeed = Random.Range(0.04f, 0.06f);
    //         movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
    //         movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //         movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector2(bulletFirePosition.x, bulletFirePosition.y + 10.0f));
    //         float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //         movingBullet.ChangeRotateAngle(angle - 90.0f + Random.Range(-40.0f, 40.0f));
    //     }
    //     else AddBulletPool();
    // }

    #endregion

    #region 패턴 13 (동방요요몽 판타즘 스테이지 - 야쿠모 유카리 7스펠 "이중흑사접" 리메이크)

    // public IEnumerator Stage13()
    // {
    //     StartCoroutine(Stage13Pattern1());
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage13Pattern1()
    // {
    //     bool isTextureIndexChange = false;
    // 
    //     while (true)
    //     {
    //         StartCoroutine(Stage13Pattern1Attack1(isTextureIndexChange));
    //         StartCoroutine(Stage13Pattern1Attack2(isTextureIndexChange));
    //         StartCoroutine(Stage13Pattern2());
    // 
    //         yield return new WaitForSeconds(4.0f);
    // 
    //         StartCoroutine(Stage13EnemyMove());
    //         if (isTextureIndexChange.Equals(false))
    //         {
    //             isTextureIndexChange = true;
    //         }
    //         else
    //         {
    //             isTextureIndexChange = false;
    //         }
    // 
    //         yield return new WaitForSeconds(1.5f);
    //     }
    // }
    // public IEnumerator Stage13EnemyMove()
    // {
    //     Vector3 randomPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(2.0f, 4.25f), 0.0f);
    // 
    //     iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", 1.0f));
    //     StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, 1.0f));
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage13Pattern1Attack1(bool isTextureIndexChange)
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 1 이펙트
    //     StartCoroutine(CreateBulletFireEffect(301, 0.8f, 0.15f, 0.3f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.15f);
    // 
    //     // 탄막 1 발사 (파란색 보석탄, 쿠나이탄) (랜덤탄)
    //     for (int i = 0; i < 192; i++)
    //     {
    //         int randomValue = Random.Range(1, 5);
    // 
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = bulletFirePosition;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
    //             bullet.transform.SetParent(enemyBulletTemp1);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             if (!bullet.GetComponent<Stage13BulletRotate>()) bullet.AddComponent<Stage13BulletRotate>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[isTextureIndexChange.Equals(false) ? 71 : 87];
    //             spriteRenderer.sortingOrder = 3;
    //             capsuleCollider2D.isTrigger = true;
    //             capsuleCollider2D.size = new Vector2(0.02f, isTextureIndexChange.Equals(false) ? 0.05f : 0.055f);
    //             capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
    //             capsuleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             switch (randomValue)
    //             {
    //                 case 1:
    //                     movingBullet.bulletMoveSpeed = Random.Range(2.0f, 4.0f);
    //                     break;
    //                 case 2:
    //                     movingBullet.bulletMoveSpeed = Random.Range(4.0f, 6.0f);
    //                     break;
    //                 case 3:
    //                     movingBullet.bulletMoveSpeed = Random.Range(6.0f, 8.0f);
    //                     break;
    //                 case 4:
    //                     movingBullet.bulletMoveSpeed = Random.Range(8.0f, 10.0f);
    //                     break;
    //                 default:
    //                     movingBullet.bulletMoveSpeed = 0.0f;
    //                     break;
    //             }
    //             movingBullet.bulletAccelerationMoveSpeed = 0.015f;
    //             movingBullet.bulletAccelerationMoveSpeedMax = 5.0f;
    //             movingBullet.bulletDecelerationMoveSpeed = movingBullet.bulletMoveSpeed * 0.01f;
    //             movingBullet.bulletRotateSpeed = 60.0f;
    //             movingBullet.bulletRotateLimit = 3.0f;
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f);
    //         }
    //         else AddBulletPool();
    //     }
    // }
    // public IEnumerator Stage13Pattern1Attack2(bool isTextureIndexChange)
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 2 이펙트
    //     StartCoroutine(CreateBulletFireEffect(300, 0.8f, 0.15f, 0.3f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.15f);
    // 
    //     // 탄막 2 발사 (분홍색 보석탄, 쿠나이탄) (랜덤탄)
    //     for (int i = 0; i < 192; i++)
    //     {
    //         int randomValue = Random.Range(1, 6);
    // 
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = bulletFirePosition;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
    //             bullet.transform.SetParent(enemyBulletTemp1);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             if (!bullet.GetComponent<Stage13BulletRotate>()) bullet.AddComponent<Stage13BulletRotate>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[isTextureIndexChange.Equals(false) ? 69 : 85];
    //             spriteRenderer.sortingOrder = 3;
    //             capsuleCollider2D.isTrigger = true;
    //             capsuleCollider2D.size = new Vector2(0.02f, isTextureIndexChange.Equals(false) ? 0.05f : 0.055f);
    //             capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
    //             capsuleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             switch (randomValue)
    //             {
    //                 case 1:
    //                     movingBullet.bulletMoveSpeed = Random.Range(2.0f, 4.0f);
    //                     break;
    //                 case 2:
    //                     movingBullet.bulletMoveSpeed = Random.Range(4.0f, 6.0f);
    //                     break;
    //                 case 3:
    //                     movingBullet.bulletMoveSpeed = Random.Range(6.0f, 8.0f);
    //                     break;
    //                 case 4:
    //                     movingBullet.bulletMoveSpeed = Random.Range(8.0f, 10.0f);
    //                     break;
    //                 case 5:
    //                     movingBullet.bulletMoveSpeed = Random.Range(10.0f, 12.0f);
    //                     break;
    //                 default:
    //                     movingBullet.bulletMoveSpeed = 0.0f;
    //                     break;
    //             }
    //             movingBullet.bulletAccelerationMoveSpeed = 0.015f;
    //             movingBullet.bulletAccelerationMoveSpeedMax = 5.0f;
    //             movingBullet.bulletDecelerationMoveSpeed = movingBullet.bulletMoveSpeed * 0.01f;
    //             movingBullet.bulletRotateSpeed = -60.0f;
    //             movingBullet.bulletRotateLimit = 3.0f;
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f);
    //         }
    //         else AddBulletPool();
    //     }
    // }
    // public IEnumerator Stage13Pattern2()
    // {
    //     int fireCount = 0;
    // 
    //     while (true)
    //     {
    //         StartCoroutine(Stage13Pattern2Attack());
    //         fireCount++;
    // 
    //         if (fireCount.Equals(8))
    //         {
    //             fireCount = 0;
    //             break;
    //         }
    //         else
    //         {
    //             yield return new WaitForSeconds(0.15f);
    //         }
    //     }
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage13Pattern2Attack()
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 3 이펙트
    //     StartCoroutine(CreateBulletFireEffect(304, 0.6f, 0.225f, 0.4f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.225f);
    // 
    //     // 탄막 3 발사 (노란색 나비탄) (조준 방사탄)
    //     for (int i = 0; i < 18; i++)
    //     {
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = bulletFirePosition;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
    //             bullet.transform.SetParent(enemyBulletTemp2);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[272];
    //             spriteRenderer.sortingOrder = 3;
    //             circleCollider2D.isTrigger = true;
    //             circleCollider2D.radius = 0.02f;
    //             circleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = 5.0f;
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f + (20.0f * i));
    //         }
    //         else AddBulletPool();
    //     }
    // }

    #endregion

    #region 패턴 14 (파동과 입자의 경계 모작)

    // public IEnumerator Stage14()
    // {
    //     Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);
    // 
    //     iTween.MoveTo(gameObject, iTween.Hash("position", targetPosition, "easetype", iTween.EaseType.easeOutQuad, "time", 1.2f));
    //     StartCoroutine(EnemySpriteSet(targetPosition.x, transform.position.x, 1.2f));
    // 
    //     yield return new WaitForSeconds(1.5f);
    // 
    //     StartCoroutine(Stage14Pattern1());
    // }
    // public IEnumerator Stage14Pattern1()
    // {
    //     float rotateAngle = 0.0f;
    //     int fireCount = 0;
    //     int rotateMultiply = 1;
    //     int rotateDirectionChange = 0;
    // 
    //     while (true)
    //     {
    //         StartCoroutine(Stage14Pattern1Attack(rotateAngle, rotateMultiply));
    // 
    //         rotateAngle += (0.4f * rotateMultiply);
    //         fireCount++;
    //         if (rotateDirectionChange.Equals(0))
    //         {
    //             rotateMultiply++;
    //         }
    //         else
    //         {
    //             rotateMultiply--;
    //         }
    // 
    //         if (fireCount.Equals(24))
    //         {
    //             fireCount = 0;
    //             rotateDirectionChange = Random.Range(0, 2);
    //         }
    // 
    //         if (rotateMultiply >= 120) rotateDirectionChange = 1;
    //         else if (rotateMultiply <= -120) rotateDirectionChange = 0;
    // 
    //         yield return new WaitForSeconds(0.06f);
    //     }
    // }
    // public IEnumerator Stage14Pattern1Attack(float rotateAngle, float rotateMultiply)
    // {
    //     Vector2 bulletFirePosition = transform.position;
    // 
    //     // 탄막 1 이펙트
    //     StartCoroutine(CreateBulletFireEffect(300, 0.8f, 0.1f, 0.4f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.1f);
    // 
    //     // 탄막 1 발사 (보라색 쌀탄) (회전 방사탄)
    //     for (int i = 0; i < 16; i++)
    //     {
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = bulletFirePosition;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
    //             bullet.transform.SetParent(enemyBulletTemp1);
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //             CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
    //             InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //             MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //             spriteRenderer.sprite = spriteCollection[53];
    //             spriteRenderer.sortingOrder = 3;
    //             capsuleCollider2D.isTrigger = true;
    //             capsuleCollider2D.size = new Vector2(0.025f, 0.05f);
    //             capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
    //             capsuleCollider2D.enabled = false;
    //             initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //             initializeBullet.bulletObject = bullet.gameObject;
    //             initializeBullet.targetObject = playerObject;
    //             initializeBullet.isGrazed = false;
    //             movingBullet.bulletMoveSpeed = 2.0f;
    //             movingBullet.bulletAccelerationMoveSpeed = 0.025f;
    //             movingBullet.bulletAccelerationMoveSpeedMax = 6.0f;
    //             movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
    //             movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector2(transform.position.x, transform.position.y - 1.0f));
    //             float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //             movingBullet.ChangeRotateAngle(angle - 90.0f + (22.5f * i) + rotateAngle);
    //         }
    //         else AddBulletPool();
    //     }
    // 
    //     yield return null;
    // }

    #endregion

    #region 패턴 20 (도돈파치 대왕생 2주차 5스테이지 - 히바치 5패턴 리메이크)

    // public IEnumerator Stage20()
    // {
    //     StartCoroutine(Stage20EnemyMove());
    //     StartCoroutine(Stage20EnemyAttack1());
    // 
    //     yield return new WaitForSeconds(2.5f);
    // 
    //     StartCoroutine(Stage20EnemyAttack2());
    // 
    //     yield return new WaitForSeconds(2.5f);
    // 
    //     StartCoroutine(Stage20EnemyAttack3());
    // }
    // public IEnumerator Stage20EnemyMove()
    // {
    //     while (true)
    //     {
    //         Vector3 playerPosition = playerObject.transform.position;
    //         Vector3 randomPosition;
    //         if (playerPosition.x <= -3.0f)
    //         {
    //             randomPosition = new Vector3(Random.Range(playerPosition.x + 0.25f, playerPosition.x + 1.25f), Random.Range(1.5f, 2.5f), 0.0f);
    //         }
    //         else if (playerPosition.x >= 3.0f)
    //         {
    //             randomPosition = new Vector3(Random.Range(playerPosition.x - 1.25f, playerPosition.x - 0.25f), Random.Range(1.5f, 2.5f), 0.0f);
    //         }
    //         else
    //         {
    //             randomPosition = new Vector3(Random.Range(playerPosition.x - 0.75f, playerPosition.x + 0.75f), Random.Range(1.5f, 2.5f), 0.0f);
    //         }
    // 
    //         iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", 1.0f));
    //         StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, 1.0f));
    // 
    //         yield return new WaitForSeconds(1.0f);
    //     }
    // }
    // public IEnumerator Stage20EnemyAttack1()
    // {
    //     float rotateAngle = 0.0f;
    // 
    //     while (true)
    //     {
    //         Vector2 bulletFirePosition = transform.position;
    // 
    //         // 탄막 1 이펙트
    //         StartCoroutine(CreateBulletFireEffect(299, 0.8f, 0.1f, 0.4f, bulletFirePosition));
    // 
    //         yield return new WaitForSeconds(0.1f);
    // 
    //         // 탄막 1 발사 (빨간색 대옥탄) (조준 회전 방사탄)
    //         for (int i = 0; i < 2; i++)
    //         {
    //             if (bulletManager.bulletPool.Count > 0)
    //             {
    //                 GameObject bullet = bulletManager.bulletPool.Dequeue();
    //                 bullet.SetActive(true);
    //                 ClearChild(bullet);
    //                 bullet.transform.position = transform.position;
    //                 bullet.gameObject.tag = "BULLET_ENEMY";
    //                 bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
    //                 bullet.transform.SetParent(enemyBulletTemp1);
    //                 if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //                 if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //                 if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //                 if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //                 if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //                 SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //                 CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //                 InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //                 MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //                 spriteRenderer.sprite = spriteCollection[241];
    //                 spriteRenderer.sortingOrder = 3;
    //                 spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.85f);
    //                 circleCollider2D.isTrigger = true;
    //                 circleCollider2D.radius = 0.135f;
    //                 circleCollider2D.enabled = false;
    //                 initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //                 initializeBullet.bulletObject = bullet.gameObject;
    //                 initializeBullet.targetObject = playerObject;
    //                 initializeBullet.isGrazed = false;
    //                 movingBullet.bulletMoveSpeed = 6.0f;
    //                 movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //                 movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //                 movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination
    //                     (new Vector3(transform.position.x, (i == 0 ? transform.position.y - 10.0f : transform.position.y + 10.0f), 0.0f));
    //                 float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //                 movingBullet.ChangeRotateAngle(angle - 90.0f + rotateAngle + Random.Range(-3.0f, 3.0f));
    //             }
    //             else AddBulletPool();
    //         }
    // 
    //         rotateAngle += 6.0f;
    //         if (rotateAngle >= 360.0f)
    //         {
    //             rotateAngle = 0.0f;
    //         }
    //         yield return new WaitForSeconds(0.1f);
    //     }
    // }
    // public IEnumerator Stage20EnemyAttack2()
    // {
    //     float rotateAngle = 0.0f;
    // 
    //     while (true)
    //     {
    //         Vector2 bulletFirePosition = transform.position;
    // 
    //         // 탄막 2 이펙트
    //         StartCoroutine(CreateBulletFireEffect(299, 0.8f, 0.1f, 0.4f, bulletFirePosition));
    // 
    //         yield return new WaitForSeconds(0.1f);
    // 
    //         // 탄막 2 발사 (빨간색 대옥탄) (조준 회전 방사탄)
    //         for (int i = 0; i < 2; i++)
    //         {
    //             if (bulletManager.bulletPool.Count > 0)
    //             {
    //                 GameObject bullet = bulletManager.bulletPool.Dequeue();
    //                 bullet.SetActive(true);
    //                 ClearChild(bullet);
    //                 bullet.transform.position = transform.position;
    //                 bullet.gameObject.tag = "BULLET_ENEMY";
    //                 bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
    //                 bullet.transform.SetParent(enemyBulletTemp1);
    //                 if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //                 if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //                 if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //                 if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //                 if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //                 SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //                 CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //                 InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //                 MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //                 spriteRenderer.sprite = spriteCollection[241];
    //                 spriteRenderer.sortingOrder = 3;
    //                 spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.85f);
    //                 circleCollider2D.isTrigger = true;
    //                 circleCollider2D.radius = 0.135f;
    //                 circleCollider2D.enabled = false;
    //                 initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //                 initializeBullet.bulletObject = bullet.gameObject;
    //                 initializeBullet.targetObject = playerObject;
    //                 initializeBullet.isGrazed = false;
    //                 movingBullet.bulletMoveSpeed = 6.0f;
    //                 movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //                 movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //                 movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination
    //                     (new Vector3(transform.position.x, (i == 0 ? transform.position.y - 10.0f : transform.position.y + 10.0f), 0.0f));
    //                 float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //                 movingBullet.ChangeRotateAngle(angle - 90.0f - rotateAngle + Random.Range(-3.0f, 3.0f));
    //             }
    //             else AddBulletPool();
    //         }
    // 
    //         rotateAngle += 6.0f;
    //         if (rotateAngle >= 360.0f)
    //         {
    //             rotateAngle = 0.0f;
    //         }
    //         yield return new WaitForSeconds(0.1f);
    //     }
    // }
    // public IEnumerator Stage20EnemyAttack3()
    // {
    //     float rotateAngle = 0.0f;
    // 
    //     while (true)
    //     {
    //         Vector2 bulletFirePosition = transform.position;
    // 
    //         // 탄막 3 이펙트
    //         StartCoroutine(CreateBulletFireEffect(301, 0.8f, 0.1f, 0.5f, bulletFirePosition));
    // 
    //         yield return new WaitForSeconds(0.1f);
    // 
    //         // 탄막 3 발사 (파란색 환탄) (조준 회전 방사탄)
    //         for (int i = 0; i < 12; i++)
    //         {
    //             if (bulletManager.bulletPool.Count > 0)
    //             {
    //                 GameObject bullet = bulletManager.bulletPool.Dequeue();
    //                 bullet.SetActive(true);
    //                 ClearChild(bullet);
    //                 bullet.transform.position = transform.position;
    //                 bullet.gameObject.tag = "BULLET_ENEMY";
    //                 bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
    //                 bullet.transform.SetParent(enemyBulletTemp2);
    //                 if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //                 if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //                 if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //                 if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //                 if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //                 SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //                 CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //                 InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //                 MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //                 spriteRenderer.sprite = spriteCollection[261];
    //                 spriteRenderer.sortingOrder = 3;
    //                 circleCollider2D.isTrigger = true;
    //                 circleCollider2D.radius = 0.08f;
    //                 circleCollider2D.enabled = false;
    //                 bullet.AddComponent<InitializeBullet>();
    //                 bullet.AddComponent<MovingBullet>();
    //                 bullet.AddComponent<EraseBullet>();
    //                 initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
    //                 initializeBullet.bulletObject = bullet.gameObject;
    //                 initializeBullet.targetObject = playerObject;
    //                 initializeBullet.isGrazed = false;
    //                 movingBullet.bulletMoveSpeed = 6.0f;
    //                 movingBullet.bulletRotateSpeed = 60.0f;
    //                 movingBullet.bulletRotateLimit = 1.5f;
    //                 movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //                 movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
    //                 movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector3(transform.position.x, transform.position.y - 10.0f, 0.0f));
    //                 float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //                 movingBullet.ChangeRotateAngle(angle - 90.0f + (30.0f * i) + rotateAngle + Random.Range(-6.0f, 6.0f));
    //             }
    //             else AddBulletPool();
    //         }
    // 
    //         rotateAngle += 12.0f;
    //         if (rotateAngle >= 360.0f)
    //         {
    //             rotateAngle = 0.0f;
    //         }
    //         yield return new WaitForSeconds(0.02f);
    //     }
    // }

    #endregion

    #endregion

    #region 기타 함수

    #region 탄막 발사 함수

    // 일반 탄막
    public IEnumerator BulletFire
        (Vector2 bulletFirePosition, Vector3 bulletScale, string bulletTag,
        int bulletLayer, Transform bulletParent, int colliderType, float circleColliderRadius,
        float capsuleColliderSizeX, float capsuleColliderSizeY, float capsuleColliderOffsetX, float capsuleColliderOffsetY,
        float boxColliderSizeX, float boxColliderSizeY, float boxColliderOffsetX, float boxColliderOffsetY,
        float spriteAlpha, int spriteNumber, BulletType bulletType, GameObject targetObject,
        BulletSpeedState bulletSpeedState, float bulletMoveSpeed,
        float bulletAccelerationMoveSpeed, float bulletAccelerationMoveSpeedMax,
        float bulletDecelerationMoveSpeed, float bulletDecelerationMoveSpeedMin, bool bulletMoveSpeedLoopBool,
        BulletRotateState bulletRotateState, float bulletRotateSpeed, float bulletRotateLimit,
        int bulletDestinationType, Vector2 targetPosition, float addRotateAngle, int addCustomScript = 0,
        bool isLaserType = false, float laserWidth = 1.8f, bool isLaserEnabled = false, bool isLaserDisabled = false,
        float laserEnableTime = 0.0f, float laserEnableSpeed = 0.0f, float laserDisableTime = 0.0f, float laserDisableSpeed = 0.0f,
        BulletRotateState laserRotateState = BulletRotateState.BULLETROTATESTATE_NONE, float laserRotateSpeed = 0.0f,
        bool isLaserRotateEnabled = false, bool isLaserRotateDisabled = false)
    {
        if (bulletManager.bulletPool.Count > 0)
        {
            GameObject bullet = bulletManager.bulletPool.Dequeue();
            bullet.SetActive(true);
            ClearChild(bullet);
            bullet.transform.position = bulletFirePosition;
            bullet.transform.localScale = bulletScale;
            bullet.gameObject.tag = bulletTag;
            bullet.gameObject.layer = bulletLayer;
            bullet.transform.SetParent(bulletParent);
            if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
            if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
            if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
            if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
            SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
            InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
            MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
            switch (colliderType)
            {
                case 1:
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                    circleCollider2D.isTrigger = true;
                    circleCollider2D.radius = circleColliderRadius;
                    circleCollider2D.enabled = false;
                    break;
                case 2:
                    if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
                    CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
                    capsuleCollider2D.isTrigger = true;
                    capsuleCollider2D.size = new Vector2(capsuleColliderSizeX, capsuleColliderSizeY);
                    capsuleCollider2D.offset = new Vector2(capsuleColliderOffsetX, capsuleColliderOffsetY);
                    capsuleCollider2D.enabled = false;
                    break;
                case 3:
                    if (!bullet.GetComponent<BoxCollider2D>()) bullet.AddComponent<BoxCollider2D>();
                    BoxCollider2D boxCollider2D = bullet.GetComponent<BoxCollider2D>();
                    boxCollider2D.isTrigger = true;
                    boxCollider2D.size = new Vector2(boxColliderSizeX, boxColliderSizeY);
                    boxCollider2D.offset = new Vector2(boxColliderOffsetX, boxColliderOffsetY);
                    boxCollider2D.enabled = false;
                    break;
                default:
                    break;
            }
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, spriteAlpha);
            spriteRenderer.sprite = spriteCollection[spriteNumber];
            spriteRenderer.sortingOrder = 3;
            initializeBullet.bulletType = bulletType;
            initializeBullet.bulletObject = bullet.gameObject;
            initializeBullet.targetObject = targetObject;
            initializeBullet.isGrazed = false;
            movingBullet.bulletSpeedState = bulletSpeedState;
            movingBullet.bulletMoveSpeed = bulletMoveSpeed;
            movingBullet.bulletAccelerationMoveSpeed = bulletAccelerationMoveSpeed;
            movingBullet.bulletAccelerationMoveSpeedMax = bulletAccelerationMoveSpeedMax;
            movingBullet.bulletDecelerationMoveSpeed = bulletDecelerationMoveSpeed;
            movingBullet.bulletDecelerationMoveSpeedMin = bulletDecelerationMoveSpeedMin;
            movingBullet.bulletMoveSpeedLoopBool = bulletMoveSpeedLoopBool;
            movingBullet.bulletRotateState = bulletRotateState;
            movingBullet.bulletRotateSpeed = bulletRotateSpeed;
            movingBullet.bulletRotateLimit = bulletRotateLimit;
            switch (bulletDestinationType)
            {
                case 1:
                    movingBullet.bulletDestination = initializeBullet.GetBulletDestination(targetPosition);
                    break;
                case 2:
                    movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                    break;
                case 3:
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
                    break;
                case 4:
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(targetPosition);
                    break;
                default:
                    break;
            }
            float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
            movingBullet.ChangeRotateAngle(angle - 90.0f + addRotateAngle);
            if (isLaserType.Equals(true))
            {
                if (!bullet.GetComponent<LaserBullet>()) bullet.AddComponent<LaserBullet>();
                LaserBullet laserBullet = bullet.GetComponent<LaserBullet>();
                laserBullet.laserWidth = laserWidth;
                laserBullet.isLaserEnabled = isLaserEnabled;
                laserBullet.isLaserDisabled = isLaserDisabled;
                laserBullet.laserEnableTime = laserEnableTime;
                laserBullet.laserEnableSpeed = laserEnableSpeed;
                laserBullet.laserDisableTime = laserDisableTime;
                laserBullet.laserDisableSpeed = laserDisableSpeed;
                laserBullet.laserRotateState = laserRotateState;
                laserBullet.laserRotateSpeed = laserRotateSpeed;
                laserBullet.isLaserRotateEnabled = isLaserRotateEnabled;
                laserBullet.isLaserRotateDisabled = isLaserRotateDisabled;
            }
            // 패턴 별 커스텀 스크립트는 여기에 집어넣기 !! 
            switch (addCustomScript)
            {
                case 1:
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }
        else AddBulletPool();

        yield return null;
    }

    // 빈 탄막
    public IEnumerator EmptyBulletFire
        (GameObject emptyBullet, Vector2 bulletFirePosition, Vector3 bulletScale, string bulletTag,
        int bulletLayer, Transform bulletParent, int colliderType, float circleColliderRadius,
        float capsuleColliderSizeX, float capsuleColliderSizeY, float capsuleColliderOffsetX, float capsuleColliderOffsetY,
        float boxColliderSizeX, float boxColliderSizeY, float boxColliderOffsetX, float boxColliderOffsetY,
        float spriteAlpha, int spriteNumber, BulletType bulletType, GameObject targetObject,
        BulletSpeedState bulletSpeedState, float bulletMoveSpeed,
        float bulletAccelerationMoveSpeed, float bulletAccelerationMoveSpeedMax,
        float bulletDecelerationMoveSpeed, float bulletDecelerationMoveSpeedMin, bool bulletMoveSpeedLoopBool,
        BulletRotateState bulletRotateState, float bulletRotateSpeed, float bulletRotateLimit,
        int bulletDestinationType, Vector2 targetPosition, float addRotateAngle, int addCustomScript = 0,
        bool isLaserType = false, float laserWidth = 1.8f, bool isLaserEnabled = false, bool isLaserDisabled = false,
        float laserEnableTime = 0.0f, float laserEnableSpeed = 0.0f, float laserDisableTime = 0.0f, float laserDisableSpeed = 0.0f,
        BulletRotateState laserRotateState = BulletRotateState.BULLETROTATESTATE_NONE, float laserRotateSpeed = 0.0f,
        bool isLaserRotateEnabled = false, bool isLaserRotateDisabled = false)
    {
        if (bulletManager.bulletPool.Count > 0)
        {
            emptyBullet.SetActive(true);
            ClearChild(emptyBullet);
            emptyBullet.transform.position = bulletFirePosition;
            emptyBullet.transform.localScale = bulletScale;
            emptyBullet.gameObject.tag = bulletTag;
            emptyBullet.gameObject.layer = bulletLayer;
            emptyBullet.transform.SetParent(bulletParent);
            if (!emptyBullet.GetComponent<SpriteRenderer>()) emptyBullet.AddComponent<SpriteRenderer>();
            if (!emptyBullet.GetComponent<InitializeBullet>()) emptyBullet.AddComponent<InitializeBullet>();
            if (!emptyBullet.GetComponent<MovingBullet>()) emptyBullet.AddComponent<MovingBullet>();
            if (!emptyBullet.GetComponent<EraseBullet>()) emptyBullet.AddComponent<EraseBullet>();
            SpriteRenderer spriteRenderer = emptyBullet.GetComponent<SpriteRenderer>();
            InitializeBullet initializeBullet = emptyBullet.GetComponent<InitializeBullet>();
            MovingBullet movingBullet = emptyBullet.GetComponent<MovingBullet>();
            switch (colliderType)
            {
                case 1:
                    if (!emptyBullet.GetComponent<CircleCollider2D>()) emptyBullet.AddComponent<CircleCollider2D>();
                    CircleCollider2D circleCollider2D = emptyBullet.GetComponent<CircleCollider2D>();
                    circleCollider2D.isTrigger = true;
                    circleCollider2D.radius = circleColliderRadius;
                    circleCollider2D.enabled = false;
                    break;
                case 2:
                    if (!emptyBullet.GetComponent<CapsuleCollider2D>()) emptyBullet.AddComponent<CapsuleCollider2D>();
                    CapsuleCollider2D capsuleCollider2D = emptyBullet.GetComponent<CapsuleCollider2D>();
                    capsuleCollider2D.isTrigger = true;
                    capsuleCollider2D.size = new Vector2(capsuleColliderSizeX, capsuleColliderSizeY);
                    capsuleCollider2D.offset = new Vector2(capsuleColliderOffsetX, capsuleColliderOffsetY);
                    capsuleCollider2D.enabled = false;
                    break;
                case 3:
                    if (!emptyBullet.GetComponent<BoxCollider2D>()) emptyBullet.AddComponent<BoxCollider2D>();
                    BoxCollider2D boxCollider2D = emptyBullet.GetComponent<BoxCollider2D>();
                    boxCollider2D.isTrigger = true;
                    boxCollider2D.size = new Vector2(boxColliderSizeX, boxColliderSizeY);
                    boxCollider2D.offset = new Vector2(boxColliderOffsetX, boxColliderOffsetY);
                    boxCollider2D.enabled = false;
                    break;
                default:
                    break;
            }
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, spriteAlpha);
            spriteRenderer.sprite = spriteCollection[spriteNumber];
            spriteRenderer.sortingOrder = 3;
            initializeBullet.bulletType = bulletType;
            initializeBullet.bulletObject = emptyBullet.gameObject;
            initializeBullet.targetObject = targetObject;
            initializeBullet.isGrazed = false;
            movingBullet.bulletSpeedState = bulletSpeedState;
            movingBullet.bulletMoveSpeed = bulletMoveSpeed;
            movingBullet.bulletAccelerationMoveSpeed = bulletAccelerationMoveSpeed;
            movingBullet.bulletAccelerationMoveSpeedMax = bulletAccelerationMoveSpeedMax;
            movingBullet.bulletDecelerationMoveSpeed = bulletDecelerationMoveSpeed;
            movingBullet.bulletDecelerationMoveSpeedMin = bulletDecelerationMoveSpeedMin;
            movingBullet.bulletMoveSpeedLoopBool = bulletMoveSpeedLoopBool;
            movingBullet.bulletRotateState = bulletRotateState;
            movingBullet.bulletRotateSpeed = bulletRotateSpeed;
            movingBullet.bulletRotateLimit = bulletRotateLimit;
            switch (bulletDestinationType)
            {
                case 1:
                    movingBullet.bulletDestination = initializeBullet.GetBulletDestination(targetPosition);
                    break;
                case 2:
                    movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                    break;
                case 3:
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
                    break;
                case 4:
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(targetPosition);
                    break;
                default:
                    break;
            }
            float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
            movingBullet.ChangeRotateAngle(angle - 90.0f + addRotateAngle);
            if (isLaserType.Equals(true))
            {
                if (!emptyBullet.GetComponent<LaserBullet>()) emptyBullet.AddComponent<LaserBullet>();
                LaserBullet laserBullet = emptyBullet.GetComponent<LaserBullet>();
                laserBullet.laserWidth = laserWidth;
                laserBullet.isLaserEnabled = isLaserEnabled;
                laserBullet.isLaserDisabled = isLaserDisabled;
                laserBullet.laserEnableTime = laserEnableTime;
                laserBullet.laserEnableSpeed = laserEnableSpeed;
                laserBullet.laserDisableTime = laserDisableTime;
                laserBullet.laserDisableSpeed = laserDisableSpeed;
                laserBullet.laserRotateState = laserRotateState;
                laserBullet.laserRotateSpeed = laserRotateSpeed;
                laserBullet.isLaserRotateEnabled = isLaserRotateEnabled;
                laserBullet.isLaserRotateDisabled = isLaserRotateDisabled;
            }
            // 패턴 별 커스텀 스크립트는 여기에 집어넣기 !! 
            switch (addCustomScript)
            {
                case 1:
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }
        else AddBulletPool();

        yield return null;
    }

    #endregion

    #region 불릿 풀 추가 함수
    public void AddBulletPool()
    {
        GameObject bullet = Instantiate(bulletManager.bulletObject);
        bullet.SetActive(false);
        bullet.transform.SetParent(bulletManager.bulletParent.transform);
        bulletManager.bulletPool.Enqueue(bullet);
    }
    public void AddEffect()
    {
        GameObject effect = Instantiate(effectManager.bulletObject);
        effect.SetActive(false);
        effect.transform.SetParent(effectManager.bulletParent.transform);
        effectManager.bulletPool.Enqueue(effect);
    }
    #endregion

    #region 차일드 제거 함수
    public static void ClearChild(GameObject bullet)
    {
        if (bullet.transform.childCount > 0)
        {
            for (int i = 0; i < bullet.transform.childCount; i++)
            {
                if (bullet.transform.GetChild(i).gameObject.activeSelf == true)
                {
                    bullet.transform.GetChild(i).GetComponent<EraseBullet>().ClearBullet();
                }
            }
        }
    }
    #endregion

    #region 적 스프라이트 조절 함수
    public IEnumerator EnemySpriteSet(float randomPositionX, float enemyPositionX, float time)
    {
        if (randomPositionX <= enemyPositionX)
        {
            body.GetComponent<SpriteRenderer>().flipX = true;
            body.GetComponent<EnemySprite>().isLeftMove = true;
        }
        else if (randomPositionX > enemyPositionX)
        {
            body.GetComponent<EnemySprite>().isRightMove = true;
        }
    
        yield return new WaitForSeconds(time);
    
        body.GetComponent<EnemySprite>().isSpriteReturn = true;
    }
    #endregion

    #region 탄막 발사 이펙트 생성 함수
    
    public IEnumerator CreateBulletFireEffect(int spriteNumber, float scaleDownSpeed, float scaleDownTime, float alphaUpSpeed, Vector2 effectPosition)
    {
        if (effectManager.bulletPool.Count > 0)
        {
            GameObject effect = effectManager.bulletPool.Dequeue();
            if (!effect.GetComponent<EraseEffect>()) effect.AddComponent<EraseEffect>();
            SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
            effect.SetActive(true);
            effect.transform.position = effectPosition;
            spriteRenderer.sprite = spriteCollection[spriteNumber];
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.4f);
            spriteRenderer.sortingOrder = 4;

            StartCoroutine(EffectAlphaUp(effect, alphaUpSpeed));
            StartCoroutine(EffectScaleDown(effect, scaleDownSpeed, scaleDownTime));
        }
        else AddEffect();

        yield return null;
    }
    public IEnumerator CreateBulletFireEffect(int spriteNumber, float scaleUpSpeed, float scaleLimit, float alphaUpSpeed, float alphaDownSpeed, float alphaRemainTime, Vector2 effectPosition)
    {
        if (effectManager.bulletPool.Count > 0)
        {
            GameObject effect = effectManager.bulletPool.Dequeue();
            if (!effect.GetComponent<EraseEffect>()) effect.AddComponent<EraseEffect>();
            SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
            effect.SetActive(true);
            effect.transform.position = effectPosition;
            spriteRenderer.sprite = spriteCollection[spriteNumber];
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.4f);

            StartCoroutine(EffectAlphaUp(effect, alphaUpSpeed, alphaDownSpeed, alphaRemainTime));
            StartCoroutine(EffectScaleUp(effect, scaleUpSpeed, scaleLimit));
        }
        else AddEffect();

        yield return null;
    }
    public IEnumerator EffectAlphaUp(GameObject effect, float alphaUpSpeed)
    {
        SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
        float alpha = 0.0f;

        while (true)
        {
            alpha += alphaUpSpeed;
            if (alpha >= 1.0f) break;

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
    public IEnumerator EffectAlphaUp(GameObject effect, float alphaUpSpeed, float alphaDownSpeed, float alphaRemainTime)
    {
        SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
        float alpha = 0.0f;

        while (true)
        {
            alpha += alphaUpSpeed;
            if (alpha >= 1.0f)
            {
                alpha = 1.0f;
                break;
            }

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(alphaRemainTime);

        StartCoroutine(EffectAlphaDown(effect, alphaDownSpeed));
    }
    public IEnumerator EffectScaleUp(GameObject effect, float scaleUpSpeed, float scaleLimit)
    {
        EraseEffect eraseEffect = effect.GetComponent<EraseEffect>();

        effect.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);

        while (true)
        {
            if (effect.transform.localScale.x >= scaleLimit || effect.transform.localScale.y >= scaleLimit)
            {
                effect.transform.localScale = new Vector3(scaleLimit, scaleLimit, 0.0f);
                break;
            }
            else
            {
                effect.transform.localScale = new Vector3(effect.transform.localScale.x + scaleUpSpeed, effect.transform.localScale.y + scaleUpSpeed, 0.0f);
            }

            yield return new WaitForEndOfFrame();
        }
        
        yield return null;
    }
    public IEnumerator EffectScaleDown(GameObject effect, float scaleDownSpeed, float scaleDownTime)
    {
        EraseEffect eraseEffect = effect.GetComponent<EraseEffect>();
        float delay = 0.0f;

        while (true)
        {
            delay += Time.deltaTime;
            if (delay >= scaleDownTime) break;

            effect.transform.localScale = new Vector3(effect.transform.localScale.x - scaleDownSpeed, effect.transform.localScale.y - scaleDownSpeed, 0.0f);

            yield return new WaitForEndOfFrame();
        }

        eraseEffect.ClearEffect();
        yield return null;
    }
    public IEnumerator EffectAlphaDown(GameObject effect, float alphaDownSpeed)
    {
        EraseEffect eraseEffect = effect.GetComponent<EraseEffect>();
        SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
        float alpha = 1.0f;

        while (true)
        {
            alpha -= alphaDownSpeed;
            if (alpha <= 0.0f) break;

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            yield return new WaitForEndOfFrame();
        }

        eraseEffect.ClearEffect();
        yield return null;
    }

    #endregion

    #region 적 이동 함수

    public IEnumerator EnemyMove(Vector3 targetPosition, float moveTime)
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", targetPosition, "easetype", iTween.EaseType.easeOutQuad, "time", moveTime));
        StartCoroutine(EnemySpriteSet(targetPosition.x, transform.position.x, moveTime));

        yield return null;
    }
    public IEnumerator EnemyRandomMove(float posXMin, float posXMax, float posYMin, float posYMax, float moveTime)
    {
        Vector3 randomPosition = new Vector3(Random.Range(posXMin, posXMax), Random.Range(posYMin, posYMax), 0.0f);

        iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", moveTime));
        StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, moveTime));

        yield return null;
    }

    #endregion

    #endregion

    #region 레이저 발사 및 회전 예제

    // public IEnumerator Stage9()
    // {
    //     StartCoroutine(Stage9Pattern1());
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage9Pattern1()
    // {
    //     float rotateAngle = 0.0f;
    //     Vector2 currentPosition;
    //     Vector2 playerPosition;
    // 
    //     while (true)
    //     {
    //         currentPosition = transform.position;
    //         playerPosition = playerObject.transform.position;
    // 
    //         StartCoroutine(Stage9EnemyMove());
    // 
    //         for (int i = 0; i < 2; i++)
    //         {
    //             rotateAngle = (45.0f * i);
    // 
    //             StartCoroutine(Stage9Pattern1Attack(currentPosition, playerPosition, rotateAngle, (i.Equals(0) ? 1 : -1)));
    // 
    //             yield return new WaitForSeconds(0.75f);
    //         }
    //     }
    // }
    // public IEnumerator Stage9EnemyMove()
    // {
    //     StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.25f));
    // 
    //     yield return null;
    // }
    // public IEnumerator Stage9Pattern1Attack(Vector2 currentPosition, Vector2 playerPosition, float rotateAngle, int rotateDirection)
    // {
    //     for (int i = 0; i < 8; i++)
    //     {
    //         // 레이저 이펙트
    //         StartCoroutine(CreateBulletFireEffect(302, 0.5f, 0.2f, 0.4f, currentPosition));
    // 
    //         // 레이저 발사 (하늘색 레이저) (곡선 이동)
    //         for (int j = 0; j < 16; j++)
    //         {
    //             if (bulletManager.bulletPool.Count > 0)
    //             {
    //                 GameObject bullet = bulletManager.bulletPool.Dequeue();
    //                 bullet.SetActive(true);
    //                 ClearChild(bullet);
    //                 bullet.transform.position = currentPosition;
    //                 bullet.gameObject.tag = "BULLET_ENEMY";
    //                 bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
    //                 bullet.transform.SetParent(enemyBulletTemp1);
    //                 if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //                 if (!bullet.GetComponent<BoxCollider2D>()) bullet.AddComponent<BoxCollider2D>();
    //                 if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //                 if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //                 if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //                 SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //                 BoxCollider2D boxCollider2D = bullet.GetComponent<BoxCollider2D>();
    //                 InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //                 MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //                 switch (i)
    //                 {
    //                     case 0:
    //                         spriteRenderer.sprite = spriteCollection[446];
    //                         boxCollider2D.size = new Vector2(0.48f, 0.05f);
    //                         boxCollider2D.offset = new Vector2(0.04f, 0.0f);
    //                         bullet.transform.localScale = new Vector3(0.6f, 1.8f, 0.0f);
    //                         break;
    //                     case 1:
    //                         spriteRenderer.sprite = spriteCollection[430];
    //                         boxCollider2D.size = new Vector2(0.56f, 0.05f);
    //                         bullet.transform.localScale = new Vector3(0.8f, 1.8f, 0.0f);
    //                         break;
    //                     case 6:
    //                         spriteRenderer.sprite = spriteCollection[398];
    //                         boxCollider2D.size = new Vector2(0.56f, 0.05f);
    //                         bullet.transform.localScale = new Vector3(0.8f, 1.8f, 0.0f);
    //                         break;
    //                     case 7:
    //                         spriteRenderer.sprite = spriteCollection[382];
    //                         boxCollider2D.size = new Vector2(0.48f, 0.05f);
    //                         boxCollider2D.offset = new Vector2(-0.04f, 0.0f);
    //                         bullet.transform.localScale = new Vector3(0.6f, 1.8f, 0.0f);
    //                         break;
    //                     default:
    //                         spriteRenderer.sprite = spriteCollection[414];
    //                         boxCollider2D.size = new Vector2(0.24f, 0.05f);
    //                         bullet.transform.localScale = new Vector3(2.0f, 1.8f, 0.0f);
    //                         break;
    //                 }
    //                 spriteRenderer.sortingOrder = 3;
    //                 boxCollider2D.isTrigger = true;
    //                 boxCollider2D.enabled = false;
    //                 initializeBullet.bulletType = BulletType.BULLETTYPE_LASER_MOVE;
    //                 initializeBullet.bulletObject = bullet.gameObject;
    //                 initializeBullet.targetObject = playerObject;
    //                 initializeBullet.isGrazed = false;
    //                 movingBullet.bulletMoveSpeed = 4.0f;
    //                 movingBullet.bulletRotateLimit = 1.0f;
    //                 movingBullet.bulletRotateSpeed = rotateDirection.Equals(1) ? 60.0f : -60.0f;
    //                 movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //                 movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
    //                 movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(playerPosition);
    //                 float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //                 movingBullet.ChangeRotateAngle(angle - 90.0f + (22.5f * j) + rotateAngle);
    //             }
    //             else AddBulletPool();
    //         }
    // 
    //         yield return new WaitForSeconds(0.06f);
    //     }
    // }

    #endregion

}