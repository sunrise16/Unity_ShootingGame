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
//  * (Index 1 ~ 16)    콩알탄 : Circle Collider 2D, Radius 0.02
//  * (Index 17 ~ 32)   기본 원탄 : Circle Collider 2D, Radius 0.04
//  * (Index 33 ~ 48)   테두리 원탄 : Circle Collider 2D, Radius 0.03
//  * (Index 49 ~ 64)   쌀탄 : Capsule Collider 2D, Size X 0.025, Y 0.05, Direction Vertical
//  * (Index 65 ~ 80)   보석탄 : Capsule Collider 2D, Size X 0.02, Y 0.05, Direction Vertical
//  * (Index 81 ~ 96)   쿠나이탄 : Capsule Collider 2D, Size X 0.02, Y 0.055, Direction Vertical
//  * (Index 97 ~ 112)  쐐기탄 : Capsule Collider 2D, Size X 0.04, Y 0.06, Direction Vertical
//  * (Index 113 ~ 128) 부적탄 : Box Collider 2D, Size X 0.025, Y 0.05
//  * (Index 129 ~ 144) 총알탄 : Box Collider 2D, Size X 0.02, Y 0.06
//  * (Index 145 ~ 160) 감주탄 : Capsule Collider 2D, Size X 0.02, Y 0.07, Direction Vertical
//  * (Index 161 ~ 176) 소형 별탄 : Circle Collider 2D, Radius 0.015, Offset X 0, Y -0.005
//  * (Index 177 ~ 192) 옥구슬탄 : Circle Collider 2D, Radius 0.04
//  * (Index 193 ~ 208) 소형 테두리 원탄 : Circle Collider 2D, Radius 0.015
//  * (Index 209 ~ 224) 소형 쌀탄 : Circle Collider 2D, Radius 0.015
//  * (Index 225 ~ 240) 소형 촉탄 : Circle Collider 2D, Radius 0.02, Offset X 0, Y 0.01
//  * (Index 241 ~ 244) 대옥탄 : Circle Collider 2D, Radius 0.135
//  * (Index 245 ~ 247) 엽전탄 : Circle Collider 2D, Radius 0.04
//  * (Index 258 ~ 265) 대형 환탄 : Circle Collider 2D, Radius 0.08
//  * (Index 266 ~ 273) 나비탄 : Circle Collider 2D, Radius 0.02
//  * (Index 274 ~ 281) 나이프탄 : Capsule Collider 2D, Size X 0.035, Y 0.2, Direction Vertical
//  * (Index 282 ~ 289) 알약탄 : Capsule Collider 2D, Size X 0.06, Y 0.18, Direction Vertical
//  * (Index 290 ~ 297) 대형 별탄 : Circle Collider 2D, Radius 0.045, Offset X 0, Y -0.01
//  * (Index 298 ~ 305) 탄막 발사 이펙트
//  * (Index 306 ~ 313) 대형 발광탄 : Circle Collider 2D, Radius 0.1
//  * (Index 314 ~ 321) 하트탄 : Circle Collider 2D, Radius 0.065, Offset X 0, Y -0.01
//  * (Index 322 ~ 329) 탄막 발사 이펙트
//  * (Index 330 ~ 337) 화살탄 : Capsule Collider 2D, Size X 0.01, Y 0.2, Direction Vertical
//  * (Index 338 ~ 349) 음표탄 : Circle Collider 2D, Radius 0.02, Offset X -0.01, Y -0.09, Sprite Animation 적용
//  * (Index 350 ~ 357) 쉼표탄 : Capsule Collider 2D, Size X 0.02, Y 0.18
//  * (Index 358 ~ 373) 고정 레이저탄 : Box Collider 2D, Size X 0.09, Y 0.13
//  * (Index 374 ~ 389) 무빙 레이저탄 (머리 1) : Box Collider 2D, Size X 0.52, Y 0.04, Offset X 0.02, Y 0
//  * (Index 390 ~ 405) 무빙 레이저탄 (머리 2) : Box Collider 2D, Size X 0.56, Y 0.04
//  * (Index 406 ~ 421) 무빙 레이저탄 (몸통) : Box Collider 2D, Size X 0.24, Y 0.04
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

        enemyBulletTemp1 = GameObject.Find("BULLET").transform.GetChild(3);
        enemyBulletTemp2 = GameObject.Find("BULLET").transform.GetChild(4);
        enemyBulletTemp3 = GameObject.Find("BULLET").transform.GetChild(5);
    }

    #region 적 패턴 모음

    public void Fire(int stageNumber)
    {
        switch (stageNumber)
        {
            case 1:
                StartCoroutine(Stage1());
                break;
            case 2:
                StartCoroutine(Stage2());
                break;
            case 3:
                StartCoroutine(Stage3());
                break;
            case 4:
                StartCoroutine(Stage4());
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                StartCoroutine(Stage10());
                break;
            default:
                break;
        }
    }

    #endregion

    #region 패턴 1 (자작)

    public IEnumerator Stage1()
    {
        StartCoroutine(Stage1EnemyMove());
        StartCoroutine(Stage1Pattern1());
        StartCoroutine(Stage1Pattern2());

        yield return null;
    }
    public IEnumerator Stage1EnemyMove()
    {
        while (true)
        {
            Vector3 playerPosition = playerObject.transform.position;
            Vector3 randomPosition;

            if (playerPosition.x <= -3.0f)
            {
                randomPosition = new Vector3(Random.Range(playerPosition.x + 0.25f, playerPosition.x + 1.25f), Random.Range(1.5f, 3.0f), 0.0f);
            }
            else if (playerPosition.x >= 3.0f)
            {
                randomPosition = new Vector3(Random.Range(playerPosition.x - 1.25f, playerPosition.x - 0.25f), Random.Range(1.5f, 3.0f), 0.0f);
            }
            else
            {
                randomPosition = new Vector3(Random.Range(playerPosition.x - 0.75f, playerPosition.x + 0.75f), Random.Range(1.5f, 3.0f), 0.0f);
            }

            iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", 1.5f));
            StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, 1.5f));

            yield return new WaitForSeconds(2.5f);
        }
    }
    public IEnumerator Stage1Pattern1()
    {
        int angleMultiply = 1;
        float rotateAngle = 0.0f;
        int rotateMultiply = 1;

        while (true)
        {
            StartCoroutine(Stage1Pattern1Attack(rotateAngle, angleMultiply));

            rotateAngle += (1.0f * rotateMultiply);
            rotateMultiply++;

            if (rotateAngle <= 720.0f)
            {
                yield return new WaitForSeconds(0.06f);
            }
            else
            {
                rotateAngle = 0.0f;
                angleMultiply *= -1;
                rotateMultiply = 0;
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
    public IEnumerator Stage1Pattern1Attack(float rotateAngle, int angleMultiply)
    {
        Vector2 playerPosition = playerObject.transform.position;
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.8f, 0.15f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.15f);

        // 탄막 1 발사 (파란색, 하늘색 쐐기탄) (조준 방사탄)
        for (int i = 0; i < 16; i++)
        {
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = bulletFirePosition;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                bullet.transform.SetParent(enemyBulletTemp1);
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                spriteRenderer.sprite = spriteCollection[i % 2 == 0 ? 103 : 105];
                spriteRenderer.sortingOrder = 3;
                capsuleCollider2D.isTrigger = true;
                capsuleCollider2D.size = new Vector2(0.04f, 0.06f);
                capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
                capsuleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 9.0f;
                movingBullet.bulletDecelerationMoveSpeed = 0.3f;
                movingBullet.bulletDecelerationMoveSpeedMin = 3.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(playerPosition);
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f + (22.5f * i) + (angleMultiply == 1 ? rotateAngle : -rotateAngle));
            }
            else AddBulletPool();
        }
    }
    public IEnumerator Stage1Pattern2()
    {
        int rotateMultiply = 1;

        while (true)
        {
            StartCoroutine(Stage1Pattern2Attack(rotateMultiply));
            rotateMultiply = (rotateMultiply == 1 ? -1 : 1);

            yield return new WaitForSeconds(0.25f);
        }
    }
    public IEnumerator Stage1Pattern2Attack(int rotateMultiply)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(304, 0.6f, 0.225f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.225f);

        // 탄막 2 발사 (노란색, 주황색 부적탄) (조준 방사탄)
        for (int i = 0; i < 18; i++)
        {
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = bulletFirePosition;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                bullet.transform.SetParent(enemyBulletTemp2);
                bullet.AddComponent<SpriteRenderer>();
                bullet.AddComponent<CapsuleCollider2D>();
                bullet.AddComponent<InitializeBullet>();
                bullet.AddComponent<MovingBullet>();
                bullet.AddComponent<EraseBullet>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                spriteRenderer.sprite = spriteCollection[rotateMultiply == 1 ? 126 : 127];
                spriteRenderer.sortingOrder = 3;
                capsuleCollider2D.isTrigger = true;
                capsuleCollider2D.size = new Vector2(0.04f, 0.06f);
                capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
                capsuleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 3.0f;
                movingBullet.bulletRotateSpeed = rotateMultiply == 1 ? 45.0f : -45.0f;
                movingBullet.bulletRotateLimit = 1.5f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
                movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f + (20.0f * i));
            }
            else AddBulletPool();
        }
    }

    #endregion

    #region 패턴 2 (자작)

    public IEnumerator Stage2()
    {
        Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);

        iTween.MoveTo(gameObject, iTween.Hash("position", targetPosition, "easetype", iTween.EaseType.easeOutQuad, "time", 1.2f));
        StartCoroutine(EnemySpriteSet(targetPosition.x, transform.position.x, 1.2f));

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(Stage2Pattern1());
    }
    public IEnumerator Stage2Pattern1()
    {
        while (true)
        {
            StartCoroutine(Stage2Pattern1Attack());

            yield return new WaitForSeconds(0.25f);
        }
    }
    public IEnumerator Stage2Pattern1Attack()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(303, 0.6f, 0.225f, 0.2f, bulletFirePosition));

        yield return new WaitForSeconds(0.225f);

        // 탄막 1 발사 (초록색 화살탄) (각도 제한 랜덤탄)
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
            if (!bullet.GetComponent<Stage2BulletFragmentation>()) bullet.AddComponent<Stage2BulletFragmentation>();
            SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
            CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
            InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
            MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
            spriteRenderer.sprite = spriteCollection[335];
            spriteRenderer.sortingOrder = 3;
            capsuleCollider2D.isTrigger = true;
            capsuleCollider2D.size = new Vector2(0.01f, 0.2f);
            capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
            capsuleCollider2D.enabled = false;
            initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
            initializeBullet.bulletObject = bullet.gameObject;
            initializeBullet.targetObject = playerObject;
            initializeBullet.isGrazed = false;
            movingBullet.bulletMoveSpeed = 5.0f;
            movingBullet.bulletDecelerationMoveSpeed = Random.Range(0.04f, 0.06f);
            movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
            movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
            movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector2(bulletFirePosition.x, bulletFirePosition.y + 10.0f));
            float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
            movingBullet.ChangeRotateAngle(angle - 90.0f + Random.Range(-40.0f, 40.0f));
        }
        else AddBulletPool();
    }

    #endregion

    #region 패턴 3 (동방요요몽 판타즘 스테이지 - 야쿠모 유카리 7스펠 "이중흑사접" 리메이크)

    public IEnumerator Stage3()
    {
        StartCoroutine(Stage3Pattern1());

        yield return null;
    }
    public IEnumerator Stage3EnemyMove()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(2.0f, 4.25f), 0.0f);

        iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", 1.0f));
        StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, 1.0f));

        yield return null;
    }
    public IEnumerator Stage3Pattern1()
    {
        bool isTextureIndexChange = false;

        while (true)
        {
            StartCoroutine(Stage3Pattern1Attack1(isTextureIndexChange));
            StartCoroutine(Stage3Pattern1Attack2(isTextureIndexChange));
            StartCoroutine(Stage3Pattern2());

            yield return new WaitForSeconds(4.0f);

            StartCoroutine(Stage3EnemyMove());
            if (isTextureIndexChange.Equals(false))
            {
                isTextureIndexChange = true;
            }
            else
            {
                isTextureIndexChange = false;
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
    public IEnumerator Stage3Pattern1Attack1(bool isTextureIndexChange)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(301, 0.8f, 0.15f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.15f);

        // 탄막 1 발사 (파란색 보석탄, 쿠나이탄) (랜덤탄)
        for (int i = 0; i < 192; i++)
        {
            int randomValue = Random.Range(1, 6);

            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = bulletFirePosition;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
                bullet.transform.SetParent(enemyBulletTemp1);
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                if (!bullet.GetComponent<Stage3BulletRotate>()) bullet.AddComponent<Stage3BulletRotate>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                spriteRenderer.sprite = spriteCollection[isTextureIndexChange.Equals(false) ? 71 : 87];
                spriteRenderer.sortingOrder = 3;
                capsuleCollider2D.isTrigger = true;
                capsuleCollider2D.size = new Vector2(0.02f, isTextureIndexChange.Equals(false) ? 0.05f : 0.055f);
                capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
                capsuleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                switch (randomValue)
                {
                    case 1:
                        movingBullet.bulletMoveSpeed = Random.Range(2.0f, 4.0f);
                        break;
                    case 2:
                        movingBullet.bulletMoveSpeed = Random.Range(4.0f, 6.0f);
                        break;
                    case 3:
                        movingBullet.bulletMoveSpeed = Random.Range(6.0f, 8.0f);
                        break;
                    case 4:
                        movingBullet.bulletMoveSpeed = Random.Range(8.0f, 10.0f);
                        break;
                    case 5:
                        movingBullet.bulletMoveSpeed = Random.Range(10.0f, 12.0f);
                        break;
                    default:
                        movingBullet.bulletMoveSpeed = 0.0f;
                        break;
                }
                movingBullet.bulletAccelerationMoveSpeed = 0.015f;
                movingBullet.bulletAccelerationMoveSpeedMax = 5.0f;
                movingBullet.bulletDecelerationMoveSpeed = movingBullet.bulletMoveSpeed * 0.01f;
                movingBullet.bulletRotateSpeed = 60.0f;
                movingBullet.bulletRotateLimit = 3.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f);
            }
            else AddBulletPool();
        }
    }
    public IEnumerator Stage3Pattern1Attack2(bool isTextureIndexChange)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 2 이펙트
        StartCoroutine(CreateBulletFireEffect(300, 0.8f, 0.15f, 0.3f, bulletFirePosition));

        yield return new WaitForSeconds(0.15f);

        // 탄막 2 발사 (분홍색 보석탄, 쿠나이탄) (랜덤탄)
        for (int i = 0; i < 192; i++)
        {
            int randomValue = Random.Range(1, 6);

            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = bulletFirePosition;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
                bullet.transform.SetParent(enemyBulletTemp1);
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                if (!bullet.GetComponent<Stage3BulletRotate>()) bullet.AddComponent<Stage3BulletRotate>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                spriteRenderer.sprite = spriteCollection[isTextureIndexChange.Equals(false) ? 69 : 85];
                spriteRenderer.sortingOrder = 3;
                capsuleCollider2D.isTrigger = true;
                capsuleCollider2D.size = new Vector2(0.02f, isTextureIndexChange.Equals(false) ? 0.05f : 0.055f);
                capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
                capsuleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                switch (randomValue)
                {
                    case 1:
                        movingBullet.bulletMoveSpeed = Random.Range(2.0f, 4.0f);
                        break;
                    case 2:
                        movingBullet.bulletMoveSpeed = Random.Range(4.0f, 6.0f);
                        break;
                    case 3:
                        movingBullet.bulletMoveSpeed = Random.Range(6.0f, 8.0f);
                        break;
                    case 4:
                        movingBullet.bulletMoveSpeed = Random.Range(8.0f, 10.0f);
                        break;
                    case 5:
                        movingBullet.bulletMoveSpeed = Random.Range(10.0f, 12.0f);
                        break;
                    default:
                        movingBullet.bulletMoveSpeed = 0.0f;
                        break;
                }
                movingBullet.bulletAccelerationMoveSpeed = 0.015f;
                movingBullet.bulletAccelerationMoveSpeedMax = 5.0f;
                movingBullet.bulletDecelerationMoveSpeed = movingBullet.bulletMoveSpeed * 0.01f;
                movingBullet.bulletRotateSpeed = -60.0f;
                movingBullet.bulletRotateLimit = 3.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f);
            }
            else AddBulletPool();
        }
    }
    public IEnumerator Stage3Pattern2()
    {
        int fireCount = 0;

        while (true)
        {
            StartCoroutine(Stage3Pattern2Attack());
            fireCount++;

            if (fireCount.Equals(8))
            {
                fireCount = 0;
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.15f);
            }
        }

        yield return null;
    }
    public IEnumerator Stage3Pattern2Attack()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 3 이펙트
        StartCoroutine(CreateBulletFireEffect(304, 0.6f, 0.225f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.225f);

        // 탄막 3 발사 (노란색 나비탄) (조준 방사탄)
        for (int i = 0; i < 18; i++)
        {
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = bulletFirePosition;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                bullet.transform.SetParent(enemyBulletTemp2);
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                spriteRenderer.sprite = spriteCollection[272];
                spriteRenderer.sortingOrder = 3;
                circleCollider2D.isTrigger = true;
                circleCollider2D.radius = 0.02f;
                circleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 5.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f + (20.0f * i));
            }
            else AddBulletPool();
        }
    }

    #endregion

    #region 패턴 4 (파동과 입자의 경계 모작)

    public IEnumerator Stage4()
    {
        Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);

        iTween.MoveTo(gameObject, iTween.Hash("position", targetPosition, "easetype", iTween.EaseType.easeOutQuad, "time", 1.2f));
        StartCoroutine(EnemySpriteSet(targetPosition.x, transform.position.x, 1.2f));

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(Stage4Pattern1());
    }
    public IEnumerator Stage4Pattern1()
    {
        float rotateAngle = 0.0f;
        int fireCount = 0;
        int rotateMultiply = 1;
        int rotateDirectionChange = 0;

        while (true)
        {
            StartCoroutine(Stage4Pattern1Attack(rotateAngle, rotateMultiply));

            rotateAngle += (0.4f * rotateMultiply);
            fireCount++;
            if (rotateDirectionChange.Equals(0))
            {
                rotateMultiply++;
            }
            else
            {
                rotateMultiply--;
            }

            if (fireCount.Equals(24))
            {
                fireCount = 0;
                rotateDirectionChange = Random.Range(0, 2);
            }

            if (rotateMultiply >= 120) rotateDirectionChange = 1;
            else if (rotateMultiply <= -120) rotateDirectionChange = 0;

            yield return new WaitForSeconds(0.06f);
        }
    }
    public IEnumerator Stage4Pattern1Attack(float rotateAngle, float rotateMultiply)
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        StartCoroutine(CreateBulletFireEffect(300, 0.8f, 0.1f, 0.4f, bulletFirePosition));

        yield return new WaitForSeconds(0.1f);

        // 탄막 1 발사 (보라색 쌀탄) (회전 방사탄)
        for (int i = 0; i < 16; i++)
        {
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = bulletFirePosition;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                bullet.transform.SetParent(enemyBulletTemp1);
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                spriteRenderer.sprite = spriteCollection[53];
                spriteRenderer.sortingOrder = 3;
                capsuleCollider2D.isTrigger = true;
                capsuleCollider2D.size = new Vector2(0.025f, 0.05f);
                capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
                capsuleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 2.0f;
                movingBullet.bulletAccelerationMoveSpeed = 0.025f;
                movingBullet.bulletAccelerationMoveSpeedMax = 6.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector2(transform.position.x, transform.position.y - 1.0f));
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f + (22.5f * i) + rotateAngle);
            }
            else AddBulletPool();
        }

        yield return null;
    }

    #endregion

    #region 패턴 10 (도돈파치 대왕생 2주차 5스테이지 - 히바치 5패턴 열화판)

    public IEnumerator Stage10()
    {
        StartCoroutine(Stage10EnemyMove());
        StartCoroutine(Stage10EnemyAttack1());

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(Stage10EnemyAttack2());

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(Stage10EnemyAttack3());
    }
    public IEnumerator Stage10EnemyMove()
    {
        while (true)
        {
            Vector3 playerPosition = playerObject.transform.position;
            Vector3 randomPosition;
            if (playerPosition.x <= -3.0f)
            {
                randomPosition = new Vector3(Random.Range(playerPosition.x + 0.25f, playerPosition.x + 1.25f), Random.Range(1.5f, 2.5f), 0.0f);
            }
            else if (playerPosition.x >= 3.0f)
            {
                randomPosition = new Vector3(Random.Range(playerPosition.x - 1.25f, playerPosition.x - 0.25f), Random.Range(1.5f, 2.5f), 0.0f);
            }
            else
            {
                randomPosition = new Vector3(Random.Range(playerPosition.x - 0.75f, playerPosition.x + 0.75f), Random.Range(1.5f, 2.5f), 0.0f);
            }

            iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", 1.0f));
            StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, 1.0f));

            yield return new WaitForSeconds(1.0f);
        }
    }
    public IEnumerator Stage10EnemyAttack1()
    {
        float rotateAngle = 0.0f;

        while (true)
        {
            Vector2 bulletFirePosition = transform.position;

            // 탄막 1 이펙트
            StartCoroutine(CreateBulletFireEffect(299, 0.8f, 0.1f, 0.4f, bulletFirePosition));

            yield return new WaitForSeconds(0.1f);

            // 탄막 1 발사 (빨간색 대옥탄) (조준 회전 방사탄)
            for (int i = 0; i < 2; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
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
                    spriteRenderer.sprite = spriteCollection[241];
                    spriteRenderer.sortingOrder = 3;
                    spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.75f);
                    circleCollider2D.isTrigger = true;
                    circleCollider2D.radius = 0.135f;
                    circleCollider2D.enabled = false;
                    initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                    initializeBullet.bulletObject = bullet.gameObject;
                    initializeBullet.targetObject = playerObject;
                    initializeBullet.isGrazed = false;
                    movingBullet.bulletMoveSpeed = 6.0f;
                    movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination
                        (new Vector3(transform.position.x, (i == 0 ? transform.position.y - 10.0f : transform.position.y + 10.0f), 0.0f));
                    float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                    movingBullet.ChangeRotateAngle(angle - 90.0f + rotateAngle + Random.Range(-3.0f, 3.0f));
                }
                else AddBulletPool();
            }

            rotateAngle += 6.0f;
            if (rotateAngle >= 360.0f)
            {
                rotateAngle = 0.0f;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public IEnumerator Stage10EnemyAttack2()
    {
        float rotateAngle = 0.0f;

        while (true)
        {
            Vector2 bulletFirePosition = transform.position;

            // 탄막 2 이펙트
            StartCoroutine(CreateBulletFireEffect(299, 0.8f, 0.1f, 0.4f, bulletFirePosition));

            yield return new WaitForSeconds(0.1f);

            // 탄막 2 발사 (빨간색 대옥탄) (조준 회전 방사탄)
            for (int i = 0; i < 2; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
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
                    spriteRenderer.sprite = spriteCollection[241];
                    spriteRenderer.sortingOrder = 3;
                    spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.75f);
                    circleCollider2D.isTrigger = true;
                    circleCollider2D.radius = 0.135f;
                    circleCollider2D.enabled = false;
                    initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                    initializeBullet.bulletObject = bullet.gameObject;
                    initializeBullet.targetObject = playerObject;
                    initializeBullet.isGrazed = false;
                    movingBullet.bulletMoveSpeed = 6.0f;
                    movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination
                        (new Vector3(transform.position.x, (i == 0 ? transform.position.y - 10.0f : transform.position.y + 10.0f), 0.0f));
                    float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                    movingBullet.ChangeRotateAngle(angle - 90.0f - rotateAngle + Random.Range(-3.0f, 3.0f));
                }
                else AddBulletPool();
            }

            rotateAngle += 6.0f;
            if (rotateAngle >= 360.0f)
            {
                rotateAngle = 0.0f;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public IEnumerator Stage10EnemyAttack3()
    {
        float rotateAngle = 0.0f;

        while (true)
        {
            Vector2 bulletFirePosition = transform.position;

            // 탄막 3 이펙트
            StartCoroutine(CreateBulletFireEffect(301, 0.8f, 0.1f, 0.5f, bulletFirePosition));

            yield return new WaitForSeconds(0.1f);

            // 탄막 3 발사 (파란색 환탄) (조준 회전 방사탄)
            for (int i = 0; i < 12; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                    bullet.transform.SetParent(enemyBulletTemp2);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                    CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                    InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                    MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                    spriteRenderer.sprite = spriteCollection[261];
                    spriteRenderer.sortingOrder = 3;
                    circleCollider2D.isTrigger = true;
                    circleCollider2D.radius = 0.08f;
                    circleCollider2D.enabled = false;
                    bullet.AddComponent<InitializeBullet>();
                    bullet.AddComponent<MovingBullet>();
                    bullet.AddComponent<EraseBullet>();
                    initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                    initializeBullet.bulletObject = bullet.gameObject;
                    initializeBullet.targetObject = playerObject;
                    initializeBullet.isGrazed = false;
                    movingBullet.bulletMoveSpeed = 6.0f;
                    movingBullet.bulletRotateSpeed = 60.0f;
                    movingBullet.bulletRotateLimit = 1.5f;
                    movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector3(transform.position.x, transform.position.y - 10.0f, 0.0f));
                    float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                    movingBullet.ChangeRotateAngle(angle - 90.0f + (30.0f * i) + rotateAngle + Random.Range(-6.0f, 6.0f));
                }
                else AddBulletPool();
            }

            rotateAngle += 12.0f;
            if (rotateAngle >= 360.0f)
            {
                rotateAngle = 0.0f;
            }
            yield return new WaitForSeconds(0.02f);
        }
    }

    #endregion

    #region 테스트용 패턴



    #endregion

    #region 패턴 보관함
    /*
    #region 패턴 보관함

    #region 패턴 1 (동방지령전 4스테이지 - 코메이지 사토리 마리사A 4스펠 "리턴 인애니메이트니스" 열화판)

    public IEnumerator Stage1EnemyAttack()
    {
        while (true)
        {
            // 탄막 1 발사 (파란색 환탄) (조준 방사탄)
            for (int i = 0; i < 16; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    if (!bullet.GetComponent<Stage1BulletFragmentation>()) bullet.AddComponent<Stage1BulletFragmentation>();
                    SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                    CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                    InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                    MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                    spriteRenderer.sprite = spriteCollection[261];
                    spriteRenderer.sortingOrder = 3;
                    circleCollider2D.isTrigger = true;
                    circleCollider2D.radius = 0.08f;
                    circleCollider2D.enabled = false;
                    initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                    initializeBullet.bulletObject = bullet.gameObject;
                    initializeBullet.targetObject = playerObject;
                    initializeBullet.isGrazed = false;
                    if (i < 8)
                    {
                        movingBullet.bulletMoveSpeed = 4.0f;
                        movingBullet.bulletDecelerationMoveSpeed = 0.1f;
                    }
                    else
                    {
                        movingBullet.bulletMoveSpeed = 8.0f;
                        movingBullet.bulletDecelerationMoveSpeed = 0.2f;
                    }
                    movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                    movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
                    float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                    movingBullet.ChangeRotateAngle(angle - 90.0f + (45.0f * (i % 8)));
                }
                else AddBulletPool();

                yield return new WaitForSeconds(0.03f);

                if (i == 7)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            yield return new WaitForSeconds(1.5f);
        }
    }

    #endregion

    #region 패턴 2 (자작)

    public IEnumerator Stage2EnemyAttack()
    {
        while (true)
        {
            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (좌측 상단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = new Vector2(Random.Range(-6.0f, -5.0f), Random.Range(-4.5f, 1.5f));
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[268];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    bullet.GetComponent<CircleCollider2D>().enabled = false;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                else AddBulletPool();
            }
            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (좌측 하단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = new Vector2(Random.Range(-6.0f, -5.0f), Random.Range(1.5f, 7.5f));
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[268];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    bullet.GetComponent<CircleCollider2D>().enabled = false;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                else AddBulletPool();
            }
            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (우측 상단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = new Vector2(Random.Range(5.0f, 6.0f), Random.Range(-4.5f, 1.5f));
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[268];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    bullet.GetComponent<CircleCollider2D>().enabled = false;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                else AddBulletPool();
            }
            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (우측 하단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = new Vector2(Random.Range(5.0f, 6.0f), Random.Range(1.5f, 7.5f));
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[268];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    bullet.GetComponent<CircleCollider2D>().enabled = false;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                else AddBulletPool();
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    #endregion

    #region 패턴 3 (자작)

    public IEnumerator Stage3EnemyAttack()
    {
        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                // 빈 탄막 발사
                if (bulletManager.bulletPool.Count > 0)
                {
                    AddBulletPool();
                }

                GameObject emptyBullet = bulletManager.bulletPool.Dequeue();
                if (bulletManager.bulletPool.Count > 0)
                {
                    emptyBullet.SetActive(true);
                    ClearChild(emptyBullet);
                    emptyBullet.transform.position = transform.position;
                    emptyBullet.gameObject.tag = "BULLET_ENEMY_EMPTY";
                    emptyBullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    emptyBullet.transform.SetParent(enemyBulletTemp1);
                    if (!emptyBullet.GetComponent<SpriteRenderer>()) emptyBullet.AddComponent<SpriteRenderer>();
                    if (!emptyBullet.GetComponent<CircleCollider2D>()) emptyBullet.AddComponent<CircleCollider2D>();
                    emptyBullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[0];
                    emptyBullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    emptyBullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    emptyBullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    emptyBullet.GetComponent<CircleCollider2D>().enabled = false;
                    if (!emptyBullet.GetComponent<InitializeBullet>()) emptyBullet.AddComponent<InitializeBullet>();
                    if (!emptyBullet.GetComponent<MovingBullet>()) emptyBullet.AddComponent<MovingBullet>();
                    if (!emptyBullet.GetComponent<EraseBullet>()) emptyBullet.AddComponent<EraseBullet>();
                    emptyBullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    emptyBullet.GetComponent<InitializeBullet>().bulletObject = emptyBullet.gameObject;
                    emptyBullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                    emptyBullet.GetComponent<InitializeBullet>().isGrazed = true;
                    emptyBullet.GetComponent<MovingBullet>().bulletMoveSpeed = 2.0f;
                    emptyBullet.GetComponent<MovingBullet>().bulletRotateSpeed = 60.0f;
                    emptyBullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    emptyBullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    emptyBullet.GetComponent<MovingBullet>().bulletDestination = emptyBullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                    float angle = Mathf.Atan2(emptyBullet.GetComponent<MovingBullet>().bulletDestination.y, emptyBullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    emptyBullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 135.0f + (22.5f * i));
                }
                else AddBulletPool();

                // 탄막 발사 (하늘색 나비탄) (빈 탄막을 중심으로 회전)
                for (int j = 0; j < 12; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = emptyBullet.transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                        bullet.transform.SetParent(emptyBullet.transform);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[270];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                        bullet.GetComponent<CircleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = emptyBullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 2.0f;
                        bullet.GetComponent<MovingBullet>().bulletRotateSpeed = 180.0f;
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (30.0f * j));
                    }
                    else AddBulletPool();
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    #endregion

    #region 패턴 4 (자작)

    public IEnumerator Stage4EnemyAttack()
    {
        int angleMultiply = 1;

        while (true)
        {
            for (int i = 0; i < 18; i++)
            {
                // 탄막 1 발사 (무작위 색상 원탄) (랜덤탄)
                for (int j = 0; j < 18; j++)
                {
                    int bulletTextureIndex1 = Random.Range(17, 32);

                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = gameObject.transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(enemyBulletTemp1);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[bulletTextureIndex1];
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
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = Random.Range(2.0f, 8.0f);
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetRandomAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f);
                    }
                    else AddBulletPool();
                }

                yield return new WaitForSeconds(0.04f);
            }

            yield return new WaitForSeconds(0.2f);

            int bulletTextureIndex2 = Random.Range(97, 112);
            Vector2 targetPosition = playerObject.transform.position;

            // 탄막 2 발사 (무작위 색상 쐐기탄) (조준 방사탄)
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(enemyBulletTemp2);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[bulletTextureIndex2];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CapsuleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CapsuleCollider2D>().size = new Vector2(0.04f, 0.06f);
                        bullet.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Vertical;
                        bullet.GetComponent<CapsuleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 9.0f;
                        bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeed = 0.3f;
                        bullet.GetComponent<MovingBullet>().bulletDecelerationMoveSpeedMin = 3.0f;
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(targetPosition);
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (22.5f * j) + (1.5f * i) * angleMultiply);
                    }
                    else AddBulletPool();
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

    public IEnumerator Stage5EnemyMove(float moveTime)
    {
        Vector3 randomPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);

        iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", moveTime));
        StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, moveTime));

        yield return null;
    }
    public IEnumerator Stage5EnemyAttack()
    {
        int angleMultiply = 1;

        while (true)
        {
            // 대옥탄에서 생성된 탄 활성화
            for (int i = 0; i < enemyBulletTemp2.childCount; i++)
            {
                GameObject bullet = enemyBulletTemp2.transform.GetChild(i).gameObject;
                if (bullet.activeSelf == true)
                {
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[19];
                    bullet.GetComponent<MovingBullet>().bulletAccelerationMoveSpeed = 0.02f;
                    bullet.GetComponent<MovingBullet>().bulletAccelerationMoveSpeedMax = 3.0f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
                }
            }

            // 탄막 발사 (빨간색 대옥탄) (조준 회전탄)
            for (int i = 0; i < 32; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[241];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.135f;
                    bullet.GetComponent<CircleCollider2D>().enabled = false;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    if (!bullet.GetComponent<Stage5BulletCreate>()) bullet.AddComponent<Stage5BulletCreate>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                else AddBulletPool();
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
            StartCoroutine(Stage5EnemyMove(1.5f));

            yield return new WaitForSeconds(3.0f);
        }
    }

    #endregion

    #region 패턴 6 (동방홍마향 1스테이지 - 루미아 1통상 리메이크)

    public IEnumerator Stage6EnemyMove(float moveTime)
    {
        Vector3 randomPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);

        iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", moveTime));
        StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, moveTime));

        yield return null;
    }
    public IEnumerator Stage6EnemyAttack()
    {
        while (true)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8 * (i + 1); j++)
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
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[18 + (i % 2)];
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
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f + (0.8f * (j % 8));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (10.0f * (j / 8)) - (5.0f * i));
                    }
                    else AddBulletPool();
                }

                yield return new WaitForSeconds(0.1f);
            }

            // 랜덤한 지점으로 이동
            StartCoroutine(Stage6EnemyMove(1.5f));

            yield return new WaitForSeconds(2.0f);
        }
    }

    #endregion

    #region 패턴 7 (동방홍마향 6스테이지 - 레밀리아 스칼렛 4스펠 "스칼렛 마이스터")

    public IEnumerator Stage7EnemyMove(float moveTime)
    {
        Vector3 randomPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);

        iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", moveTime));
        StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, moveTime));

        yield return null;
    }
    public IEnumerator Stage7EnemyAttack()
    {
        while (true)
        {
            // 플레이어 조준 발사 6회
            for (int i = 0; i < 6; i++)
            {
                // 탄막 1 발사 (빨간색 대옥탄)
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[241];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.135f;
                    bullet.GetComponent<CircleCollider2D>().enabled = false;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 5.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f);
                }
                else AddBulletPool();

                // 탄막 2 발사 (빨간색 대형 환탄)
                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                        bullet.transform.SetParent(enemyBulletTemp2);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[259];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.08f;
                        bullet.GetComponent<CircleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.5f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (Random.Range(-15.0f, 15.0f)));
                    }
                    else AddBulletPool();
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(enemyBulletTemp3);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[35];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                        bullet.GetComponent<CircleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (Random.Range(-30.0f, 30.0f)));
                    }
                    else AddBulletPool();
                }

                yield return new WaitForSeconds(0.07f);
            }

            // 랜덤한 지점으로 이동
            StartCoroutine(Stage7EnemyMove(1.0f));

            Vector2 playerPosition = playerObject.transform.position;

            // 시계 방향 발사 11회
            for (int i = 0; i < 11; i++)
            {
                // 탄막 1 발사 (빨간색 대옥탄)
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[241];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.135f;
                    bullet.GetComponent<CircleCollider2D>().enabled = false;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 5.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f - (30.0f * (i + 1)));
                }
                else AddBulletPool();

                // 탄막 2 발사 (빨간색 대형 환탄)
                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                        bullet.transform.SetParent(enemyBulletTemp2);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[259];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.08f;
                        bullet.GetComponent<CircleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.5f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f - (30.0f * (i + 1)) + (Random.Range(-30.0f, 30.0f)));
                    }
                    else AddBulletPool();
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                for (int j = 0; j < 16; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(enemyBulletTemp3);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[35];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                        bullet.GetComponent<CircleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f - (30.0f * (i + 1)) + (Random.Range(-45.0f, 45.0f)));
                    }
                    else AddBulletPool();
                }

                yield return new WaitForSeconds(0.07f);
            }

            yield return new WaitForSeconds(1.0f);

            // 플레이어 조준 발사 3회
            for (int i = 0; i < 3; i++)
            {
                // 탄막 1 발사 (빨간색 대옥탄)
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[241];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.135f;
                    bullet.GetComponent<CircleCollider2D>().enabled = false;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 5.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f);
                }
                else AddBulletPool();

                // 탄막 2 발사 (빨간색 대형 환탄)
                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                        bullet.transform.SetParent(enemyBulletTemp2);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[259];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.08f;
                        bullet.GetComponent<CircleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.5f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (Random.Range(-15.0f, 15.0f)));
                    }
                    else AddBulletPool();
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                for (int j = 0; j < 12; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(enemyBulletTemp3);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[35];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                        bullet.GetComponent<CircleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (Random.Range(-30.0f, 30.0f)));
                    }
                    else AddBulletPool();
                }

                yield return new WaitForSeconds(0.06f);
            }

            // 랜덤한 지점으로 이동
            StartCoroutine(Stage7EnemyMove(1.0f));

            playerPosition = playerObject.transform.position;

            // 반시계 방향 발사 11회
            for (int i = 0; i < 11; i++)
            {
                // 탄막 1 발사 (빨간색 대옥탄)
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[241];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.135f;
                    bullet.GetComponent<CircleCollider2D>().enabled = false;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 5.5f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (30.0f * (i + 1)));
                }
                else AddBulletPool();

                // 탄막 2 발사 (빨간색 대형 환탄)
                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                        bullet.transform.SetParent(enemyBulletTemp2);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[259];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.08f;
                        bullet.GetComponent<CircleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.5f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (30.0f * (i + 1)) + (Random.Range(-30.0f, 30.0f)));
                    }
                    else AddBulletPool();
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                for (int j = 0; j < 24; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        GameObject bullet = bulletManager.bulletPool.Dequeue();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(enemyBulletTemp3);
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[35];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                        bullet.GetComponent<CircleCollider2D>().enabled = false;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
                        bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 3.0f + (Random.Range(-1.0f, 1.0f));
                        bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                        bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                        bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                        float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                        bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (30.0f * (i + 1)) + (Random.Range(-45.0f, 45.0f)));
                    }
                    else AddBulletPool();
                }

                yield return new WaitForSeconds(0.06f);
            }

            yield return new WaitForSeconds(2.0f);
        }
    }

    #endregion

    #region 패턴 8 (동방홍마향 1스테이지 - 루미아 1스펠 "나이트 버드" 리메이크

    public IEnumerator Stage8EnemyMove(float moveTime)
    {
        Vector3 randomPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);

        iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", moveTime));
        StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, moveTime));

        yield return null;
    }
    public IEnumerator Stage8EnemyAttack()
    {
        Vector2 playerPosition;

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                playerPosition = playerObject.transform.position;

                // 탄막 1 발사 (파란색 테두리원탄) (조준 방사탄)
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 3; k++)
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
                            bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[39];
                            bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                            bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                            bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                            bullet.GetComponent<CircleCollider2D>().enabled = false;
                            if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                            if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                            if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                            bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                            bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                            bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                            bullet.GetComponent<InitializeBullet>().isGrazed = false;
                            bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 2.0f + (0.3f * k);
                            bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                            bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                            bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                            float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                            bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f - 33.75f + (7.5f * j));
                        }
                        else AddBulletPool();
                    }

                    yield return new WaitForSeconds(0.02f);
                }

                yield return new WaitForSeconds(0.03f);

                playerPosition = playerObject.transform.position;

                // 탄막 2 발사 (하늘색 테두리원탄) (조준 방사탄)
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (bulletManager.bulletPool.Count > 0)
                        {
                            GameObject bullet = bulletManager.bulletPool.Dequeue();
                            bullet.SetActive(true);
                            ClearChild(bullet);
                            bullet.transform.position = transform.position;
                            bullet.gameObject.tag = "BULLET_ENEMY";
                            bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                            bullet.transform.SetParent(enemyBulletTemp2);
                            if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                            if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                            bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[41];
                            bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                            bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                            bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                            bullet.GetComponent<CircleCollider2D>().enabled = false;
                            if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                            if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                            if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                            bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                            bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                            bullet.GetComponent<InitializeBullet>().targetObject = playerObject;
                            bullet.GetComponent<InitializeBullet>().isGrazed = false;
                            bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 2.4f + (0.3f * k);
                            bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                            bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                            bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                            float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                            bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + 30.0f - (7.5f * j));
                        }
                        else AddBulletPool();
                    }

                    yield return new WaitForSeconds(0.02f);
                }

                yield return new WaitForSeconds(0.03f);
            }

            // 랜덤한 지점으로 이동
            StartCoroutine(Stage8EnemyMove(1.25f));

            yield return new WaitForSeconds(1.5f);
        }
    }

    #endregion

    #region 패턴 9 (자작)

    public IEnumerator Stage9EnemyMove(float moveTime)
    {
        Vector3 randomPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);

        iTween.MoveTo(gameObject, iTween.Hash("position", randomPosition, "easetype", iTween.EaseType.easeOutQuad, "time", moveTime));
        StartCoroutine(EnemySpriteSet(randomPosition.x, transform.position.x, moveTime));

        yield return null;
    }
    public IEnumerator Stage9EnemyAttack()
    {
        float rotateAngle = 0.0f;
        Vector2 playerPosition;

        while (true)
        {
            playerPosition = playerObject.transform.position;

            for (int i = 0; i < 2; i++)
            {
                rotateAngle = (45.0f * i);

                StartCoroutine(Stage9EnemyAttack1(playerPosition, rotateAngle, (i.Equals(0) ? 1 : -1)));

                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(Stage9EnemyAttack2());
            // 랜덤한 지점으로 이동
            StartCoroutine(Stage9EnemyMove(1.25f));

            yield return new WaitForSeconds(1.5f);
        }
    }
    public IEnumerator Stage9EnemyAttack1(Vector2 playerPosition, float rotateAngle, int rotateDirection)
    {
        // 레이저 발사 (하늘색 레이저) (곡선 이동)
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    GameObject bullet = bulletManager.bulletPool.Dequeue();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(enemyBulletTemp1);
                    bullet.AddComponent<SpriteRenderer>();
                    bullet.AddComponent<BoxCollider2D>();
                    SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                    BoxCollider2D boxCollider2D = bullet.GetComponent<BoxCollider2D>();
                    switch (i)
                    {
                        case 0:
                            spriteRenderer.sprite = spriteCollection[446];
                            boxCollider2D.size = new Vector2(0.48f, 0.05f);
                            boxCollider2D.offset = new Vector2(0.04f, 0.0f);
                            bullet.transform.localScale = new Vector3(0.6f, 1.8f, 0.0f);
                            break;
                        case 1:
                            spriteRenderer.sprite = spriteCollection[430];
                            boxCollider2D.size = new Vector2(0.56f, 0.05f);
                            bullet.transform.localScale = new Vector3(0.8f, 1.8f, 0.0f);
                            break;
                        case 6:
                            spriteRenderer.sprite = spriteCollection[398];
                            boxCollider2D.size = new Vector2(0.56f, 0.05f);
                            bullet.transform.localScale = new Vector3(0.8f, 1.8f, 0.0f);
                            break;
                        case 7:
                            spriteRenderer.sprite = spriteCollection[382];
                            boxCollider2D.size = new Vector2(0.48f, 0.05f);
                            boxCollider2D.offset = new Vector2(-0.04f, 0.0f);
                            bullet.transform.localScale = new Vector3(0.6f, 1.8f, 0.0f);
                            break;
                        default:
                            spriteRenderer.sprite = spriteCollection[414];
                            boxCollider2D.size = new Vector2(0.24f, 0.05f);
                            bullet.transform.localScale = new Vector3(2.0f, 1.8f, 0.0f);
                            break;
                    }
                    spriteRenderer.sortingOrder = 3;
                    boxCollider2D.isTrigger = true;
                    boxCollider2D.enabled = false;
                    bullet.AddComponent<InitializeBullet>();
                    bullet.AddComponent<MovingBullet>();
                    bullet.AddComponent<EraseBullet>();
                    InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                    MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                    initializeBullet.bulletType = BulletType.BULLETTYPE_LASER_MOVE;
                    initializeBullet.bulletObject = bullet.gameObject;
                    initializeBullet.targetObject = playerObject;
                    initializeBullet.isGrazed = false;
                    movingBullet.bulletMoveSpeed = 4.0f;
                    movingBullet.bulletRotateLimit = 1.0f;
                    if (rotateDirection.Equals(1))
                    {
                        movingBullet.bulletRotateSpeed = 60.0f;
                    }
                    else if (rotateDirection.Equals(-1))
                    {
                        movingBullet.bulletRotateSpeed = -60.0f;
                    }
                    movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
                    movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(playerPosition);
                    float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                    movingBullet.ChangeRotateAngle(angle - 90.0f + (45.0f * j) + rotateAngle);
                }
                else AddBulletPool();
            }

            yield return new WaitForSeconds(0.06f);
        }
    }
    public IEnumerator Stage9EnemyAttack2()
    {
        // 탄막 1 발사 (파란색 쌀탄) (랜덤탄)
        for (int i = 0; i < 64; i++)
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
                bullet.AddComponent<SpriteRenderer>();
                bullet.AddComponent<CapsuleCollider2D>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
                spriteRenderer.sprite = spriteCollection[55];
                spriteRenderer.sortingOrder = 3;
                capsuleCollider2D.isTrigger = true;
                capsuleCollider2D.size = new Vector2(0.025f, 0.05f);
                capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
                capsuleCollider2D.enabled = false;
                bullet.AddComponent<InitializeBullet>();
                bullet.AddComponent<MovingBullet>();
                bullet.AddComponent<EraseBullet>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 6.0f;
                movingBullet.bulletDecelerationMoveSpeed = 0.4f;
                movingBullet.bulletDecelerationMoveSpeedMin = Random.Range(0.6f, 1.2f);
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f);
            }
            else AddBulletPool();

            yield return new WaitForSeconds(0.01f);
        }
    }

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
        StartCoroutine(CreateBulletFireEffect(301, 0.72f, 0.125f, bulletFirePosition));

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
            }
            else AddBulletPool();
        }
    }

    #endregion

    #endregion
    */
    #endregion

    #region 기타 함수

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

    #endregion

    #region 레이저 발사 및 회전 예제

    // public IEnumerator LaserRotatePattern()
    // {
    //     Vector2 playerPosition;
    // 
    //     while (true)
    //     {
    //         bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();
    //         playerPosition = GameObject.Find("PLAYER").transform.position;
    // 
    //         // 빈 탄막 발사
    //         GameObject emptyBullet = bulletManager.bulletPool.Dequeue();
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             emptyBullet.SetActive(true);
    //             ClearChild(emptyBullet);
    //             emptyBullet.transform.position = transform.position;
    //             emptyBullet.gameObject.tag = "BULLET_ENEMY_EMPTY";
    //             emptyBullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_LASER");
    //             emptyBullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
    //             if (!emptyBullet.GetComponent<SpriteRenderer>()) emptyBullet.AddComponent<SpriteRenderer>();
    //             if (!emptyBullet.GetComponent<BoxCollider2D>()) emptyBullet.AddComponent<BoxCollider2D>();
    //             emptyBullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[0];
    //             emptyBullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
    //             emptyBullet.GetComponent<BoxCollider2D>().isTrigger = true;
    //             emptyBullet.GetComponent<BoxCollider2D>().size = new Vector2(0.05f, 0.05f);
    //             if (!emptyBullet.GetComponent<InitializeBullet>()) emptyBullet.AddComponent<InitializeBullet>();
    //             if (!emptyBullet.GetComponent<MovingBullet>()) emptyBullet.AddComponent<MovingBullet>();
    //             if (!emptyBullet.GetComponent<EraseBullet>()) emptyBullet.AddComponent<EraseBullet>();
    //             if (!emptyBullet.GetComponent<LaserBullet>()) emptyBullet.AddComponent<LaserBullet>();
    //             emptyBullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_LASER_HOLD;
    //             emptyBullet.GetComponent<InitializeBullet>().bulletObject = emptyBullet.gameObject;
    //             emptyBullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
    //             emptyBullet.GetComponent<InitializeBullet>().isGrazed = false;
    //             emptyBullet.GetComponent<MovingBullet>().bulletMoveSpeed = 0.0f;
    //             emptyBullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //             emptyBullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             emptyBullet.GetComponent<LaserBullet>().laserWidth = 1.8f;
    //             emptyBullet.GetComponent<LaserBullet>().laserEnableTime = 1.0f;
    //             emptyBullet.GetComponent<LaserBullet>().laserEnableSpeed = 0.1f;
    //             emptyBullet.GetComponent<LaserBullet>().laserDisableTime = 2.5f;
    //             emptyBullet.GetComponent<LaserBullet>().laserDisableSpeed = 0.1f;
    //             emptyBullet.GetComponent<LaserBullet>().laserRotateState = BulletRotateState.BULLETROTATESTATE_NORMAL;
    //             emptyBullet.GetComponent<LaserBullet>().laserRotateSpeed = 30.0f;
    //             emptyBullet.GetComponent<LaserBullet>().isLaserRotateEnable = true;
    //             emptyBullet.GetComponent<LaserBullet>().isLaserRotateDisable = true;
    //         }
    //         else
    //         {
    //             GameObject bullet = Instantiate(bulletManager.bulletObject);
    //             bullet.SetActive(false);
    //             bullet.transform.SetParent(bulletManager.bulletParent.transform);
    //             bulletManager.bulletPool.Enqueue(bullet);
    //         }
    // 
    //         // 탄막 1 발사 (회색 고정 레이저탄) (조준탄)
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //              
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = new Vector3(transform.position.x, transform.position.y - 7.0f, transform.position.z);
    //             bullet.transform.localScale = new Vector3(1.8f, 100.0f, bullet.transform.localScale.z);
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_LASER");
    //             bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<BoxCollider2D>()) bullet.AddComponent<BoxCollider2D>();
    //             bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[358];
    //             bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
    //             bullet.GetComponent<BoxCollider2D>().isTrigger = true;
    //             bullet.GetComponent<BoxCollider2D>().size = new Vector2(0.08f, 0.14f);
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             if (!bullet.GetComponent<LaserBullet>()) bullet.AddComponent<LaserBullet>();
    //             bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_LASER_HOLD;
    //             bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
    //             bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
    //             bullet.GetComponent<InitializeBullet>().isGrazed = false;
    //             bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 0.0f;
    //             bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
    //             bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             bullet.GetComponent<LaserBullet>().laserWidth = 1.8f;
    //             bullet.GetComponent<LaserBullet>().laserEnableTime = 1.0f;
    //             bullet.GetComponent<LaserBullet>().laserEnableSpeed = 0.1f;
    //             bullet.GetComponent<LaserBullet>().laserDisableTime = 2.5f;
    //             bullet.GetComponent<LaserBullet>().laserDisableSpeed = 0.1f;
    //             bullet.transform.SetParent(emptyBullet.transform);
    //             emptyBullet.GetComponent<MovingBullet>().bulletDestination = emptyBullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
    //             float angle = Mathf.Atan2(emptyBullet.GetComponent<MovingBullet>().bulletDestination.y, emptyBullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
    //             emptyBullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle + 90.0f);
    //         }
    //         else
    //         {
    //             GameObject bullet = Instantiate(bulletManager.bulletObject);
    //             bullet.SetActive(false);
    //             bullet.transform.SetParent(bulletManager.bulletParent.transform);
    //             bulletManager.bulletPool.Enqueue(bullet);
    //         }
    // 
    //         yield return new WaitForSeconds(5.0f);
    // 
    //         // 랜덤한 지점으로 이동
    //         Vector3 targetPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);
    //         StartCoroutine(MoveToDestination(targetPosition, 1.0f));
    //         if (targetPosition.x <= 0.0f)
    //         {
    //             transform.Find("Body").GetComponent<EnemySprite>().isLeftMove = true;
    //         }
    //         else
    //         {
    //             transform.Find("Body").GetComponent<EnemySprite>().isRightMove = true;
    //         }
    // 
    //         yield return new WaitForSeconds(1.5f);
    // 
    //         playerPosition = GameObject.Find("PLAYER").transform.position;
    // 
    //         // 탄막 2 발사 (분홍색 무빙 레이저탄) (조준탄)
    //         if (bulletManager.bulletPool.Count > 0)
    //         {
    //             GameObject bullet = bulletManager.bulletPool.Dequeue();
    //              
    //             bullet.SetActive(true);
    //             ClearChild(bullet);
    //             bullet.transform.position = transform.position;
    //             bullet.gameObject.tag = "BULLET_ENEMY";
    //             bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_LASER");
    //             bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
    //             if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //             if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
    //             bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[378];
    //             bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
    //             bullet.GetComponent<CapsuleCollider2D>().isTrigger = true;
    //             bullet.GetComponent<CapsuleCollider2D>().size = new Vector2(2.4f, 0.04f);
    //             bullet.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
    //             if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //             if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //             if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //             if (!bullet.GetComponent<LaserBullet>()) bullet.AddComponent<LaserBullet>();
    //             bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_LASER_MOVE;
    //             bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
    //             bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
    //             bullet.GetComponent<InitializeBullet>().isGrazed = false;
    //             bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 1.0f;
    //             bullet.GetComponent<MovingBullet>().bulletAccelerationMoveSpeed = 0.3f;
    //             bullet.GetComponent<MovingBullet>().bulletAccelerationMoveSpeedMax = 6.0f;
    //             bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
    //             bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
    //             bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
    //             float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
    //             bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle);
    //             bullet.GetComponent<LaserBullet>().laserLength = 1.0f;
    //             StartCoroutine(bullet.GetComponent<EraseBullet>().AutoClearBullet(3.0f));
    //         }
    //         else
    //         {
    //             GameObject bullet = Instantiate(bulletManager.bulletObject);
    //             bullet.SetActive(false);
    //             bullet.transform.SetParent(bulletManager.bulletParent.transform);
    //             bulletManager.bulletPool.Enqueue(bullet);
    //         }
    // 
    //         yield return new WaitForSeconds(5.0f);
    //     }
    // }

    #endregion

}