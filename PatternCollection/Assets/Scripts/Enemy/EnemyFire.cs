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
    private GameManager gameManager;
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
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        bulletManager = GameObject.Find("BulletManager").transform.GetChild(0).GetComponent<BulletManager>();
        effectManager = GameObject.Find("EffectManager").GetComponent<BulletManager>();
        enemyDatabase = GetComponent<EnemyDatabase>();
        playerObject = GameObject.Find("PLAYER");
        body = transform.Find("Body");
    }

    // 적 패턴 변경
    public void Fire(int patternNumber)
    {
        StartCoroutine("Pattern" + patternNumber.ToString());
    }

    #region 패턴 모음집

    #region 패턴 1 (동방홍마향 1스테이지 - 루미아 중간보스 통상)

    public IEnumerator Pattern1()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern1_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern1_1Lunatic()
    {
        while (true)
        {
            StartCoroutine(EnemyMove(new Vector3(2.5f, 2.0f, 0.0f), 1.0f));

            yield return new WaitForSeconds(1.25f);

            for (int i = 0; i < 18; i++)
            {
                StartCoroutine(Pattern1_1LunaticAttack1(20.0f * i));
            }

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(EnemyMove(new Vector3(0.0f, 3.5f, 0.0f), 1.0f));

            yield return new WaitForSeconds(1.25f);

            Vector2 playerPosition = playerObject.transform.position;
            for (int i = 0; i < 10; i++)
            {
                StartCoroutine(Pattern1_1LunaticAttack2(i % 5, playerPosition, -2.0f * i));

                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(0.75f);

            StartCoroutine(EnemyMove(new Vector3(-2.5f, 2.5f, 0.0f), 1.0f));

            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    StartCoroutine(Pattern1_1LunaticAttack3(i, 22.5f * j));
                }

                yield return new WaitForSeconds(0.4f);
            }

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(EnemyMove(new Vector3(0.0f, 3.0f, 0.0f), 1.0f));

            yield return new WaitForSeconds(1.25f);

            for (int i = 0; i < 32; i++)
            {
                StartCoroutine(Pattern1_1LunaticAttack4(Random.Range(50, 64), Random.Range(8.0f, 12.0f)));

                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
    public IEnumerator Pattern1_1LunaticAttack1(float addRotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.25f, 0.5f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.5f);

        // 탄막 1 발사
        for (int i = 0; i < 8; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
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
    public IEnumerator Pattern1_1LunaticAttack2(int spriteNumber, Vector2 playerPosition, float addRotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(304, 0.4f, 0.3f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.3f);

        // 탄막 2 발사
        for (int i = 0; i < 18; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
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
    public IEnumerator Pattern1_1LunaticAttack3(int spriteNumber, float addRotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 3 이펙트
        StartCoroutine(CreateBulletFireEffect(304 - spriteNumber, 0.4f, 0.3f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.3f);

        // 탄막 3 발사
        for (int i = 0; i < 8; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
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
    public IEnumerator Pattern1_1LunaticAttack4(int spriteNumber, float bulletSpeed)
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
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
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

    #endregion

    #region 패턴 2 (동방홍마향 1스테이지 - 루미아 중간보스 스펠 "문라이트 레이")

    public IEnumerator Pattern2()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
            case GameDifficulty.DIFFICULTY_NORMAL:
                gameManager.patternNumber++;
                StartCoroutine(Pattern3());
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern2_1Lunatic());
                StartCoroutine(Pattern2_2Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern2_1Lunatic()
    {
        while (true)
        {
            StartCoroutine(Pattern2_1LunaticAttack());

            yield return new WaitForSeconds(3.0f);

            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 2.0f, 4.25f, 1.0f));

            yield return new WaitForSeconds(1.25f);
        }
    }
    public IEnumerator Pattern2_1LunaticAttack()
    {
        // 레이저 발사
        for (int i = 0; i < 2; i++)
        {
            // 빈 탄막
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
                capsuleCollider2D.enabled = false;
                initializeBullet.bulletNumber = 0;
                initializeBullet.bulletType = BulletType.BULLETTYPE_LASER_HOLD;
                initializeBullet.bulletObject = emptyBullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 0.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                laserBullet.isLaserEnabled = false;
                laserBullet.isLaserDisabled = true;
                laserBullet.laserWidth = 1.8f;
                laserBullet.laserEnableTime = 0.0f;
                laserBullet.laserEnableSpeed = 0.1f;
                laserBullet.laserDisableTime = 1.5f;
                laserBullet.laserDisableSpeed = 0.1f;
                laserBullet.laserRotateState = BulletRotateState.BULLETROTATESTATE_NORMAL;
                laserBullet.laserRotateSpeed = (i.Equals(0) ? 30.0f : -30.0f);
                laserBullet.isLaserRotateEnabled = true;
                laserBullet.isLaserRotateDisabled = true;
            }
            else AddBulletPool();

            // 레이저 탄막
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
                capsuleCollider2D.enabled = false;
                initializeBullet.bulletNumber = 0;
                initializeBullet.bulletType = BulletType.BULLETTYPE_LASER_HOLD;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 0.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                laserBullet.isLaserEnabled = false;
                laserBullet.isLaserDisabled = true;
                laserBullet.laserWidth = 1.8f;
                laserBullet.laserEnableTime = 0.0f;
                laserBullet.laserEnableSpeed = 0.1f;
                laserBullet.laserDisableTime = 1.5f;
                laserBullet.laserDisableSpeed = 0.1f;
                bullet.transform.SetParent(emptyBullet.transform);
                emptyMovingBullet.bulletDestination = emptyInitializeBullet.GetAimedBulletDestination(new Vector2(transform.position.x, transform.position.y - 7.0f));
                float angle = Mathf.Atan2(emptyMovingBullet.bulletDestination.y, emptyMovingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                emptyMovingBullet.ChangeRotateAngle(angle + 90.0f + (i.Equals(0) ? -55.0f : 55.0f));
            }
            else AddBulletPool();
        }

        yield return null;
    }
    public IEnumerator Pattern2_2Lunatic()
    {
        while (true)
        {
            StartCoroutine(Pattern2_2LunaticAttack());

            yield return new WaitForSeconds(0.4f);
        }
    }
    public IEnumerator Pattern2_2LunaticAttack()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(305, 0.5f, 0.25f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.25f);

        // 탄막 1 발사
        for (int i = 0; i < 64; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
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

    #endregion

    #region 패턴 3 (동방홍마향 1스테이지 - 루미아 1통상)

    public IEnumerator Pattern3()
    {
        int randomIndex = Random.Range(1, 4);

        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                StartCoroutine("Pattern3_" + randomIndex.ToString() + "Easy");
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                StartCoroutine("Pattern3_" + randomIndex.ToString() + "Normal");
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                StartCoroutine("Pattern3_" + randomIndex.ToString() + "Hard");
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine("Pattern3_" + randomIndex.ToString() + "Lunatic");
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern3_1Lunatic()
    {
        int fireCount = 0;
        int bulletCount = 8;

        while (true)
        {
            StartCoroutine(Pattern3_1LunaticAttack(fireCount, bulletCount));
            fireCount++;
            bulletCount += 8;

            if (fireCount.Equals(8))
            {
                fireCount = 0;
                bulletCount = 0;
                StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.5f));
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(Pattern3());
    }
    public IEnumerator Pattern3_1LunaticAttack(int fireCount, int bulletCount)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(299, 0.8f, 0.1f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.1f);

        // 탄막 1 발사 (빨강색, 진홍색 원탄) (조준탄)
        for (int i = 0; i < bulletCount; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.04f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 18 + ((i / 8) % 2), BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 2.0f + (0.8f * (i % 8)) + (0.2f * fireCount),
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, 10.0f * (i / 8) - (5.0f * ((bulletCount - 1) / 8))));
        }

        yield return null;
    }
    public IEnumerator Pattern3_2Lunatic()
    {
        int fireCount = 0;
        float rotateAngle = 0.0f;

        StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 0.75f));

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(Pattern3_2LunaticAttack1(rotateAngle));
            rotateAngle += 3.0f;
        }
        rotateAngle = 0.0f;
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(Pattern3_2LunaticAttack2(fireCount, rotateAngle));
            rotateAngle += 3.33f;
            fireCount++;
        }

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(Pattern3());
    }
    public IEnumerator Pattern3_2LunaticAttack1(float rotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2_1 이펙트
        StartCoroutine(CreateBulletFireEffect(299, 0.5f, 0.5f, 0.25f, bulletFirePosition));

        yield return new WaitForSeconds(0.5f);

        // 탄막 2_1 발사 (빨강색 쌀탄) (조준탄)
        for (int i = 0; i < 25; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                0.025f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 51, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 3.0f + (0.2f * i),
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, -6.0f + rotateAngle));
        }

        yield return null;
    }
    public IEnumerator Pattern3_2LunaticAttack2(int fireCount, float rotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2_2 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.5f, 0.5f, 0.25f, bulletFirePosition));

        yield return new WaitForSeconds(0.5f);

        // 탄막 2_2 발사 (파란색 쌀탄) (조준탄)
        for (int i = 0; i < 36; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                0.025f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 54, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 4.0f + (2.0f * fireCount),
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, (10.0f * i) + rotateAngle));
        }

        yield return null;
    }
    public IEnumerator Pattern3_3Lunatic()
    {
        float rotateAngle = 0.0f;
        
        for (int i = 0; i < 18; i++)
        {
            StartCoroutine(Pattern3_3LunaticAttack(rotateAngle));
            StartCoroutine(Pattern3_3LunaticAttack(-rotateAngle));
            rotateAngle += 5.0f;

            yield return new WaitForSeconds(0.04f);
        }

        StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 0.75f));

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(Pattern3());
    }
    public IEnumerator Pattern3_3LunaticAttack(float rotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 3 이펙트
        StartCoroutine(CreateBulletFireEffect(303, 0.6f, 0.3f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.3f);

        // 탄막 3 발사 (연두색 테두리 원탄) (조준탄)
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.03f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, (i % 2).Equals(0) ? 43 : 44, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 3.25f + (0.25f * i),
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_LIMIT, (i % 2).Equals(0) ? 30.0f : -30.0f, 1.0f,
                3, playerObject.transform.position, rotateAngle));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 4 (동방홍마향 1스테이지 - 루미아 1스펠 "나이트 버드" 리메이크)

    public IEnumerator Pattern4()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern4_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern4_1Lunatic()
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
                StartCoroutine(Pattern4_1LunaticAttack(targetPosition, rotateAngle, rotateDirection));
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
    public IEnumerator Pattern4_1LunaticAttack(Vector2 targetPosition, float rotateAngle, int rotateDirection)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(rotateDirection.Equals(1) ? 301 : 302, 0.6f, 0.1f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < (rotateDirection.Equals(1) ? 3 : 4); i++)
        {
            // 탄막 1, 2 발사 (파란색 / 하늘색 테두리 원탄) (조준 방사탄)
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), rotateDirection.Equals(0) ? enemyBulletTemp1 : enemyBulletTemp2, 1, 0.03f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, rotateDirection.Equals(1) ? 39 : 41, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 2.0f + (0.25f * i),
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, targetPosition, (rotateDirection.Equals(1) ? -30.0f + rotateAngle : 30.0f - rotateAngle) + (rotateDirection.Equals(1) ? (-1.0f * i) : (1.0f * i))));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 5 (동방홍마향 1스테이지 - 루미아 2통상)

    public IEnumerator Pattern5()
    {
        int randomIndex = Random.Range(1, 4);
        
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                StartCoroutine("Pattern5_" + randomIndex.ToString() + "Easy");
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                StartCoroutine("Pattern5_" + randomIndex.ToString() + "Normal");
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                StartCoroutine("Pattern5_" + randomIndex.ToString() + "Hard");
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine("Pattern5_" + randomIndex.ToString() + "Lunatic");
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern5_1Lunatic()
    {
        float rotateAngle = 0.0f;

        StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.5f));

        for (int i = 0; i < 7; i++)
        {
            StartCoroutine(Pattern5_1LunaticAttack1(rotateAngle - 12.0f));
            rotateAngle += 4.0f;
        }
        for (int i = 0; i < 6; i++)
        {
            StartCoroutine(Pattern5_1LunaticAttack2());

            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(4.0f);

        StartCoroutine(Pattern5());
    }
    public IEnumerator Pattern5_1LunaticAttack1(float rotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(303, 0.8f, 0.15f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.15f);

        // 탄막 1 발사 (초록색 테두리 원탄) (조준탄)
        for (int i = 0; i < 5; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.03f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 42, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 4.0f + (0.5f * i),
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, rotateAngle));
        }

        yield return null;
    }
    public IEnumerator Pattern5_1LunaticAttack2()
    {
        // 빈 탄막
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
            initializeBullet.bulletNumber = 0;
            initializeBullet.bulletType = BulletType.BULLETTYPE_LASER_HOLD;
            initializeBullet.bulletObject = emptyBullet.gameObject;
            initializeBullet.targetObject = playerObject;
            initializeBullet.isGrazed = false;
            movingBullet.bulletMoveSpeed = 0.0f;
            movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
            movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
            laserBullet.isLaserEnabled = false;
            laserBullet.isLaserDisabled = true;
            laserBullet.laserWidth = 1.8f;
            laserBullet.laserEnableTime = 1.25f;
            laserBullet.laserEnableSpeed = 0.1f;
            laserBullet.laserDisableTime = 1.75f;
            laserBullet.laserDisableSpeed = 0.1f;
            laserBullet.laserRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
            laserBullet.laserRotateSpeed = 0.0f;
            laserBullet.isLaserRotateEnabled = true;
            laserBullet.isLaserRotateDisabled = true;
        }
        else AddBulletPool();

        // 레이저 탄막
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
            capsuleCollider2D.enabled = false;
            initializeBullet.bulletNumber = 0;
            initializeBullet.bulletType = BulletType.BULLETTYPE_LASER_HOLD;
            initializeBullet.bulletObject = bullet.gameObject;
            initializeBullet.targetObject = playerObject;
            initializeBullet.isGrazed = false;
            movingBullet.bulletMoveSpeed = 0.0f;
            movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
            movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
            laserBullet.isLaserEnabled = false;
            laserBullet.isLaserDisabled = true;
            laserBullet.laserWidth = 1.8f;
            laserBullet.laserEnableTime = 1.25f;
            laserBullet.laserEnableSpeed = 0.1f;
            laserBullet.laserDisableTime = 1.75f;
            laserBullet.laserDisableSpeed = 0.1f;
            bullet.transform.SetParent(emptyBullet.transform);
            emptyMovingBullet.bulletDestination = emptyInitializeBullet.GetAimedBulletDestination();
            float angle = Mathf.Atan2(emptyMovingBullet.bulletDestination.y, emptyMovingBullet.bulletDestination.x) * Mathf.Rad2Deg;
            emptyMovingBullet.ChangeRotateAngle(angle + 90.0f);
        }
        else AddBulletPool();

        yield return null;
    }
    public IEnumerator Pattern5_2Lunatic()
    {
        int fireCount = 0;

        for (int i = 0; i < 8; i++)
        {
            StartCoroutine(Pattern5_2LunaticAttack(0, fireCount));
            StartCoroutine(Pattern5_2LunaticAttack(1, fireCount));
            fireCount++;

            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.0f));

        yield return new WaitForSeconds(1.25f);

        StartCoroutine(Pattern5());
    }
    public IEnumerator Pattern5_2LunaticAttack(int rotateDirection, int fireCount)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(304, 0.65f, 0.2f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.2f);

        // 탄막 2 발사 (노란색 / 주황색 쌀탄) (조준탄)
        for (int i = 0; i < 18; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                0.025f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 62 + (fireCount % 2), BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 4.0f,
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_LIMIT, rotateDirection.Equals(0) ? 30.0f : -30.0f, 1.5f,
                3, playerObject.transform.position, 20.0f * i));
        }

        yield return null;
    }
    public IEnumerator Pattern5_3Lunatic()
    {
        for (int i = 0; i < 24; i++)
        {
            StartCoroutine(Pattern5_3LunaticAttack());

            yield return new WaitForSeconds(0.04f);
        }

        StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.0f));

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(Pattern5());
    }
    public IEnumerator Pattern5_3LunaticAttack()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 3 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.7f, 0.2f, 0.4f, bulletFirePosition));
        
        yield return new WaitForSeconds(0.2f);

        // 탄막 3 발사 (파란색 원탄) (랜덤탄)
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine
                (BulletFire(i, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.04f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 23, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 12.0f,
                0.0f, 0.0f,
                Random.Range(0.4f, 0.6f), Random.Range(3.0f, 4.0f), false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                2, playerObject.transform.position, 0.0f));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 6 (동방홍마향 1스테이지 - 루미아 2스펠 "디마케이션")

    public IEnumerator Pattern6()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern6_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern6_1Lunatic()
    {
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        StartCoroutine(Pattern6_1LunaticAttack(55, 301, 0));
                        StartCoroutine(Pattern6_1LunaticAttack(55, 301, 1));
                        break;
                    case 1:
                        StartCoroutine(Pattern6_1LunaticAttack(59, 303, 0));
                        StartCoroutine(Pattern6_1LunaticAttack(59, 303, 1));
                        break;
                    case 2:
                        StartCoroutine(Pattern6_1LunaticAttack(51, 299, 0));
                        StartCoroutine(Pattern6_1LunaticAttack(51, 299, 1));
                        break;
                    default:
                        break;
                }

                if (i.Equals(2))
                {
                    yield return new WaitForSeconds(1.5f);
                }
                else
                {
                    yield return new WaitForSeconds(1.0f);
                }
            }

            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.25f));

            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(Pattern6_2Lunatic(i));

                if (i.Equals(2))
                {
                    StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.5f));
                }

                yield return new WaitForSeconds(0.75f);
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
    public IEnumerator Pattern6_1LunaticAttack(int spriteNumber, int effectSpriteNumber, int fireCount)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(effectSpriteNumber, 0.65f, 0.2f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.2f);

        // 탄막 1 발사 (파란색 / 연두색 / 빨강색 쌀탄) (회전 방사탄)
        for (int i = 0; i < 64; i++)
        {
            StartCoroutine
                (BulletFire(i, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                0.025f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, spriteNumber, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, fireCount.Equals(0) ? 5.0f : 3.5f,
                0.0f, 0.0f,
                fireCount.Equals(0) ? 0.1f : 0.07f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, (i % 2).Equals(0) ? 10.0f : -10.0f, 3.0f,
                3, playerObject.transform.position, 5.625f * i, 1));
        }

        yield return null;
    }
    public IEnumerator Pattern6_2Lunatic(int rotateDirection)
    {
        float rotateAngle = 0.0f;

        for (int i = 0; i < 30; i++)
        {
            StartCoroutine(Pattern6_2LunaticAttack(rotateAngle));
            rotateAngle = (rotateDirection % 2).Equals(0) ? rotateAngle + 3.0f : rotateAngle - 3.0f;

            yield return new WaitForSeconds(0.015f);
        }
    }
    public IEnumerator Pattern6_2LunaticAttack(float rotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;
        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y - 5.0f);

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.65f, 0.2f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.2f);

        // 탄막 2 발사 (파란색 테두리 원탄) (회전 방사탄)
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp2, 1, 0.03f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 38, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 6.0f + (0.6f * i),
                0.0f, 0.0f,
                0.12f + (0.012f * i), 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, targetPosition, rotateAngle, 2));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 7 (동방홍마향 2스테이지 - 대요정 중간보스 통상)

    public IEnumerator Pattern7()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern7_1Lunatic(0));
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern7_1Lunatic(int fireCount)
    {
        Vector2 targetPosition = playerObject.transform.position;
        float rotateAngle = 0.0f;

        for (int i = 0; i < 75; i++)
        {
            StartCoroutine(Pattern7_1LunaticAttack(targetPosition, fireCount.Equals(0) ? 90 : 83, rotateAngle));
            rotateAngle = fireCount.Equals(0) ? rotateAngle + 6.0f : rotateAngle - 6.0f;

            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 0.75f));

        yield return new WaitForSeconds(1.0f);

        if (fireCount.Equals(0))
        {
            StartCoroutine(Pattern7_1Lunatic(1));
        }
        else
        {
            StartCoroutine(Pattern7_2Lunatic());
        }
    }
    public IEnumerator Pattern7_1LunaticAttack(Vector2 targetPosition, int spriteNumber, float rotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(spriteNumber.Equals(90) ? 303 : 299, 0.6f, 0.3f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.3f);

        // 탄막 1 발사 (연두색 / 빨강색 쿠나이탄) (조준 방사탄)
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                0.02f, 0.055f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, spriteNumber, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 12.0f + (2.0f * i),
                0.0f, 0.0f,
                0.6f + (0.1f * i), 4.0f - (1.0f * i), false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, targetPosition, rotateAngle + (2.0f * i)));
        }

        yield return null;
    }
    public IEnumerator Pattern7_2Lunatic()
    {
        for (int i = 0; i < 24; i++)
        {
            StartCoroutine(Pattern7_2LunaticAttack1());
            StartCoroutine(Pattern7_2LunaticAttack2());

            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.25f));

        yield return new WaitForSeconds(1.75f);

        StartCoroutine(Pattern7());
    }
    public IEnumerator Pattern7_2LunaticAttack1()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.8f, 0.15f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.15f);

        // 탄막 2 발사 (파란색 보석탄) (조준 방사탄)
        for (int i = 0; i < 9; i++)
        {
            Vector2 playerPosition = playerObject.transform.position;

            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                0.02f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 71, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 6.0f,
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, playerPosition, -40.0f + (10.0f * i)));
        }

        yield return null;
    }
    public IEnumerator Pattern7_2LunaticAttack2()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(298, 0.8f, 0.15f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.15f);

        // 탄막 2 발사 (흰색 보석탄) (조준 방사탄)
        for (int i = 0; i < 9; i++)
        {
            Vector2 playerPosition = playerObject.transform.position;

            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp2, 2, 0.0f,
                0.02f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 65, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 9.0f,
                0.0f, 0.0f,
                0.15f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, playerPosition, -40.0f + (10.0f * i), 3));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 8 (동방홍마향 2스테이지 - 치르노 1통상)

    public IEnumerator Pattern8()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern8_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern8_1Lunatic()
    {
        float rotateAngle = 4.0f;

        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(Pattern8_1LunaticAttack1(i, rotateAngle));
            StartCoroutine(Pattern8_1LunaticAttack2(i));
            rotateAngle += 2.0f;

            yield return new WaitForSeconds(1.0f);
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Pattern8_2Lunatic());

        yield return null;
    }
    public IEnumerator Pattern8_1LunaticAttack1(int addBulletAmount, float rotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.6f, 0.3f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.3f);

        // 탄막 1 발사 (파란색 보석탄) (조준 방사탄)
        for (int i = 0; i < 6 + addBulletAmount; i++)
        {
            for (int j = 0; j < 3 + i; j++)
            {
                StartCoroutine
                    (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                    LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                    0.02f, 0.05f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 0.0f,
                    1.0f, 71, BulletType.BULLETTYPE_NORMAL, playerObject,
                    BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 9.0f - (0.5f * i),
                    0.0f, 0.0f,
                    0.0f, 0.0f, false,
                    BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                    3, playerObject.transform.position, (rotateAngle + (rotateAngle * 0.5f * i)) - (rotateAngle * j)));
            }
        }

        yield return null;
    }
    public IEnumerator Pattern8_1LunaticAttack2(int addBulletAmount)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.6f, 0.3f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.3f);

        // 탄막 2 발사 (파란색 원탄) (조준 방사탄)
        for (int i = 0; i < 3 + addBulletAmount; i++)
        {
            float rotateAngle = 0.0f;
            switch (3 + addBulletAmount)
            {
                case 3:
                    rotateAngle = 7.5f;
                    break;
                case 4:
                    rotateAngle = 5.625f;
                    break;
                case 5:
                    rotateAngle = 4.5f;
                    break;
                default:
                    break;
            }

            for (int j = 0; j < 16; j++)
            {
                StartCoroutine
                    (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                    LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp2, 1, 0.04f,
                    0.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 0.0f,
                    1.0f, 23, BulletType.BULLETTYPE_NORMAL, playerObject,
                    BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 4.0f + (1.0f * i),
                    0.0f, 0.0f,
                    0.0f, 0.0f, false,
                    BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                    3, playerObject.transform.position, (22.5f * j) + (addBulletAmount.Equals(1) ? (rotateAngle * i) : -(rotateAngle * i))));
            }
        }

        yield return null;
    }
    public IEnumerator Pattern8_2Lunatic()
    {
        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.5f));
            StartCoroutine(Pattern8_2LunaticAttack1());
            for (int j = 0; j < 3; j++)
            {
                StartCoroutine(Pattern8_2LunaticAttack2(j));

                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitForSeconds(0.8f);
        }

        yield return new WaitForSeconds(0.75f);

        StartCoroutine(Pattern8());

        yield return null;
    }
    public IEnumerator Pattern8_2LunaticAttack1()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 3 이펙트
        StartCoroutine(CreateBulletFireEffect(305, 0.8f, 0.2f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.2f);

        // 탄막 3 발사 (흰색 쌀탄) (방사 후 조준탄)
        for (int i = 0; i < 72; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2"), enemyBulletTemp1, 2, 0.0f,
                0.025f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 49, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 10.0f,
                0.0f, 0.0f,
                0.2f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, 5.0f * i, 4));
        }
        
        yield return null;
    }
    public IEnumerator Pattern8_2LunaticAttack2(int fireCount)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 4 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.8f, 0.2f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.2f);

        // 탄막 4 발사 (파란색 테두리 원탄) (조준 방사탄)
        for (int i = 0; i < 36; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp2, 1, 0.03f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, fireCount.Equals(1) ? 38 : 39, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 6.0f,
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, fireCount.Equals(1) ? (10.0f * i) - 5.0f : 10.0f * i));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 9 (동방홍마향 2스테이지 - 치르노 1스펠 "아이시클 폴", "헤일 스톰")

    public IEnumerator Pattern9()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern9_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern9_1Lunatic()
    {
        int fireCount = 0;
        float positionRotateAngle = 0.0f;
        float rotateAngle = 0.0f;

        while (true)
        {
            for (int i = 0; i < 16; i++)
            {
                StartCoroutine(Pattern9_1LunaticAttack(fireCount, positionRotateAngle, rotateAngle));
                positionRotateAngle += 22.5f;
            }
            fireCount++;
            rotateAngle += 3.0f;
            positionRotateAngle = 0.0f;

            if (fireCount.Equals(8))
            {
                StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 0.9f));
                rotateAngle = 0.0f;
                yield return new WaitForSeconds(1.0f);
            }
            else if (fireCount.Equals(16))
            {
                StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.0f));
                fireCount = 0;
                rotateAngle = 0.0f;
                yield return new WaitForSeconds(1.25f);
            }
            else
            {
                yield return new WaitForSeconds(0.4f);
            }
        }
    }
    public IEnumerator Pattern9_1LunaticAttack(int fireCount, float positionRotateAngle, float rotateAngle)
    {
        Vector2 bulletFirePosition = GetBulletFirePosition(0.2f, positionRotateAngle);

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.4f, 0.4f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.4f);

        // 탄막 1 발사 (연청색 보석탄) (조준 방사탄)
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine
                (BulletFire(fireCount, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                0.02f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 70, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 6.0f + (2.0f * i),
                0.0f, 0.0f,
                0.15f + (0.05f * i), 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, new Vector2(transform.position.x, transform.position.y - 5.0f), 90.0f + positionRotateAngle + rotateAngle, 5));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 10 (동방홍마향 2스테이지 - 치르노 2통상) (제작중)

    public IEnumerator Pattern10()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern10_1Lunatic());
                break;
            default:
                break;
        }
    
        yield return null;
    }
    
    #region Lunatic
    
    public IEnumerator Pattern10_1Lunatic()
    {
        for (int i = 0; i < 12; i++)
        {
            StartCoroutine(Pattern10_1LunaticAttack1());
            StartCoroutine(Pattern10_1LunaticAttack2());

            yield return new WaitForSeconds(0.25f);
        }

        // StartCoroutine(Pattern10_2Lunatic());
        StartCoroutine(Pattern10());
    
        yield return null;
    }
    public IEnumerator Pattern10_1LunaticAttack1()
    {
        Vector2 bulletFirePosition = transform.position;
    
        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.6f, 0.25f, 0.4f, bulletFirePosition));
    
        yield return new WaitForSeconds(0.25f);

        // 탄막 1 발사 (파란색 테두리 원탄) (조준 방사탄)
        for (int i = 0; i < 36; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.03f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 39, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 12.0f,
                0.0f, 0.0f,
                1.0f, 4.0f + (0.8f * (i % 3)), false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, 10.0f * i));
        }

        yield return null;
    }
    public IEnumerator Pattern10_1LunaticAttack2()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(305, 0.6f, 0.25f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.25f);

        // 탄막 2 발사 (흰색 원탄) (조준 방사탄)
        for (int i = 0; i < 36; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp2, 1, 0.04f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 32, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 12.0f,
                0.0f, 0.0f,
                1.0f, 3.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, (10.0f * i) + 5.0f));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 11 (동방홍마향 2스테이지 - 치르노 2스펠 "퍼펙트 프리즈")

    public IEnumerator Pattern11()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern11_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern11_1Lunatic()
    {
        while (true)
        {
            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 2.25f));
            for (int i = 0; i < 48; i++)
            {
                StartCoroutine(Pattern11_1LunaticAttack1());

                yield return new WaitForSeconds(0.04f);
            }

            yield return new WaitForSeconds(1.0f);

            for (int i = 0; i < enemyBulletTemp1.childCount; i++)
            {
                SpriteRenderer spriteRenderer = enemyBulletTemp1.GetChild(i).GetComponent<SpriteRenderer>();
                MovingBullet movingBullet = enemyBulletTemp1.GetChild(i).GetComponent<MovingBullet>();
                spriteRenderer.sprite = spriteCollection[32];
                movingBullet.bulletMoveSpeed = 0.0f;
            }

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 2.0f));
            for (int i = 0; i < 10; i++)
            {
                StartCoroutine(Pattern11_1LunaticAttack2());

                yield return new WaitForSeconds(0.15f);
            }

            yield return new WaitForSeconds(1.5f);

            for (int i = 0; i < enemyBulletTemp1.childCount; i++)
            {
                InitializeBullet initializeBullet = enemyBulletTemp1.GetChild(i).GetComponent<InitializeBullet>();
                MovingBullet movingBullet = enemyBulletTemp1.GetChild(i).GetComponent<MovingBullet>();
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
                movingBullet.bulletAccelerationMoveSpeed = 0.02f;
                movingBullet.bulletAccelerationMoveSpeedMax = 6.0f;
                movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f);
            }

            yield return new WaitForSeconds(4.0f);
        }
    } 
    public IEnumerator Pattern11_1LunaticAttack1()
    {
        Vector2 bulletFirePosition = transform.position;
        int bulletSprite = Random.Range(18, 32);
        int effectSprite = 0;

        switch (bulletSprite)
        {
            case 18:
            case 19:
                effectSprite = 299;
                break;
            case 20:
            case 21:
                effectSprite = 300;
                break;
            case 22:
            case 23:
                effectSprite = 301;
                break;
            case 24:
            case 25:
                effectSprite = 302;
                break;
            case 26:
            case 27:
            case 28:
                effectSprite = 303;
                break;
            case 29:
            case 30:
            case 31:
                effectSprite = 304;
                break;
        }

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(effectSprite, 0.7f, 0.2f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.2f);

        // 탄막 1 발사 (무작위 색상 원탄) (랜덤탄)
        for (int i = 0; i < 16; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.04f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, bulletSprite, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, Random.Range(1.5f, 7.0f),
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                2, playerObject.transform.position, 0.0f));
        }

        yield return null;
    }
    public IEnumerator Pattern11_1LunaticAttack2()
    {
        Vector2 bulletFirePosition = transform.position;
        
        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.7f, 0.2f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.2f);

        // 탄막 2 발사 (파란색 원탄) (조준 방사탄)
        for (int i = 0; i < 16; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.04f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 23, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 7.0f,
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, 22.5f * i));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 12 (동방홍마향 2스테이지 - 치르노 3스펠 "다이아몬드 블리자드" 리메이크)

    public IEnumerator Pattern12()
    {
        StartCoroutine(Pattern12_EnemyMove());

        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                gameManager.patternNumber++;
                StartCoroutine(Pattern13());
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern12_1Lunatic());
                StartCoroutine(Pattern12_2Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }
    public IEnumerator Pattern12_EnemyMove()
    {
        while (true)
        {
            StartCoroutine(EnemyRandomMove(-1.0f, 1.0f, 2.0f, 3.0f, 2.25f));

            yield return new WaitForSeconds(2.5f);
        }
    }

    #region Lunatic

    public IEnumerator Pattern12_1Lunatic()
    {
        while (true)
        {
            for (int i = 0; i < 9; i++)
            {
                StartCoroutine(Pattern12_1LunaticAttack());
            }

            yield return new WaitForSeconds(0.06f);
        }
    }
    public IEnumerator Pattern12_1LunaticAttack()
    {
        Vector2 bulletFirePosition = GetRandomBulletFirePosition(1.5f);

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(302, 0.4f, 0.15f, 0.3f, bulletFirePosition, 4.0f, 4.0f));

        yield return new WaitForSeconds(0.15f);

        // 탄막 1 발사 (하늘색 보석탄) (랜덤탄)
        StartCoroutine
            (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
            LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
            0.02f, 0.05f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, 72, BulletType.BULLETTYPE_NORMAL, playerObject,
            BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 14.0f,
            0.0f, 0.0f,
            0.5f, Random.Range(2.0f, 4.0f), false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            2, playerObject.transform.position, 0.0f));

        yield return null;
    }
    public IEnumerator Pattern12_2Lunatic()
    {
        float rotateAngle = 0.0f;

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                StartCoroutine(Pattern12_2LunaticAttack(i, rotateAngle));
            }
            
            rotateAngle += 12.0f;
            if (rotateAngle >= 360.0f) rotateAngle = 0.0f;

            yield return new WaitForSeconds(0.08f);
        }
    }
    public IEnumerator Pattern12_2LunaticAttack(int bulletNumber, float rotateAngle)
    {
        Vector2 bulletFirePosition = GetBulletFirePosition(2.0f, (90.0f * bulletNumber) + rotateAngle);

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.4f, 0.3f, 0.3f, bulletFirePosition, 8.0f, 8.0f));

        yield return new WaitForSeconds(0.3f);

        // 탄막 2 발사 (파란색 테두리 원탄) (방사탄)
        StartCoroutine
            (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
            LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.03f,
            0.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, 39, BulletType.BULLETTYPE_NORMAL, playerObject,
            BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 9.0f,
            0.0f, 0.0f,
            0.3f, 3.0f, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            4, new Vector2(transform.position.x, transform.position.y - 5.0f), 90.0f + (90.0f * bulletNumber) + rotateAngle));

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 13 (동방홍마향 3스테이지 - 홍 메이링 중간보스 통상)

    public IEnumerator Pattern13()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern13_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern13_1Lunatic()
    {
        StartCoroutine(EnemyMove(new Vector2(0.0f, 1.0f), 0.8f));

        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            for (int i = 0; i < 8; i++)
            {
                StartCoroutine(Pattern13_1LunaticAttack(i));

                yield return new WaitForSeconds(0.15f);
            }

            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 0.75f));

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(Pattern13_2Lunatic());
            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.5f));

            yield return new WaitForSeconds(1.75f);

            StartCoroutine(Pattern13_2Lunatic());
            StartCoroutine(Pattern13_3Lunatic());
            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.5f));

            yield return new WaitForSeconds(1.75f);
        }
    }
    public IEnumerator Pattern13_1LunaticAttack(int fireCount)
    {
        Vector2 bulletFirePosition = GetBulletFirePosition(fireCount < 4 ? 0.8f - (0.2f * fireCount) : (0.3f * fireCount), -90.0f + Random.Range(-20.0f, 20.0f));

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.6f, 0.35f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.35f);

        // 탄막 1 발사 (파란색 원탄) (랜덤탄)
        for (int i = 0; i < 32; i++)
        {
            StartCoroutine
            (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
            LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.04f,
            0.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, 23, BulletType.BULLETTYPE_NORMAL, playerObject,
            BulletSpeedState.BULLETSPEEDSTATE_NORMAL, Random.Range(4.0f, 6.0f),
            0.0f, 0.0f,
            0.0f, 0.0f, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            2, playerObject.transform.position, 0.0f));
        }

        yield return null;
    }
    public IEnumerator Pattern13_2Lunatic()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2 bulletFirePosition = GetBulletFirePosition(1.0f, Random.Range(0.0f, 360.0f));
            Vector2 targetPosition = new Vector2(Random.Range(-2.5f, 2.5f), playerObject.transform.position.y);

            for (int j = 0; j < 3; j++)
            {
                StartCoroutine(Pattern13_2LunaticAttack(j, bulletFirePosition, targetPosition));
            }

            yield return new WaitForSeconds(0.3f);
        }

        yield return null;
    }
    public IEnumerator Pattern13_2LunaticAttack(int fireCount, Vector2 bulletFirePosition, Vector2 targetPosition)
    {
        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(299, 0.6f, 0.35f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.35f);

        // 탄막 2 발사 (빨강색 쌀탄) (랜덤탄)
        for (int i = 0; i < 36; i++)
        {
            StartCoroutine
            (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
            LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
            0.025f, 0.05f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, 66, BulletType.BULLETTYPE_NORMAL, playerObject,
            BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 4.0f + (1.0f * fireCount),
            0.0f, 0.0f,
            0.0f, 0.0f, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            4, targetPosition, 10.0f * i));
        }

        yield return null;
    }
    public IEnumerator Pattern13_3Lunatic()
    {
        StartCoroutine(Pattern13_3LunaticAttack());

        yield return null;
    }
    public IEnumerator Pattern13_3LunaticAttack()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 3 이펙트
        StartCoroutine(CreateBulletFireEffect(299, 0.65f, 0.3f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.3f);

        // 탄막 3 발사 (빨강색 테두리 원탄) (방사탄)
        for (int i = 0; i < 90; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.03f,
                0.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 35, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 2.5f,
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, 4.0f * i));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 14 (동방홍마향 3스테이지 - 홍 메이링 중간보스 스펠 "방화현란", "세라기넬라 9")

    public IEnumerator Pattern14()
    {
        StartCoroutine(EnemyMove(new Vector2(0.0f, 1.0f), 0.8f));

        yield return new WaitForSeconds(1.0f);

        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern14_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern14_1Lunatic()
    {
        int fireCount = 0;
        float rotateAngle = 0.0f;
        float extraRotateAngle = 0.0f;

        StartCoroutine(Pattern14_1LunaticAttack1());

        while (true)
        {
            for (int i = 0; i < 12; i++)
            {
                StartCoroutine(Pattern14_1LunaticAttack2((i % 2).Equals(0) ? fireCount : 5 - fireCount,
                    60 * (i / 2) + ((i % 2).Equals(0) ? 1.5f : -1.5f) + 22.5f, (i % 2).Equals(0) ? rotateAngle : -rotateAngle, extraRotateAngle));
            }

            if (fireCount >= 3)
            {
                rotateAngle -= 9.5f;
            }
            else
            {
                rotateAngle += 9.5f;
            }
            fireCount++;
            extraRotateAngle += 1.0f;

            if (fireCount.Equals(6)) fireCount = 0;
            if (extraRotateAngle >= 360.0f) extraRotateAngle = 0.0f;

            yield return new WaitForSeconds(0.08f);
        }
    }
    public IEnumerator Pattern14_1LunaticAttack1()
    {
        Vector2 bulletFirePosition = transform.position;

        while (true)
        {
            // 탄막 1 이펙트
            StartCoroutine(CreateBulletFireEffect(299, 0.3f, 0.3f, 0.3f, bulletFirePosition, 0.7f, 0.7f));

            yield return new WaitForSeconds(0.3f);

            // 탄막 1 발사 (빨강색 쌀탄) (조준 방사탄)
            for (int i = 0; i < 32; i++)
            {
                StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                0.025f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 67, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 4.0f,
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                3, playerObject.transform.position, 11.25f * i));
            }

            yield return new WaitForSeconds(0.7f);
        }
    }
    public IEnumerator Pattern14_1LunaticAttack2(int fireCount, float initRotateAngle, float addRotateAngle, float extraRotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(304, 0.3f, 0.3f, 0.3f, bulletFirePosition, 0.7f, 0.7f));

        yield return new WaitForSeconds(0.3f);

        // 탄막 2 발사 (노란색 쌀탄) (고정탄)
        StartCoroutine
            (BulletFire(fireCount, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
            LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp2, 2, 0.0f,
            0.025f, 0.05f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, 77, BulletType.BULLETTYPE_NORMAL, playerObject,
            BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 6.0f,
            0.0f, 0.0f,
            0.08f, 0.0f, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            4, new Vector2(transform.position.x, transform.position.y - 5.0f), initRotateAngle + addRotateAngle + extraRotateAngle, 6));

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 15 (동방홍마향 3스테이지 - 홍 메이링 1통상)

    public IEnumerator Pattern15()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern15_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern15_1Lunatic()
    {
        float rotateAngle = 0.0f;

        while (true)
        {
            Vector2 targetPosition = playerObject.transform.position;

            for (int i = 0; i < 24; i++)
            {
                if (i.Equals(16))
                {
                    StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.25f));
                }

                if (i < 16)
                {
                    StartCoroutine(Pattern15_1LunaticAttack1(false, targetPosition, rotateAngle));
                }
                else
                {
                    StartCoroutine(Pattern15_1LunaticAttack1(true, targetPosition, rotateAngle));
                    if (i.Equals(23))
                    {
                        StartCoroutine(Pattern15_1LunaticAttack2());
                    }
                }

                rotateAngle += 4.0f;

                yield return new WaitForSeconds(0.2f - (0.0075f * i));
            }
            rotateAngle = 0.0f;

            yield return new WaitForSeconds(1.25f);
        }
    }
    public IEnumerator Pattern15_1LunaticAttack1(bool isRandomBullet, Vector2 targetPosition, float rotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(299, 0.7f, 0.2f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.2f);

        // 탄막 1 발사 (빨강색 보석탄) (조준 방사탄, 랜덤탄)
        for (int i = 0; i < 32; i++)
        {
            StartCoroutine
            (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
            LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
            0.02f, 0.05f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, 67, BulletType.BULLETTYPE_NORMAL, playerObject,
            BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, isRandomBullet.Equals(false) ? 5.0f : Random.Range(4.5f, 6.5f),
            0.0f, 0.0f,
            0.008f, 4.0f, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            isRandomBullet.Equals(false) ? 4 : 2, targetPosition, isRandomBullet.Equals(false) ? (11.25f * i) + rotateAngle : 0.0f));
        }

        yield return null;
    }
    public IEnumerator Pattern15_1LunaticAttack2()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(299, 0.7f, 0.2f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.2f);

        // 탄막 2 발사 (빨강색 테두리 원탄) (조준 방사탄)
        for (int i = 0; i < 48; i++)
        {
            StartCoroutine
            (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
            LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp2, 1, 0.03f,
            0.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, 34, BulletType.BULLETTYPE_NORMAL, playerObject,
            BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 7.0f,
            0.0f, 0.0f,
            0.0f, 0.0f, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            3, playerObject.transform.position, 7.5f * i));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 16 (동방홍마향 3스테이지 - 홍 메이링 1스펠 "채홍의 풍령" 리메이크)

    public IEnumerator Pattern16()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern16_1Lunatic());
                StartCoroutine(Pattern16_2Lunatic());
                StartCoroutine(Pattern16_3Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern16_1Lunatic()
    {
        int fireCount = 0;
        int rotateDirection = 1;
        float rotateAngle = 0.0f;

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                StartCoroutine(Pattern16_1LunaticAttack(i, rotateAngle));
            }
            fireCount++;
            if (rotateDirection.Equals(1)) rotateAngle += 7.0f;
            else rotateAngle -= 7.0f;

            if (fireCount >= 40)
            {
                fireCount = 0;
                rotateDirection *= -1;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    public IEnumerator Pattern16_1LunaticAttack(int fireDirection, float rotateAngle)
    {
        int spriteNumber = 0;
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(305, 0.5f, 0.2f, 0.3f, bulletFirePosition, 0.8f, 0.8f));

        yield return new WaitForSeconds(0.2f);

        // 탄막 1 발사 (무지개색 보석탄) (고정탄)
        for (int i = 0; i < 7; i++)
        {
            switch (i)
            {
                case 0:
                    spriteNumber = 67;
                    break;
                case 1:
                    spriteNumber = 79;
                    break;
                case 2:
                    spriteNumber = 77;
                    break;
                case 3:
                    spriteNumber = 74;
                    break;
                case 4:
                    spriteNumber = 72;
                    break;
                case 5:
                    spriteNumber = 71;
                    break;
                case 6:
                    spriteNumber = 68;
                    break;
                default:
                    break;
            }

            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
                0.02f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, spriteNumber, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 4.0f,
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, new Vector2(transform.position.x, transform.position.y - 5.0f), (90.0f * fireDirection) + (18.0f - (6.0f * i)) + rotateAngle));
        }

        yield return null;
    }
    public IEnumerator Pattern16_2Lunatic()
    {
        int fireCount = 0;
        int spriteIndex = 0;
        float rotateAngle = 0.0f;

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                StartCoroutine(Pattern16_2LunaticAttack(i, spriteIndex, rotateAngle));
            }
            fireCount++;
            rotateAngle += 4.0f;

            if (rotateAngle >= 360.0f) rotateAngle = 0.0f;
            if (fireCount.Equals(16))
            {
                fireCount = 0;
                spriteIndex++;
                if (spriteIndex.Equals(7)) spriteIndex = 0;
            }

            yield return new WaitForSeconds(0.12f);
        }
    }
    public IEnumerator Pattern16_2LunaticAttack(int fireDirection, int spriteIndex, float rotateAngle)
    {
        int spriteNumber = 0;
        Vector2 bulletFirePosition = transform.position;

        switch (spriteIndex)
        {
            case 0:
                spriteNumber = 67;
                break;
            case 1:
                spriteNumber = 79;
                break;
            case 2:
                spriteNumber = 77;
                break;
            case 3:
                spriteNumber = 74;
                break;
            case 4:
                spriteNumber = 72;
                break;
            case 5:
                spriteNumber = 71;
                break;
            case 6:
                spriteNumber = 68;
                break;
            default:
                break;
        }

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(305, 0.5f, 0.2f, 0.3f, bulletFirePosition, 0.8f, 0.8f));

        yield return new WaitForSeconds(0.2f);

        // 탄막 2 발사 (무지개색 보석탄) (고정탄)
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp2, 2, 0.0f,
                0.02f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, spriteNumber, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 2.5f + (0.5f * i),
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, new Vector2(transform.position.x, transform.position.y - 5.0f), (90.0f * fireDirection) + (1.0f * i) + rotateAngle));
        }

        yield return null;
    }
    public IEnumerator Pattern16_3Lunatic()
    {
        int fireCount = 0;
        float rotateAngle = 0.0f;

        while (true)
        {
            StartCoroutine(Pattern16_3LunaticAttack(fireCount, rotateAngle));
            fireCount++;
            rotateAngle += 2.0f;

            if (rotateAngle >= 360.0f) rotateAngle = 0.0f;
            if (fireCount.Equals(7))
            {
                fireCount = 0;
            }

            yield return new WaitForSeconds(0.75f);
        }
    }
    public IEnumerator Pattern16_3LunaticAttack(int fireCount, float rotateAngle)
    {
        int spriteNumber = 0;
        Vector2 bulletFirePosition = transform.position;

        switch (fireCount)
        {
            case 0:
                spriteNumber = 68;
                break;
            case 1:
                spriteNumber = 71;
                break;
            case 2:
                spriteNumber = 72;
                break;
            case 3:
                spriteNumber = 74;
                break;
            case 4:
                spriteNumber = 77;
                break;
            case 5:
                spriteNumber = 79;
                break;
            case 6:
                spriteNumber = 67;
                break;
            default:
                break;
        }

        // 탄막 3 이펙트
        StartCoroutine(CreateBulletFireEffect(305, 0.5f, 0.2f, 0.3f, bulletFirePosition, 0.8f, 0.8f));

        yield return new WaitForSeconds(0.2f);

        // 탄막 3 발사 (무지개색 보석탄) (원형 방사탄)
        for (int i = 0; i < 30; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp3, 2, 0.0f,
                0.02f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, spriteNumber, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 3.0f,
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, new Vector2(transform.position.x, transform.position.y - 5.0f), (12.0f * i) + rotateAngle));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 17 (동방홍마향 3스테이지 - 홍 메이링 2통상) (자코가 등장하므로 보류)

    public IEnumerator Pattern17()
    {
        gameManager.patternNumber++;
        StartCoroutine(Pattern18());

        // switch (gameManager.gameDifficulty)
        // {
        //     case GameDifficulty.DIFFICULTY_EASY:
        //         break;
        //     case GameDifficulty.DIFFICULTY_NORMAL:
        //         break;
        //     case GameDifficulty.DIFFICULTY_HARD:
        //         break;
        //     case GameDifficulty.DIFFICULTY_LUNATIC:
        //         StartCoroutine(Pattern17_1Lunatic());
        //         break;
        //     default:
        //         break;
        // }

        yield return null;
    }

    #endregion

    #region 패턴 18 (동방홍마향 3스테이지 - 홍 메이링 2스펠 "화상몽갈")

    public IEnumerator Pattern18()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
            case GameDifficulty.DIFFICULTY_NORMAL:
                gameManager.patternNumber++;
                StartCoroutine(Pattern19());
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern18_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern18_1Lunatic()
    {
        int fireCount = 0;
        float rotateAngle = 0.0f;

        StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.25f));

        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                StartCoroutine(Pattern18_1LunaticAttack(i, fireCount, rotateAngle));
            }
            fireCount++;
            rotateAngle += 9.0f;

            if (rotateAngle >= 360.0f) rotateAngle = 0.0f;

            if (fireCount.Equals(20))
            {
                fireCount = 0;
                yield return new WaitForSeconds(0.75f);

                StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.25f));
            }
            else
            {
                yield return new WaitForSeconds(0.12f);
            }
        }
    }
    public IEnumerator Pattern18_1LunaticAttack(int fireDirection, int fireCount, float rotateAngle)
    {
        float distance = 0.0f;
        switch (fireCount % 4)
        {
            case 0:
                distance = 1.6f;
                break;
            case 1:
            case 3:
                distance = 1.2f;
                break;
            case 2:
                distance = 0.8f;
                break;
        }

        Vector2 bulletFirePosition = GetBulletFirePosition(distance, (72.0f * (fireDirection % 5)) + rotateAngle);

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.4f, 0.2f, 0.3f, bulletFirePosition, 8.0f, 8.0f));

        yield return new WaitForSeconds(0.2f);

        // 탄막 1 발사 (파란색 원탄) (랜덤탄)
        for (int i = 0; i < 6; i++)
        {
            StartCoroutine
            (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
            LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 1, 0.04f,
            0.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, 23, BulletType.BULLETTYPE_NORMAL, playerObject,
            BulletSpeedState.BULLETSPEEDSTATE_NORMAL, Random.Range(3.0f, 5.0f),
            0.0f, 0.0f,
            0.0f, 0.0f, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            2, playerObject.transform.position, 0.0f));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 19 (동방홍마향 3스테이지 - 홍 메이링 3통상)

    public IEnumerator Pattern19()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern19_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern19_1Lunatic()
    {
        while (true)
        {
            for (int i = 0; i < 36; i++)
            {
                StartCoroutine(Pattern19_1LunaticAttack(0, 10.0f * i));
            }

            yield return new WaitForSeconds(1.25f);

            for (int i = 0; i < 36; i++)
            {
                StartCoroutine(Pattern19_1LunaticAttack(1, 10.0f * i));
            }

            yield return new WaitForSeconds(0.25f);

            StartCoroutine(Pattern19_2Lunatic());

            yield return new WaitForSeconds(3.5f);
        }
    }
    public IEnumerator Pattern19_1LunaticAttack(int rotateDirection, float rotateAngle)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(rotateDirection.Equals(0) ? 301 : 299, 0.3f, 0.3f, 0.3f, bulletFirePosition, 7.0f, 7.0f));

        yield return new WaitForSeconds(0.3f);

        // 탄막 1 발사 (파란색, 빨강색 쿠나이탄) (방사탄)
        for (int i = 0; i < 6; i++)
        {
            StartCoroutine
                (BulletFire(rotateDirection, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2"), rotateDirection.Equals(0) ? enemyBulletTemp1 : enemyBulletTemp2, 2, 0.0f,
                0.02f, 0.055f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, rotateDirection.Equals(0) ? 87 : 82, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 5.0f + (1.0f * i),
                0.0f, 0.0f,
                0.1f + (0.02f * i), 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, new Vector2(transform.position.x, transform.position.y - 5.0f), rotateAngle, 7));
        }

        yield return null;
    }
    public IEnumerator Pattern19_2Lunatic()
    {
        for (int i = 0; i < 8; i++)
        {
            StartCoroutine(Pattern19_2LunaticAttack());

            yield return new WaitForSeconds(0.06f);
        }

        StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 1.0f));

        yield return null;
    }
    public IEnumerator Pattern19_2LunaticAttack()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.3f, 0.3f, 0.3f, bulletFirePosition, 7.0f, 7.0f));

        yield return new WaitForSeconds(0.3f);

        // 탄막 2 발사 (파란색 쌀탄) (랜덤탄, 중력 적용)
        for (int i = 0; i < 16; i++)
        {
            StartCoroutine
                (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2"), enemyBulletTemp3, 2, 0.0f,
                0.025f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 55, BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 0.0f,
                0.0f, 0.0f,
                0.0f, 0.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                4, new Vector2(transform.position.x + Random.Range(-1.5f, 1.5f), transform.position.y + 5.0f), 0.0f, 0,
                false, 0, true, Random.Range(100.0f, 200.0f), 0.2f, true));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 20 (동방홍마향 3스테이지 - 홍 메이링 3스펠 "채우", "채광난무")

    public IEnumerator Pattern20()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern20_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern20_1Lunatic()
    {
        while (true)
        {
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    StartCoroutine(Pattern20_1LunaticAttack(i / 8, i, j, 11.25f * i));
                }

                yield return new WaitForSeconds(0.00625f);
            }

            yield return new WaitForSeconds(0.8f);

            StartCoroutine(EnemyRandomMove(-1.5f, 1.5f, 1.5f, 3.0f, 0.75f));

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(Pattern20_2Lunatic());

            yield return new WaitForSeconds(1.25f);
        }
    }
    public IEnumerator Pattern20_1LunaticAttack(int spriteIndex, int fireCount, int fireDirection, float rotateAngle)
    {
        int spriteNumber = 0;
        Vector2 bulletFirePosition = GetBulletFirePosition(0.8f, (90 * fireDirection) + rotateAngle);

        switch (spriteIndex)
        {
            case 0:
                spriteNumber = 67;
                break;
            case 1:
                spriteNumber = 79;
                break;
            case 2:
                spriteNumber = 77;
                break;
            case 3:
                spriteNumber = 74;
                break;
            case 4:
                spriteNumber = 72;
                break;
            case 5:
                spriteNumber = 71;
                break;
            case 6:
                spriteNumber = 68;
                break;
            default:
                break;
        }

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(305, 0.8f, 0.15f, 0.4f, bulletFirePosition, 8.0f, 8.0f));

        yield return new WaitForSeconds(0.15f);

        // 탄막 1 발사 (무지개색 쌀탄) (고정탄, 중력 적용)
        StartCoroutine
            (BulletFire(0, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
            LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2"), enemyBulletTemp1, 2, 0.0f,
            0.025f, 0.05f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, spriteNumber, BulletType.BULLETTYPE_NORMAL, playerObject,
            BulletSpeedState.BULLETSPEEDSTATE_NORMAL, 0.0f,
            0.0f, 0.0f,
            0.0f, 0.0f, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            4, GetBulletFirePosition(1.2f, (90 * fireDirection) + rotateAngle), 0.0f, 0,
            false, 0, true, 120.0f, 0.2f, true));

        yield return null;
    }
    public IEnumerator Pattern20_2Lunatic()
    {
        for (int i = 0; i < 24; i++)
        {
            StartCoroutine(Pattern20_2LunaticAttack());

            yield return new WaitForSeconds(0.06f);
        }

        yield return null;
    }
    public IEnumerator Pattern20_2LunaticAttack()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(305, 0.8f, 0.15f, 0.4f, bulletFirePosition, 8.0f, 8.0f));

        yield return new WaitForSeconds(0.15f);

        // 탄막 2 발사 (무작위 색상 쌀탄) (랜덤 회전탄)
        for (int i = 0; i < 12; i++)
        {
            StartCoroutine
            (BulletFire(i, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
            LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2"), enemyBulletTemp2, 2, 0.0f,
            0.025f, 0.05f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, Random.Range(50, 64), BulletType.BULLETTYPE_NORMAL, playerObject,
            BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, Random.Range(6.0f, 9.0f),
            0.0f, 0.0f,
            0.5f, 2.0f, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            2, playerObject.transform.position, 0.0f, 8));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 21 (동방홍마향 3스테이지 - 홍 메이링 4스펠 "극채태풍")

    public IEnumerator Pattern21()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                gameManager.patternNumber++;
                StartCoroutine(Pattern22());
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern21_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern21_1Lunatic()
    {
        StartCoroutine(EnemyMove(new Vector2(0.0f, 1.5f), 0.75f));

        yield return new WaitForSeconds(0.8f);

        while (true)
        {
            StartCoroutine(Pattern21_1LunaticAttack());

            yield return new WaitForSeconds(0.18f);
        }
    }
    public IEnumerator Pattern21_1LunaticAttack()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(305, 0.8f, 0.15f, 0.4f, bulletFirePosition, 8.0f, 8.0f));

        yield return new WaitForSeconds(0.15f);

        // 탄막 1 발사 (랜덤 색상 보석탄) (랜덤 회전탄)
        for (int i = 0; i < 20; i++)
        {
            StartCoroutine
                (BulletFire(i, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2"), enemyBulletTemp1, 2, 0.0f,
                0.02f, 0.05f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, Random.Range(66, 80), BulletType.BULLETTYPE_NORMAL, playerObject,
                BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, Random.Range(12.0f, 16.0f),
                0.0f, 0.0f,
                Random.Range(1.0f, 1.5f), 2.0f, false,
                BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                2, playerObject.transform.position, 0.0f, 9));
        }

        yield return null;
    }

    #endregion

    #endregion

    #region 패턴 22 (동방홍마향 4스테이지 - 마도서 지대)

    public IEnumerator Pattern22()
    {
        switch (gameManager.gameDifficulty)
        {
            case GameDifficulty.DIFFICULTY_EASY:
                break;
            case GameDifficulty.DIFFICULTY_NORMAL:
                break;
            case GameDifficulty.DIFFICULTY_HARD:
                break;
            case GameDifficulty.DIFFICULTY_LUNATIC:
                StartCoroutine(Pattern22_1Lunatic());
                break;
            default:
                break;
        }

        yield return null;
    }

    #region Lunatic

    public IEnumerator Pattern22_1Lunatic()
    {
        Vector2 bulletFirePosition;

        for (int i = 0; i < 6; i++)
        {
            bulletFirePosition = new Vector2(Random.Range(-3.0f, 3.0f), Random.Range(1.0f, 4.0f));
            StartCoroutine(Pattern22_1LunaticAttack(bulletFirePosition));

            yield return new WaitForSeconds(1.0f);
        }

        yield return null;
    }
    public IEnumerator Pattern22_1LunaticAttack(Vector2 bulletFirePosition)
    {
        float rotateAngle = 0.0f;

        while (true)
        {
            // 탄막 1 이펙트
            StartCoroutine(CreateBulletFireEffect(303, 0.6f, 0.25f, 0.4f, bulletFirePosition, 8.0f, 8.0f));

            yield return new WaitForSeconds(0.25f);

            // 탄막 1 발사 (연두색 콩알탄) (고정탄)
            for (int i = 0; i < 32; i++)
            {
                StartCoroutine
                    (BulletFire(i, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                    LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2"), enemyBulletTemp1, 1, 0.02f,
                    0.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 0.0f,
                    1.0f, 10, BulletType.BULLETTYPE_NORMAL, playerObject,
                    BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, i < 16 ? 10.0f : 14.0f,
                    0.0f, 0.0f,
                    1.0f, 4.0f, false,
                    BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                    4, new Vector2(transform.position.x, transform.position.y - 5.0f), (22.5f * i) - 3.5f + rotateAngle));
            }
            // 탄막 2 발사 (연두색 원탄) (고정탄)
            for (int i = 0; i < 32; i++)
            {
                StartCoroutine
                    (BulletFire(i, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
                    LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2"), enemyBulletTemp2, 1, 0.04f,
                    0.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 0.0f,
                    1.0f, 27, BulletType.BULLETTYPE_NORMAL, playerObject,
                    BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, i < 16 ? 10.0f : 14.0f,
                    0.0f, 0.0f,
                    1.0f, 4.0f, false,
                    BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
                    4, new Vector2(transform.position.x, transform.position.y - 5.0f), (22.5f * i) + 3.5f + rotateAngle));
            }

            rotateAngle += 4.0f;
            if (rotateAngle >= 360.0f)
            {
                rotateAngle = 0.0f;
            }

            yield return new WaitForSeconds(0.6f);
        }
    }

    #endregion

    #endregion

    #region 패턴 23 (동방홍마향 4스테이지 - 소악마 중간보스 통상)

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
    //             spriteRenderer.sprite = spriteCollection[(i % 2).Equals(0) ? 103 : 105];
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
    //             movingBullet.ChangeRotateAngle(angle - 90.0f + (22.5f * i) + (angleMultiply.Equals(1) ? rotateAngle : -rotateAngle));
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
    //         rotateMultiply = (rotateMultiply.Equals(1) ? -1 : 1);
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
    //             spriteRenderer.sprite = spriteCollection[rotateMultiply.Equals(1) ? 126 : 127];
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
    //             movingBullet.bulletRotateSpeed = rotateMultiply.Equals(1) ? 45.0f : -45.0f;
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
    //                     (new Vector3(transform.position.x, (i.Equals(0) ? transform.position.y - 10.0f : transform.position.y + 10.0f), 0.0f));
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
    //                     (new Vector3(transform.position.x, (i.Equals(0) ? transform.position.y - 10.0f : transform.position.y + 10.0f), 0.0f));
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

    #region 특정 거리 바깥에서 쏘는 탄 예제

    // public IEnumerator Test(int fireCount, float positionRotateAngle, float rotateAngle)
    // {
    //     Vector2 bulletFirePosition = GetBulletFirePosition(0.2f, positionRotateAngle);
    // 
    //     // 탄막 1 이펙트
    //     StartCoroutine(CreateBulletFireEffect(301, 0.4f, 0.4f, 0.3f, bulletFirePosition));
    // 
    //     yield return new WaitForSeconds(0.4f);
    // 
    //     // 탄막 1 발사 (파란색 보석탄) (조준 방사탄)
    //     for (int i = 0; i < 3; i++)
    //     {
    //         StartCoroutine
    //             (BulletFire(fireCount, bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f), "BULLET_ENEMY",
    //             LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1"), enemyBulletTemp1, 2, 0.0f,
    //             0.02f, 0.05f, 0.0f, 0.0f,
    //             0.0f, 0.0f, 0.0f, 0.0f,
    //             1.0f, 70, BulletType.BULLETTYPE_NORMAL, playerObject,
    //             BulletSpeedState.BULLETSPEEDSTATE_DECELERATING, 4.0f + (2.0f * i),
    //             0.0f, 0.0f,
    //             0.1f + (0.05f * i), 0.0f, false,
    //             BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
    //             4, new Vector2(transform.position.x, transform.position.y - 5.0f), 90.0f + positionRotateAngle + rotateAngle, 5));
    //     }
    // 
    //     yield return null;
    // }

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

    #endregion

    #region 기타 함수

    #region 탄막 발사 함수

    // 일반 탄막
    public IEnumerator BulletFire
        (int bulletNumber, Vector2 bulletFirePosition, Vector3 bulletScale, string bulletTag,
        int bulletLayer, Transform bulletParent, int colliderType, float circleColliderRadius,
        float capsuleColliderSizeX, float capsuleColliderSizeY, float capsuleColliderOffsetX, float capsuleColliderOffsetY,
        float boxColliderSizeX, float boxColliderSizeY, float boxColliderOffsetX, float boxColliderOffsetY,
        float spriteAlpha, int spriteNumber, BulletType bulletType, GameObject targetObject,
        BulletSpeedState bulletSpeedState, float bulletMoveSpeed,
        float bulletAccelerationMoveSpeed, float bulletAccelerationMoveSpeedMax,
        float bulletDecelerationMoveSpeed, float bulletDecelerationMoveSpeedMin, bool bulletMoveSpeedLoopBool,
        BulletRotateState bulletRotateState, float bulletRotateSpeed, float bulletRotateLimit,
        int bulletDestinationType, Vector2 targetPosition, float addRotateAngle, int addCustomScript = 0,
        bool isSpriteRotate = false, float spriteRotateSpeed = 0.0f, bool isGravity = false, float velocity = 0.0f, float gravityScale = 0.0f, bool isLookAt = false,
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
                default: case 1:
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
            }
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, spriteAlpha);
            spriteRenderer.sprite = spriteCollection[spriteNumber];
            spriteRenderer.sortingOrder = 3;
            initializeBullet.bulletNumber = bulletNumber;
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
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector2(bullet.transform.position.x, bullet.transform.position.y - 5.0f));
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
            if (isSpriteRotate.Equals(true))
            {
                if (!bullet.GetComponent<ObjectRotate>()) bullet.AddComponent<ObjectRotate>();
                ObjectRotate objectRotate = bullet.GetComponent<ObjectRotate>();
                objectRotate.rotateSpeed = spriteRotateSpeed;
            }
            if (isGravity.Equals(true))
            {
                Rigidbody2D rigidbody2D = bullet.GetComponent<Rigidbody2D>();
                // rigidbody2D.AddForce(bullet.transform.position * velocity);
                rigidbody2D.AddForce(movingBullet.bulletDestination.normalized * velocity);
                rigidbody2D.gravityScale = gravityScale;
                movingBullet.isLookAt = isLookAt;
            }
            // 패턴 별 커스텀 스크립트는 여기에 집어넣기 !! 
            switch (addCustomScript)
            {
                case 1:
                    if (!bullet.GetComponent<Pattern6BulletRotate>()) bullet.AddComponent<Pattern6BulletRotate>();
                    break;
                case 2:
                    if (!bullet.GetComponent<Pattern6BulletAiming>()) bullet.AddComponent<Pattern6BulletAiming>();
                    break;
                case 3:
                    if (!bullet.GetComponent<Pattern7BulletAiming>()) bullet.AddComponent<Pattern7BulletAiming>();
                    break;
                case 4:
                    if (!bullet.GetComponent<Pattern8BulletAiming>()) bullet.AddComponent<Pattern8BulletAiming>();
                    break;
                case 5:
                    if (!bullet.GetComponent<Pattern9BulletRotate>()) bullet.AddComponent<Pattern9BulletRotate>();
                    break;
                case 6:
                    if (!bullet.GetComponent<Pattern14BulletRotate>()) bullet.AddComponent<Pattern14BulletRotate>();
                    break;
                case 7:
                    if (!bullet.GetComponent<Pattern19BulletRotate>()) bullet.AddComponent<Pattern19BulletRotate>();
                    break;
                case 8:
                    if (!bullet.GetComponent<Pattern20BulletRotate>()) bullet.AddComponent<Pattern20BulletRotate>();
                    break;
                case 9:
                    if (!bullet.GetComponent<Pattern21BulletRotate>()) bullet.AddComponent<Pattern21BulletRotate>();
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
        (int bulletNumber, GameObject emptyBullet, Vector2 bulletFirePosition, Vector3 bulletScale, string bulletTag,
        int bulletLayer, Transform bulletParent, int colliderType, float circleColliderRadius,
        float capsuleColliderSizeX, float capsuleColliderSizeY, float capsuleColliderOffsetX, float capsuleColliderOffsetY,
        float boxColliderSizeX, float boxColliderSizeY, float boxColliderOffsetX, float boxColliderOffsetY,
        float spriteAlpha, int spriteNumber, BulletType bulletType, GameObject targetObject,
        BulletSpeedState bulletSpeedState, float bulletMoveSpeed,
        float bulletAccelerationMoveSpeed, float bulletAccelerationMoveSpeedMax,
        float bulletDecelerationMoveSpeed, float bulletDecelerationMoveSpeedMin, bool bulletMoveSpeedLoopBool,
        BulletRotateState bulletRotateState, float bulletRotateSpeed, float bulletRotateLimit,
        int bulletDestinationType, Vector2 targetPosition, float addRotateAngle, int addCustomScript = 0,
        bool isSpriteRotate = false, float spriteRotateSpeed = 0.0f, bool isGravity = false, float velocity = 0.0f, float gravityScale = 0.0f, bool isLookAt = false,
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
                default: case 1:
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
            }
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, spriteAlpha);
            spriteRenderer.sprite = spriteCollection[spriteNumber];
            spriteRenderer.sortingOrder = 3;
            initializeBullet.bulletNumber = bulletNumber;
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
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector2(emptyBullet.transform.position.x, emptyBullet.transform.position.y - 5.0f));
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
            if (isSpriteRotate.Equals(true))
            {
                if (!emptyBullet.GetComponent<ObjectRotate>()) emptyBullet.AddComponent<ObjectRotate>();
                ObjectRotate objectRotate = emptyBullet.GetComponent<ObjectRotate>();
                objectRotate.rotateSpeed = spriteRotateSpeed;
            }
            if (isGravity.Equals(true))
            {
                Rigidbody2D rigidbody2D = emptyBullet.GetComponent<Rigidbody2D>();
                rigidbody2D.velocity = movingBullet.bulletDestination.normalized * velocity;
                rigidbody2D.gravityScale = gravityScale;
                movingBullet.isLookAt = isLookAt;
            }
            // 패턴 별 커스텀 스크립트는 여기에 집어넣기 !! 
            switch (addCustomScript)
            {
                case 1:
                    break;
                default:
                    break;
            }
        }
        else AddBulletPool();

        yield return null;
    }

    #endregion

    #region 탄막 발사 이펙트 생성 함수

    public IEnumerator CreateBulletFireEffect(int spriteNumber, float scaleDownSpeed, float scaleDownTime, float alphaUpSpeed,
        Vector2 effectPosition, float scaleX = 12.0f, float scaleY = 12.0f)
    {
        if (effectManager.bulletPool.Count > 0)
        {
            GameObject effect = effectManager.bulletPool.Dequeue();
            effect.transform.localScale = new Vector3(scaleX, scaleY, 0.0f);
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
    public IEnumerator CreateBulletFireEffect(int spriteNumber, float scaleUpSpeed, float scaleLimit, float alphaUpSpeed, float alphaDownSpeed,
        float alphaRemainTime, Vector2 effectPosition, float scaleX = 12.0f, float scaleY = 12.0f)
    {
        if (effectManager.bulletPool.Count > 0)
        {
            GameObject effect = effectManager.bulletPool.Dequeue();
            effect.transform.localScale = new Vector3(scaleX, scaleY, 0.0f);
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
    public IEnumerator EffectScaleUp(GameObject effect, float scaleUpSpeed, float scaleLimit)
    {
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

    #endregion

    #region 보스 주변 특정 거리의 탄막 스폰 지점 산출 함수

    public Vector2 GetBulletFirePosition(float radius, float rotateAngle)
    {
        float radian = rotateAngle * Mathf.PI / 180;
        
        Vector2 position = new Vector2(transform.position.x + (radius * Mathf.Cos(radian)), transform.position.y + (radius * Mathf.Sin(radian)));

        return position;
    }
    public Vector2 GetRandomBulletFirePosition(float radius)
    {
        float addPositionX, addPositionY;

        addPositionX = Random.Range(-radius, radius);
        addPositionY = Mathf.Sqrt(Mathf.Pow(radius, 2.0f) - Mathf.Pow(addPositionX, 2.0f));

        Vector2 position = new Vector2(transform.position.x + addPositionX, Random.Range(transform.position.y - addPositionY, transform.position.y + addPositionY));

        return position;
    }
    public Vector2 GetRandomBulletFirePosition(float margin, float radius)
    {
        float addPositionX, addPositionY;

        addPositionX = Random.Range(-radius, radius);
        if (Mathf.Abs(addPositionX) < margin)
        {
            addPositionX = addPositionX < 0.0f ? addPositionX - (margin - Mathf.Abs(addPositionX)) : addPositionX + (margin - Mathf.Abs(addPositionX));
        }
        addPositionY = Mathf.Sqrt(Mathf.Pow(radius, 2.0f) - Mathf.Pow(addPositionX, 2.0f));

        Vector2 position = new Vector2(transform.position.x + addPositionX, Random.Range(transform.position.y - addPositionY, transform.position.y + addPositionY));

        return position;
    }
    public Vector2 GetRandomBulletFirePositionMax(float radius)
    {
        float addPositionX, addPositionY;
        int positionMultiply = Random.Range(0, 2);

        addPositionX = Random.Range(-radius, radius);
        addPositionY = Mathf.Sqrt(Mathf.Pow(radius, 2.0f) - Mathf.Pow(addPositionX, 2.0f));
        
        Vector2 position = new Vector2(transform.position.x + addPositionX, (positionMultiply.Equals(0) ? transform.position.y + addPositionY : transform.position.y - addPositionY));

        return position;
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
                if (bullet.transform.GetChild(i).gameObject.activeSelf.Equals(true))
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

    #endregion

}