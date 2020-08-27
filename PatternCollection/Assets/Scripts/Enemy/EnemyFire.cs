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
//  * (Index 298 ~ 305) 중형 발광탄 : Circle Collider 2D, Radius 0.05
//  * (Index 306 ~ 313) 대형 발광탄 : Circle Collider 2D, Radius 0.1
//  * (Index 314 ~ 321) 하트탄 : Circle Collider 2D, Radius 0.065, Offset X 0, Y -0.01
//  * (Index 322 ~ 329) 소형 발광탄 : Circle Collider 2D, Radius 0.025
//  * (Index 330 ~ 337) 화살탄 : Capsule Collider 2D, Size X 0.01, Y 0.2, Direction Vertical
//  * (Index 338 ~ 349) 음표탄 : Circle Collider 2D, Radius 0.02, Offset X -0.01, Y -0.09, Sprite Animation 적용
//  * (Index 350 ~ 357) 쉼표탄 : Capsule Collider 2D, Size X 0.02, Y 0.18
//  * (Index 358 ~ 373) 고정 레이저탄 : 기본적으로 Box Collider 2D, Size X 0.08, Y 0.14, Position Scale Y 100이며, 상황에 따라 콜라이더는 다르게 적용
//  * (Index 374 ~ 389) 무빙 레이저탄 : 기본적으로 Capsule Collider 2D, Size X 2.4, Y 0.04, Direction Horizontal이며, 상황에 따라 콜라이더는 다르게 적용

#endregion

public class EnemyFire : MonoBehaviour
{
    private BulletManager bulletManager;
    public Sprite[] spriteCollection;

    // 적 패턴 모음
    public void Fire(int stageNumber)
    {
        switch (stageNumber)
        {
            case 1:
                StartCoroutine(Stage1PatternAttack());
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
                StartCoroutine(Stage8PatternAttack());
                break;
            case 9:
                StartCoroutine(Stage9PatternAttack1());
                StartCoroutine(Stage9PatternAttack2());
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
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();

            // 탄막 1 발사 (파란색 환탄) (조준 방사탄)
            for (int i = 0; i < 16; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[261];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.08f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    if (!bullet.GetComponent<Stage1BulletFragmentation>()) bullet.AddComponent<Stage1BulletFragmentation>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
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
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();

            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (좌측 상단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = new Vector2(Random.Range(-6.0f, -5.0f), Random.Range(-4.5f, 1.5f));
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[268];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
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
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }
            }
            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (좌측 하단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = new Vector2(Random.Range(-6.0f, -5.0f), Random.Range(1.5f, 7.5f));
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[268];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
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
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }
            }
            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (우측 상단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = new Vector2(Random.Range(5.0f, 6.0f), Random.Range(-4.5f, 1.5f));
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[268];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
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
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }
            }
            // 탄막 발사 (분홍색 나비탄) (랜덤탄) (우측 하단)
            for (int i = 0; i < 8; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = new Vector2(Random.Range(5.0f, 6.0f), Random.Range(1.5f, 7.5f));
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[268];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
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
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
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
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();

            for (int i = 0; i < 5; i++)
            {
                // 빈 탄막 발사
                // GameObject emptyBullet = bulletManager.bulletPool.Dequeue();
                GameObject emptyBullet = bulletManager.bulletPool.Pop();

                if (bulletManager.bulletPool.Count > 0)
                {
                    emptyBullet.SetActive(true);
                    ClearChild(emptyBullet);
                    emptyBullet.transform.position = transform.position;
                    emptyBullet.gameObject.tag = "BULLET_ENEMY_EMPTY";
                    emptyBullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    emptyBullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!emptyBullet.GetComponent<SpriteRenderer>()) emptyBullet.AddComponent<SpriteRenderer>();
                    if (!emptyBullet.GetComponent<CircleCollider2D>()) emptyBullet.AddComponent<CircleCollider2D>();
                    emptyBullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[0];
                    emptyBullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    emptyBullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    emptyBullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    if (!emptyBullet.GetComponent<InitializeBullet>()) emptyBullet.AddComponent<InitializeBullet>();
                    if (!emptyBullet.GetComponent<MovingBullet>()) emptyBullet.AddComponent<MovingBullet>();
                    if (!emptyBullet.GetComponent<EraseBullet>()) emptyBullet.AddComponent<EraseBullet>();
                    emptyBullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    emptyBullet.GetComponent<InitializeBullet>().bulletObject = emptyBullet.gameObject;
                    emptyBullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    emptyBullet.GetComponent<InitializeBullet>().isGrazed = true;
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
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }

                // 탄막 발사 (하늘색 나비탄) (빈 탄막을 중심으로 회전)
                for (int j = 0; j < 12; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
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
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
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
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();

            for (int i = 0; i < 18; i++)
            {
                // 탄막 1 발사 (무작위 색상 원탄) (랜덤탄)
                for (int j = 0; j < 18; j++)
                {
                    int bulletTextureIndex1 = Random.Range(17, 32);
                    
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = gameObject.transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[bulletTextureIndex1];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.04f;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
                    }
                }
                
                yield return new WaitForSeconds(0.04f);
            }

            yield return new WaitForSeconds(0.2f);

            int bulletTextureIndex2 = Random.Range(97, 112);
            Vector2 targetPosition = GameObject.Find("PLAYER").transform.position;
            
            // 탄막 2 발사 (무작위 색상 쐐기탄) (조준 방사탄)
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[bulletTextureIndex2];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CapsuleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CapsuleCollider2D>().size = new Vector2(0.04f, 0.06f);
                        bullet.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Vertical;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
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
                    else
                    {
                        GameObject bullet = Instantiate(bulletManager.bulletObject);
                        bullet.SetActive(false);
                        bullet.transform.SetParent(bulletManager.bulletParent.transform);
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
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
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();

            // 대옥탄에서 생성된 탄 활성화
            for (int i = 0; i < GameObject.Find("BULLET").transform.Find("EnemyBulletTemp1").childCount; i++)
            {
                GameObject bullet = GameObject.Find("BULLET").transform.Find("EnemyBulletTemp1").transform.GetChild(i).gameObject;
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
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[241];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.135f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    if (!bullet.GetComponent<Stage5BulletCreate>()) bullet.AddComponent<Stage5BulletCreate>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
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
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
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
        while (true)
        {
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8 * (i + 1); j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[18 + (i % 2)];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.04f;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
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
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();

            // 플레이어 조준 발사 6회
            for (int i = 0; i < 6; i++)
            {
                // 탄막 1 발사 (빨간색 대옥탄)
                if (bulletManager.bulletPool.Count > 0)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[241];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.135f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }

                // 탄막 2 발사 (빨간색 대형 환탄)
                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[259];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.08f;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
                    }
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[35];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
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
                if (bulletManager.bulletPool.Count > 0)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[241];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.135f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }

                // 탄막 2 발사 (빨간색 대형 환탄)
                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[259];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.08f;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
                    }
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                for (int j = 0; j < 16; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[35];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
                    }
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
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[241];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.135f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }

                // 탄막 2 발사 (빨간색 대형 환탄)
                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[259];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.08f;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
                    }
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                for (int j = 0; j < 12; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[35];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
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
                if (bulletManager.bulletPool.Count > 0)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[241];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.135f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }

                // 탄막 2 발사 (빨간색 대형 환탄)
                for (int j = 0; j < 8; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[259];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.08f;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
                    }
                }

                // 탄막 3 발사 (빨간색 테두리 원탄)
                for (int j = 0; j < 24; j++)
                {
                    if (bulletManager.bulletPool.Count > 0)
                    {
                        // GameObject bullet = bulletManager.bulletPool.Dequeue();
                        GameObject bullet = bulletManager.bulletPool.Pop();
                        bullet.SetActive(true);
                        ClearChild(bullet);
                        bullet.transform.position = transform.position;
                        bullet.gameObject.tag = "BULLET_ENEMY";
                        bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                        bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                        if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                        if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                        bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[35];
                        bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                        bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                        if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                        if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                        if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                        bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                        bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                        bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                        bullet.GetComponent<InitializeBullet>().isGrazed = false;
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
                        // bulletManager.bulletPool.Enqueue(bullet);
                        bulletManager.bulletPool.Push(bullet);
                    }
                }

                yield return new WaitForSeconds(0.06f);
            }

            yield return new WaitForSeconds(2.0f);
        }
    }
    #endregion

    #region 패턴 8 (동방홍마향 1스테이지 - 루미아 1스펠 "나이트 버드" 리메이크
    public IEnumerator Stage8PatternAttack()
    {
        Vector2 playerPosition;

        while (true)
        {
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();

            for (int i = 0; i < 4; i++)
            {
                playerPosition = GameObject.Find("PLAYER").transform.position;

                // 탄막 1 발사 (파란색 테두리원탄) (조준 방사탄)
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (bulletManager.bulletPool.Count > 0)
                        {
                            // GameObject bullet = bulletManager.bulletPool.Dequeue();
                            GameObject bullet = bulletManager.bulletPool.Pop();
                            bullet.SetActive(true);
                            ClearChild(bullet);
                            bullet.transform.position = transform.position;
                            bullet.gameObject.tag = "BULLET_ENEMY";
                            bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                            bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                            if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                            if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                            bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[39];
                            bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                            bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                            bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                            if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                            if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                            if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                            bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                            bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                            bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                            bullet.GetComponent<InitializeBullet>().isGrazed = false;
                            bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 2.0f + (0.3f * k);
                            bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                            bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                            bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                            float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                            bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f - 33.75f + (7.5f * j));
                        }
                        else
                        {
                            GameObject bullet = Instantiate(bulletManager.bulletObject);
                            bullet.SetActive(false);
                            bullet.transform.SetParent(bulletManager.bulletParent.transform);
                            // bulletManager.bulletPool.Enqueue(bullet);
                            bulletManager.bulletPool.Push(bullet);
                        }
                    }

                    yield return new WaitForSeconds(0.02f);
                }

                yield return new WaitForSeconds(0.03f);

                playerPosition = GameObject.Find("PLAYER").transform.position;

                // 탄막 2 발사 (하늘색 테두리원탄) (조준 방사탄)
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (bulletManager.bulletPool.Count > 0)
                        {
                            // GameObject bullet = bulletManager.bulletPool.Dequeue();
                            GameObject bullet = bulletManager.bulletPool.Pop();
                            bullet.SetActive(true);
                            ClearChild(bullet);
                            bullet.transform.position = transform.position;
                            bullet.gameObject.tag = "BULLET_ENEMY";
                            bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                            bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                            if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                            if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                            bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[41];
                            bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                            bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                            bullet.GetComponent<CircleCollider2D>().radius = 0.03f;
                            if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                            if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                            if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                            bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                            bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                            bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                            bullet.GetComponent<InitializeBullet>().isGrazed = false;
                            bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 2.4f + (0.3f * k);
                            bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                            bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                            bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                            float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                            bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + 30.0f - (7.5f * j));
                        }
                        else
                        {
                            GameObject bullet = Instantiate(bulletManager.bulletObject);
                            bullet.SetActive(false);
                            bullet.transform.SetParent(bulletManager.bulletParent.transform);
                            // bulletManager.bulletPool.Enqueue(bullet);
                            bulletManager.bulletPool.Push(bullet);
                        }
                    }

                    yield return new WaitForSeconds(0.02f);
                }

                yield return new WaitForSeconds(0.03f);
            }

            // 랜덤한 지점으로 이동
            Vector3 targetPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);
            StartCoroutine(MoveToDestination(targetPosition, 1.0f));
            if (targetPosition.x <= 0.0f)
            {
                transform.Find("Body").GetComponent<EnemySprite>().isLeftMove = true;
            }
            else
            {
                transform.Find("Body").GetComponent<EnemySprite>().isRightMove = true;
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
    #endregion

    #region 패턴 9 (동방홍마향 1스테이지 - 루미아 중간보스 스펠 "문라이트 레이")
    public IEnumerator Stage9PatternAttack1()
    {
        while (true)
        {
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();

            // 레이저 발사
            for (int i = 0; i < 2; i++)
            {
                // 빈 탄막 발사
                // GameObject emptyBullet = bulletManager.bulletPool.Dequeue();
                GameObject emptyBullet = bulletManager.bulletPool.Pop();

                if (bulletManager.bulletPool.Count > 0)
                {
                    emptyBullet.SetActive(true);
                    ClearChild(emptyBullet);
                    emptyBullet.transform.position = transform.position;
                    emptyBullet.gameObject.tag = "BULLET_ENEMY_EMPTY";
                    emptyBullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_LASER");
                    emptyBullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!emptyBullet.GetComponent<SpriteRenderer>()) emptyBullet.AddComponent<SpriteRenderer>();
                    if (!emptyBullet.GetComponent<BoxCollider2D>()) emptyBullet.AddComponent<BoxCollider2D>();
                    emptyBullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[0];
                    emptyBullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    emptyBullet.GetComponent<BoxCollider2D>().isTrigger = true;
                    emptyBullet.GetComponent<BoxCollider2D>().size = new Vector2(0.05f, 0.05f);
                    if (!emptyBullet.GetComponent<InitializeBullet>()) emptyBullet.AddComponent<InitializeBullet>();
                    if (!emptyBullet.GetComponent<MovingBullet>()) emptyBullet.AddComponent<MovingBullet>();
                    if (!emptyBullet.GetComponent<EraseBullet>()) emptyBullet.AddComponent<EraseBullet>();
                    if (!emptyBullet.GetComponent<LaserBullet>()) emptyBullet.AddComponent<LaserBullet>();
                    emptyBullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_LASER_HOLD;
                    emptyBullet.GetComponent<InitializeBullet>().bulletObject = emptyBullet.gameObject;
                    emptyBullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    emptyBullet.GetComponent<InitializeBullet>().isGrazed = false;
                    emptyBullet.GetComponent<MovingBullet>().bulletMoveSpeed = 0.0f;
                    emptyBullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    emptyBullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    emptyBullet.GetComponent<LaserBullet>().laserWidth = 1.8f;
                    emptyBullet.GetComponent<LaserBullet>().laserEnableTime = 0.0f;
                    emptyBullet.GetComponent<LaserBullet>().laserEnableSpeed = 0.1f;
                    emptyBullet.GetComponent<LaserBullet>().laserDisableTime = 1.8f;
                    emptyBullet.GetComponent<LaserBullet>().laserDisableSpeed = 0.1f;
                    emptyBullet.GetComponent<LaserBullet>().laserRotateState = BulletRotateState.BULLETROTATESTATE_NORMAL;
                    if (i == 0)
                    {
                        emptyBullet.GetComponent<LaserBullet>().laserRotateSpeed = 20.0f;
                    }
                    else
                    {
                        emptyBullet.GetComponent<LaserBullet>().laserRotateSpeed = -20.0f;
                    }
                    emptyBullet.GetComponent<LaserBullet>().isLaserRotateEnable = true;
                    emptyBullet.GetComponent<LaserBullet>().isLaserRotateDisable = true;
                }
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }

                // 탄막 발사 (회색 고정 레이저탄) (지점 조준 회전탄)
                if (bulletManager.bulletPool.Count > 0)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = new Vector3(transform.position.x, transform.position.y - 7.0f, transform.position.z);
                    bullet.transform.localScale = new Vector3(1.8f, 100.0f, bullet.transform.localScale.z);
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_LASER");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<BoxCollider2D>()) bullet.AddComponent<BoxCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[358];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<BoxCollider2D>().isTrigger = true;
                    bullet.GetComponent<BoxCollider2D>().size = new Vector2(0.08f, 0.14f);
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    if (!bullet.GetComponent<LaserBullet>()) bullet.AddComponent<LaserBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_LASER_HOLD;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 0.0f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<LaserBullet>().laserWidth = 1.8f;
                    bullet.GetComponent<LaserBullet>().laserEnableTime = 0.0f;
                    bullet.GetComponent<LaserBullet>().laserEnableSpeed = 0.1f;
                    bullet.GetComponent<LaserBullet>().laserDisableTime = 1.8f;
                    bullet.GetComponent<LaserBullet>().laserDisableSpeed = 0.1f;
                    bullet.transform.SetParent(emptyBullet.transform);
                    emptyBullet.GetComponent<MovingBullet>().bulletDestination = emptyBullet.GetComponent<InitializeBullet>().GetAimedBulletDestination
                        (new Vector2(transform.position.x, transform.position.y - 10.0f));
                    float angle = Mathf.Atan2(emptyBullet.GetComponent<MovingBullet>().bulletDestination.y, emptyBullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    if (i == 0)
                    {
                        emptyBullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle + 30.0f);
                    }
                    else
                    {
                        emptyBullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle + 150.0f);
                    }
                }
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }
            }

            yield return new WaitForSeconds(5.4f);

            // 랜덤한 지점으로 이동
            Vector3 targetPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);
            StartCoroutine(MoveToDestination(targetPosition, 1.0f));
            if (targetPosition.x <= 0.0f)
            {
                transform.Find("Body").GetComponent<EnemySprite>().isLeftMove = true;
            }
            else
            {
                transform.Find("Body").GetComponent<EnemySprite>().isRightMove = true;
            }

            yield return new WaitForSeconds(1.5f);

        }
    }
    public IEnumerator Stage9PatternAttack2()
    {
        Vector2 playerPosition;

        while (true)
        {
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();
            playerPosition = GameObject.Find("PLAYER").transform.position;

            // 탄막 발사 (회색 콩알탄) (조준 방사탄)
            for (int i = 0; i < 72; i++)
            {
                if (bulletManager.bulletPool.Count > 0)
                {
                    // GameObject bullet = bulletManager.bulletPool.Dequeue();
                    GameObject bullet = bulletManager.bulletPool.Pop();
                    bullet.SetActive(true);
                    ClearChild(bullet);
                    bullet.transform.position = transform.position;
                    bullet.gameObject.tag = "BULLET_ENEMY";
                    bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1");
                    bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                    if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                    if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                    bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[1];
                    bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    bullet.GetComponent<CircleCollider2D>().isTrigger = true;
                    bullet.GetComponent<CircleCollider2D>().radius = 0.02f;
                    if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                    if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                    if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                    bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_NORMAL;
                    bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                    bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                    bullet.GetComponent<InitializeBullet>().isGrazed = false;
                    bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 4.0f;
                    bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                    bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination();
                    float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                    bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle - 90.0f + (5.0f * i));
                }
                else
                {
                    GameObject bullet = Instantiate(bulletManager.bulletObject);
                    bullet.SetActive(false);
                    bullet.transform.SetParent(bulletManager.bulletParent.transform);
                    // bulletManager.bulletPool.Enqueue(bullet);
                    bulletManager.bulletPool.Push(bullet);
                }
            }
            
            yield return new WaitForSeconds(0.3f);
        }
    }
    #endregion

    #region 테스트 전용 패턴

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

    #region 차일드 제거 함수

    public static void ClearChild(GameObject bullet)
    {
        if (bullet.transform.childCount > 0)
        {
            for (int k = 0; k < bullet.transform.childCount; k++)
            {
                if (bullet.transform.GetChild(k).gameObject.activeSelf == true)
                {
                    bullet.transform.GetChild(k).GetComponent<EraseBullet>().ClearBullet();
                }
            }
        }
    }

    #endregion

    #endregion


    #region 레이저 발사 및 회전 예제
    public IEnumerator LaserRotatePattern()
    {
        Vector2 playerPosition;

        while (true)
        {
            bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();
            playerPosition = GameObject.Find("PLAYER").transform.position;

            // 빈 탄막 발사
            // GameObject emptyBullet = bulletManager.bulletPool.Dequeue();
            GameObject emptyBullet = bulletManager.bulletPool.Pop();

            if (bulletManager.bulletPool.Count > 0)
            {
                emptyBullet.SetActive(true);
                ClearChild(emptyBullet);
                emptyBullet.transform.position = transform.position;
                emptyBullet.gameObject.tag = "BULLET_ENEMY_EMPTY";
                emptyBullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_LASER");
                emptyBullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                if (!emptyBullet.GetComponent<SpriteRenderer>()) emptyBullet.AddComponent<SpriteRenderer>();
                if (!emptyBullet.GetComponent<BoxCollider2D>()) emptyBullet.AddComponent<BoxCollider2D>();
                emptyBullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[0];
                emptyBullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                emptyBullet.GetComponent<BoxCollider2D>().isTrigger = true;
                emptyBullet.GetComponent<BoxCollider2D>().size = new Vector2(0.05f, 0.05f);
                if (!emptyBullet.GetComponent<InitializeBullet>()) emptyBullet.AddComponent<InitializeBullet>();
                if (!emptyBullet.GetComponent<MovingBullet>()) emptyBullet.AddComponent<MovingBullet>();
                if (!emptyBullet.GetComponent<EraseBullet>()) emptyBullet.AddComponent<EraseBullet>();
                if (!emptyBullet.GetComponent<LaserBullet>()) emptyBullet.AddComponent<LaserBullet>();
                emptyBullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_LASER_HOLD;
                emptyBullet.GetComponent<InitializeBullet>().bulletObject = emptyBullet.gameObject;
                emptyBullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                emptyBullet.GetComponent<InitializeBullet>().isGrazed = false;
                emptyBullet.GetComponent<MovingBullet>().bulletMoveSpeed = 0.0f;
                emptyBullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                emptyBullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                emptyBullet.GetComponent<LaserBullet>().laserWidth = 1.8f;
                emptyBullet.GetComponent<LaserBullet>().laserEnableTime = 1.0f;
                emptyBullet.GetComponent<LaserBullet>().laserEnableSpeed = 0.1f;
                emptyBullet.GetComponent<LaserBullet>().laserDisableTime = 2.5f;
                emptyBullet.GetComponent<LaserBullet>().laserDisableSpeed = 0.1f;
                emptyBullet.GetComponent<LaserBullet>().laserRotateState = BulletRotateState.BULLETROTATESTATE_NORMAL;
                emptyBullet.GetComponent<LaserBullet>().laserRotateSpeed = 30.0f;
                emptyBullet.GetComponent<LaserBullet>().isLaserRotateEnable = true;
                emptyBullet.GetComponent<LaserBullet>().isLaserRotateDisable = true;
            }
            else
            {
                GameObject bullet = Instantiate(bulletManager.bulletObject);
                bullet.SetActive(false);
                bullet.transform.SetParent(bulletManager.bulletParent.transform);
                // bulletManager.bulletPool.Enqueue(bullet);
                bulletManager.bulletPool.Push(bullet);
            }

            // 탄막 1 발사 (회색 고정 레이저탄) (조준탄)
            if (bulletManager.bulletPool.Count > 0)
            {
                // GameObject bullet = bulletManager.bulletPool.Dequeue();
                GameObject bullet = bulletManager.bulletPool.Pop();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = new Vector3(transform.position.x, transform.position.y - 7.0f, transform.position.z);
                bullet.transform.localScale = new Vector3(1.8f, 100.0f, bullet.transform.localScale.z);
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_LASER");
                bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<BoxCollider2D>()) bullet.AddComponent<BoxCollider2D>();
                bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[358];
                bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                bullet.GetComponent<BoxCollider2D>().isTrigger = true;
                bullet.GetComponent<BoxCollider2D>().size = new Vector2(0.08f, 0.14f);
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                if (!bullet.GetComponent<LaserBullet>()) bullet.AddComponent<LaserBullet>();
                bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_LASER_HOLD;
                bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                bullet.GetComponent<InitializeBullet>().isGrazed = false;
                bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 0.0f;
                bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                bullet.GetComponent<LaserBullet>().laserWidth = 1.8f;
                bullet.GetComponent<LaserBullet>().laserEnableTime = 1.0f;
                bullet.GetComponent<LaserBullet>().laserEnableSpeed = 0.1f;
                bullet.GetComponent<LaserBullet>().laserDisableTime = 2.5f;
                bullet.GetComponent<LaserBullet>().laserDisableSpeed = 0.1f;
                bullet.transform.SetParent(emptyBullet.transform);
                emptyBullet.GetComponent<MovingBullet>().bulletDestination = emptyBullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                float angle = Mathf.Atan2(emptyBullet.GetComponent<MovingBullet>().bulletDestination.y, emptyBullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                emptyBullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle + 90.0f);
            }
            else
            {
                GameObject bullet = Instantiate(bulletManager.bulletObject);
                bullet.SetActive(false);
                bullet.transform.SetParent(bulletManager.bulletParent.transform);
                // bulletManager.bulletPool.Enqueue(bullet);
                bulletManager.bulletPool.Push(bullet);
            }

            yield return new WaitForSeconds(5.0f);

            // 랜덤한 지점으로 이동
            Vector3 targetPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.0f), 0.0f);
            StartCoroutine(MoveToDestination(targetPosition, 1.0f));
            if (targetPosition.x <= 0.0f)
            {
                transform.Find("Body").GetComponent<EnemySprite>().isLeftMove = true;
            }
            else
            {
                transform.Find("Body").GetComponent<EnemySprite>().isRightMove = true;
            }

            yield return new WaitForSeconds(1.5f);

            playerPosition = GameObject.Find("PLAYER").transform.position;

            // 탄막 2 발사 (분홍색 무빙 레이저탄) (조준탄)
            if (bulletManager.bulletPool.Count > 0)
            {
                // GameObject bullet = bulletManager.bulletPool.Dequeue();
                GameObject bullet = bulletManager.bulletPool.Pop();
                bullet.SetActive(true);
                ClearChild(bullet);
                bullet.transform.position = transform.position;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_LASER");
                bullet.transform.SetParent(GameObject.Find("BULLET").transform.Find("EnemyBullet"));
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
                bullet.GetComponent<SpriteRenderer>().sprite = spriteCollection[378];
                bullet.GetComponent<SpriteRenderer>().sortingOrder = 3;
                bullet.GetComponent<CapsuleCollider2D>().isTrigger = true;
                bullet.GetComponent<CapsuleCollider2D>().size = new Vector2(2.4f, 0.04f);
                bullet.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                if (!bullet.GetComponent<LaserBullet>()) bullet.AddComponent<LaserBullet>();
                bullet.GetComponent<InitializeBullet>().bulletType = BulletType.BULLETTYPE_LASER_MOVE;
                bullet.GetComponent<InitializeBullet>().bulletObject = bullet.gameObject;
                bullet.GetComponent<InitializeBullet>().targetObject = GameObject.Find("PLAYER");
                bullet.GetComponent<InitializeBullet>().isGrazed = false;
                bullet.GetComponent<MovingBullet>().bulletMoveSpeed = 1.0f;
                bullet.GetComponent<MovingBullet>().bulletAccelerationMoveSpeed = 0.3f;
                bullet.GetComponent<MovingBullet>().bulletAccelerationMoveSpeedMax = 6.0f;
                bullet.GetComponent<MovingBullet>().bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
                bullet.GetComponent<MovingBullet>().bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                bullet.GetComponent<MovingBullet>().bulletDestination = bullet.GetComponent<InitializeBullet>().GetAimedBulletDestination(playerPosition);
                float angle = Mathf.Atan2(bullet.GetComponent<MovingBullet>().bulletDestination.y, bullet.GetComponent<MovingBullet>().bulletDestination.x) * Mathf.Rad2Deg;
                bullet.GetComponent<MovingBullet>().ChangeRotateAngle(angle);
                bullet.GetComponent<LaserBullet>().laserLength = 1.0f;
                StartCoroutine(bullet.GetComponent<EraseBullet>().AutoClearBullet(3.0f));
            }
            else
            {
                GameObject bullet = Instantiate(bulletManager.bulletObject);
                bullet.SetActive(false);
                bullet.transform.SetParent(bulletManager.bulletParent.transform);
                // bulletManager.bulletPool.Enqueue(bullet);
                bulletManager.bulletPool.Push(bullet);
            }

            yield return new WaitForSeconds(5.0f);
        }
    }
    #endregion
}