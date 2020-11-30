using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    private GameManager gameManager;
    private EnemyStatus enemyStatus;
    private Transform[] bulletParent;
    private Transform[] bulletTransform;
    private Transform effectPool;
    private Transform effectParent;
    private Transform effectTransform;
    private GameObject bullet;
    private GameObject effect;
    private GameObject player;

    private float enemyAttackWaitTime;
    private float enemyAttackRepeatTime;

    private void Start()
    {
        gameManager = GameObject.Find("MANAGER").transform.Find("GameManager").GetComponent<GameManager>();
        enemyStatus = GetComponent<EnemyStatus>();
        bulletParent = new Transform[10];
        for (int i = 0; i < 10; i++)
        {
            bulletParent[i] = GameObject.Find("BULLET").transform.Find("EnemyBullet_Temp").transform.Find(string.Format("EnemyBullet_Temp{0}", i + 1));
        }
        bulletTransform = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            bulletTransform[i] = GameObject.Find("BULLET").transform.Find("EnemyBullet").GetChild(i);
        }
        effectPool = GameObject.Find("EFFECT").transform.Find("Effect");
        effectParent = GameObject.Find("EFFECT").transform.Find("Effect_Temp");
        effectTransform = GameObject.Find("EFFECT").transform.Find("Effect");
        player = GameObject.Find("CHARACTER").transform.Find("Player").gameObject;

        StartCoroutine(EnemyAttack(enemyAttackWaitTime, enemyAttackRepeatTime));
    }

    #region 탄막 관련

    // 원형 판정 탄막 발사
    public void CircleBulletFire
        (GameObject obj, int bulletNumber, int bulletLayer, Vector2 bulletFirePosition, Vector3 bulletScale,
        Transform bulletParent, float circleColliderRadius, float spriteAlpha, int spriteNumber,
        BulletType bulletType, GameObject targetObject, BulletSpeedState bulletSpeedState, float bulletMoveSpeed,
        float bulletAccelerationMoveSpeed, float bulletAccelerationMoveSpeedMax,
        float bulletDecelerationMoveSpeed, float bulletDecelerationMoveSpeedMin, bool bulletMoveSpeedLoopBool, bool bulletMoveSpeedLoopOnceBool,
        BulletRotateState bulletRotateState, float bulletRotateSpeed, float bulletRotateLimit,
        int bulletDestinationType, Vector2 targetPosition, float addRotateAngle, bool isSpriteRotate = false, float spriteRotateSpeed = 0.0f,
        bool isGravity = false, float velocity = 0.0f, float gravityScale = 0.0f, bool isLookAt = false)
    {
        obj.SetActive(true);
        // ClearChild(obj);
        BulletState bulletState = obj.GetComponent<BulletState>();
        BulletMove bulletMove = obj.GetComponent<BulletMove>();
        CircleCollider2D circleCollider2D = obj.GetComponent<CircleCollider2D>();
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        Rigidbody2D rigidbody2D = obj.GetComponent<Rigidbody2D>();
        bulletState.bulletObject = obj;
        bulletState.bulletNumber = bulletNumber;
        obj.gameObject.layer = bulletLayer;
        obj.transform.position = bulletFirePosition;
        obj.transform.localScale = bulletScale;
        obj.transform.SetParent(bulletParent);
        circleCollider2D.radius = circleColliderRadius;
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, spriteAlpha);
        spriteRenderer.sprite = gameManager.bulletSprite[spriteNumber];
        bulletState.bulletType = bulletType;
        bulletState.targetObject = targetObject;
        bulletState.isGrazed = false;
        bulletState.bulletSpeedState = bulletSpeedState;
        bulletState.bulletMoveSpeed = bulletMoveSpeed;
        bulletState.bulletAccelerationMoveSpeed = bulletAccelerationMoveSpeed;
        bulletState.bulletAccelerationMoveSpeedMax = bulletAccelerationMoveSpeedMax;
        bulletState.bulletDecelerationMoveSpeed = bulletDecelerationMoveSpeed;
        bulletState.bulletDecelerationMoveSpeedMin = bulletDecelerationMoveSpeedMin;
        bulletState.bulletMoveSpeedLoopBool = bulletMoveSpeedLoopBool;
        bulletState.bulletMoveSpeedLoopOnceBool = bulletMoveSpeedLoopOnceBool;
        bulletState.bulletRotateState = bulletRotateState;
        bulletState.bulletRotateSpeed = bulletRotateSpeed;
        bulletState.bulletRotateLimit = bulletRotateLimit;
        switch (bulletDestinationType)
        {
            case 1:
                bulletState.bulletDestination = bulletState.GetBulletDestination(targetPosition);
                break;
            case 2:
                bulletState.bulletDestination = bulletState.GetRandomAimedBulletDestination();
                break;
            case 3:
                bulletState.bulletDestination = bulletState.GetAimedBulletDestination();
                break;
            default:
                bulletState.bulletDestination = bulletState.GetBulletDestination(new Vector2(obj.transform.position.x, obj.transform.position.y - 5.0f));
                break;
        }
        float angle = Mathf.Atan2(bulletState.bulletDestination.y, bulletState.bulletDestination.x) * Mathf.Rad2Deg;
        bulletMove.ChangeRotateAngle(angle - 90.0f + addRotateAngle);

        if (isSpriteRotate.Equals(true))
        {
            if (!obj.GetComponent<ObjectRotate>()) obj.AddComponent<ObjectRotate>();
            ObjectRotate objectRotate = obj.GetComponent<ObjectRotate>();
            objectRotate.rotateSpeed = spriteRotateSpeed;
        }
        if (isGravity.Equals(true))
        {
            // rigidbody2D.AddForce(bullet.transform.position * velocity);
            rigidbody2D.AddForce(bulletState.bulletDestination.normalized * velocity);
            rigidbody2D.gravityScale = gravityScale;
            bulletState.isLookAt = isLookAt;
        }
    }

    // 캡슐형 판정 탄막 발사
    public void CapsuleBulletFire
        (GameObject obj, int bulletNumber, int bulletLayer, Vector2 bulletFirePosition, Vector3 bulletScale,
        Transform bulletParent, float capsuleColliderSizeX, float capsuleColliderSizeY, float capsuleColliderOffsetX, float capsuleColliderOffsetY,
        float spriteAlpha, int spriteNumber, BulletType bulletType, GameObject targetObject, BulletSpeedState bulletSpeedState, float bulletMoveSpeed,
        float bulletAccelerationMoveSpeed, float bulletAccelerationMoveSpeedMax,
        float bulletDecelerationMoveSpeed, float bulletDecelerationMoveSpeedMin, bool bulletMoveSpeedLoopBool, bool bulletMoveSpeedLoopOnceBool,
        BulletRotateState bulletRotateState, float bulletRotateSpeed, float bulletRotateLimit,
        int bulletDestinationType, Vector2 targetPosition, float addRotateAngle, bool isSpriteRotate = false, float spriteRotateSpeed = 0.0f,
        bool isGravity = false, float velocity = 0.0f, float gravityScale = 0.0f, bool isLookAt = false)
    {
        obj.SetActive(true);
        // ClearChild(obj);
        BulletState bulletState = obj.GetComponent<BulletState>();
        BulletMove bulletMove = obj.GetComponent<BulletMove>();
        CapsuleCollider2D capsuleCollider2D = obj.GetComponent<CapsuleCollider2D>();
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        Rigidbody2D rigidbody2D = obj.GetComponent<Rigidbody2D>();
        bulletState.bulletObject = obj;
        bulletState.bulletNumber = bulletNumber;
        obj.gameObject.layer = bulletLayer;
        obj.transform.position = bulletFirePosition;
        obj.transform.localScale = bulletScale;
        obj.transform.SetParent(bulletParent);
        capsuleCollider2D.size = new Vector2(capsuleColliderSizeX, capsuleColliderSizeY);
        capsuleCollider2D.offset = new Vector2(capsuleColliderOffsetX, capsuleColliderOffsetY);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, spriteAlpha);
        spriteRenderer.sprite = gameManager.bulletSprite[spriteNumber];
        bulletState.bulletType = bulletType;
        bulletState.targetObject = targetObject;
        bulletState.isGrazed = false;
        bulletState.bulletSpeedState = bulletSpeedState;
        bulletState.bulletMoveSpeed = bulletMoveSpeed;
        bulletState.bulletAccelerationMoveSpeed = bulletAccelerationMoveSpeed;
        bulletState.bulletAccelerationMoveSpeedMax = bulletAccelerationMoveSpeedMax;
        bulletState.bulletDecelerationMoveSpeed = bulletDecelerationMoveSpeed;
        bulletState.bulletDecelerationMoveSpeedMin = bulletDecelerationMoveSpeedMin;
        bulletState.bulletMoveSpeedLoopBool = bulletMoveSpeedLoopBool;
        bulletState.bulletMoveSpeedLoopOnceBool = bulletMoveSpeedLoopOnceBool;
        bulletState.bulletRotateState = bulletRotateState;
        bulletState.bulletRotateSpeed = bulletRotateSpeed;
        bulletState.bulletRotateLimit = bulletRotateLimit;
        switch (bulletDestinationType)
        {
            case 1:
                bulletState.bulletDestination = bulletState.GetBulletDestination(targetPosition);
                break;
            case 2:
                bulletState.bulletDestination = bulletState.GetRandomAimedBulletDestination();
                break;
            case 3:
                bulletState.bulletDestination = bulletState.GetAimedBulletDestination();
                break;
            default:
                bulletState.bulletDestination = bulletState.GetBulletDestination(new Vector2(obj.transform.position.x, obj.transform.position.y - 5.0f));
                break;
        }
        float angle = Mathf.Atan2(bulletState.bulletDestination.y, bulletState.bulletDestination.x) * Mathf.Rad2Deg;
        bulletMove.ChangeRotateAngle(angle - 90.0f + addRotateAngle);

        if (isSpriteRotate.Equals(true))
        {
            if (!obj.GetComponent<ObjectRotate>()) obj.AddComponent<ObjectRotate>();
            ObjectRotate objectRotate = obj.GetComponent<ObjectRotate>();
            objectRotate.rotateSpeed = spriteRotateSpeed;
        }
        if (isGravity.Equals(true))
        {
            // rigidbody2D.AddForce(bullet.transform.position * velocity);
            rigidbody2D.AddForce(bulletState.bulletDestination.normalized * velocity);
            rigidbody2D.gravityScale = gravityScale;
            bulletState.isLookAt = isLookAt;
        }
    }

    // 사각형 판정 탄막 발사
    public void BoxBulletFire
        (GameObject obj, int bulletNumber, int bulletLayer, Vector2 bulletFirePosition, Vector3 bulletScale,
        Transform bulletParent, float boxColliderSizeX, float boxColliderSizeY, float boxColliderOffsetX, float boxColliderOffsetY,
        float spriteAlpha, int spriteNumber, BulletType bulletType, GameObject targetObject, BulletSpeedState bulletSpeedState, float bulletMoveSpeed,
        float bulletAccelerationMoveSpeed, float bulletAccelerationMoveSpeedMax,
        float bulletDecelerationMoveSpeed, float bulletDecelerationMoveSpeedMin, bool bulletMoveSpeedLoopBool, bool bulletMoveSpeedLoopOnceBool,
        BulletRotateState bulletRotateState, float bulletRotateSpeed, float bulletRotateLimit,
        int bulletDestinationType, Vector2 targetPosition, float addRotateAngle, bool isSpriteRotate = false, float spriteRotateSpeed = 0.0f,
        bool isGravity = false, float velocity = 0.0f, float gravityScale = 0.0f, bool isLookAt = false)
    {
        obj.SetActive(true);
        // ClearChild(obj);
        BulletState bulletState = obj.GetComponent<BulletState>();
        BulletMove bulletMove = obj.GetComponent<BulletMove>();
        BoxCollider2D boxCollider2D = obj.GetComponent<BoxCollider2D>();
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        Rigidbody2D rigidbody2D = obj.GetComponent<Rigidbody2D>();
        bulletState.bulletObject = obj;
        bulletState.bulletNumber = bulletNumber;
        obj.gameObject.layer = bulletLayer;
        obj.transform.position = bulletFirePosition;
        obj.transform.localScale = bulletScale;
        obj.transform.SetParent(bulletParent);
        boxCollider2D.size = new Vector2(boxColliderSizeX, boxColliderSizeY);
        boxCollider2D.offset = new Vector2(boxColliderOffsetX, boxColliderOffsetY);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, spriteAlpha);
        spriteRenderer.sprite = gameManager.bulletSprite[spriteNumber];
        bulletState.bulletType = bulletType;
        bulletState.targetObject = targetObject;
        bulletState.isGrazed = false;
        bulletState.bulletSpeedState = bulletSpeedState;
        bulletState.bulletMoveSpeed = bulletMoveSpeed;
        bulletState.bulletAccelerationMoveSpeed = bulletAccelerationMoveSpeed;
        bulletState.bulletAccelerationMoveSpeedMax = bulletAccelerationMoveSpeedMax;
        bulletState.bulletDecelerationMoveSpeed = bulletDecelerationMoveSpeed;
        bulletState.bulletDecelerationMoveSpeedMin = bulletDecelerationMoveSpeedMin;
        bulletState.bulletMoveSpeedLoopBool = bulletMoveSpeedLoopBool;
        bulletState.bulletMoveSpeedLoopOnceBool = bulletMoveSpeedLoopOnceBool;
        bulletState.bulletRotateState = bulletRotateState;
        bulletState.bulletRotateSpeed = bulletRotateSpeed;
        bulletState.bulletRotateLimit = bulletRotateLimit;
        switch (bulletDestinationType)
        {
            case 1:
                bulletState.bulletDestination = bulletState.GetBulletDestination(targetPosition);
                break;
            case 2:
                bulletState.bulletDestination = bulletState.GetRandomAimedBulletDestination();
                break;
            case 3:
                bulletState.bulletDestination = bulletState.GetAimedBulletDestination();
                break;
            default:
                bulletState.bulletDestination = bulletState.GetBulletDestination(new Vector2(obj.transform.position.x, obj.transform.position.y - 5.0f));
                break;
        }
        float angle = Mathf.Atan2(bulletState.bulletDestination.y, bulletState.bulletDestination.x) * Mathf.Rad2Deg;
        bulletMove.ChangeRotateAngle(angle - 90.0f + addRotateAngle);

        if (isSpriteRotate.Equals(true))
        {
            if (!obj.GetComponent<ObjectRotate>()) obj.AddComponent<ObjectRotate>();
            ObjectRotate objectRotate = obj.GetComponent<ObjectRotate>();
            objectRotate.rotateSpeed = spriteRotateSpeed;
        }
        if (isGravity.Equals(true))
        {
            // rigidbody2D.AddForce(bullet.transform.position * velocity);
            rigidbody2D.AddForce(bulletState.bulletDestination.normalized * velocity);
            rigidbody2D.gravityScale = gravityScale;
            bulletState.isLookAt = isLookAt;
        }
    }

    #endregion

    #region 이펙트 관련

    public void CreateBulletFireEffect(GameObject effect, int spriteNumber, float scaleDownSpeed, float scaleDownTime, float alphaUpSpeed,
        Vector2 effectPosition, float scaleX = 10.0f, float scaleY = 10.0f)
    {
        SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
        effect.transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
        effect.SetActive(true);
        effect.transform.position = effectPosition;
        effect.transform.SetParent(effectParent);
        spriteRenderer.sprite = gameManager.effectSprite[spriteNumber];
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.4f);

        StartCoroutine(EffectAlphaUp(effect, alphaUpSpeed));
        StartCoroutine(EffectScaleDown(effect, scaleDownSpeed, scaleDownTime));
    }

    public void CreateBulletFireEffect(GameObject effect, int spriteNumber, float scaleUpSpeed, float scaleLimit, float alphaUpSpeed, float alphaDownSpeed,
        float alphaRemainTime, Vector2 effectPosition, float scaleX = 10.0f, float scaleY = 10.0f)
    {
        SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
        effect.transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
        effect.SetActive(true);
        effect.transform.position = effectPosition;
        effect.transform.SetParent(effectParent);
        spriteRenderer.sprite = gameManager.effectSprite[spriteNumber];
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.4f);

        StartCoroutine(EffectAlphaUp(effect, alphaUpSpeed, alphaDownSpeed, alphaRemainTime));
        StartCoroutine(EffectScaleUp(effect, scaleUpSpeed, scaleLimit));
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
        SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
        float alpha = 1.0f;

        while (true)
        {
            alpha -= alphaDownSpeed;
            if (alpha <= 0.0f) break;

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            yield return new WaitForEndOfFrame();
        }

        ClearEffect(effect);
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
        float delay = 0.0f;

        while (true)
        {
            delay += Time.deltaTime;
            if (delay >= scaleDownTime) break;

            effect.transform.localScale = new Vector3(effect.transform.localScale.x - scaleDownSpeed, effect.transform.localScale.y - scaleDownSpeed, 0.0f);

            yield return new WaitForEndOfFrame();
        }

        ClearEffect(effect);
        yield return null;
    }

    public void ClearEffect(GameObject effect)
    {
        effect.transform.SetParent(effectPool);
        effect.transform.position = new Vector2(0.0f, 0.0f);
        effect.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        effect.transform.localScale = new Vector3(10.0f, 10.0f, 1.0f);

        // 비활성화
        effect.SetActive(false);
    }

    #endregion

    #region 패턴 관련

    public IEnumerator EnemyAttack(float waitTime, float repeatTime)
    {
        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            switch (GameData.gameDifficulty)
            {
                case GameDifficulty.DIFFICULTY_EASY:
                    break;
                case GameDifficulty.DIFFICULTY_NORMAL:
                    break;
                case GameDifficulty.DIFFICULTY_HARD:
                    break;
                case GameDifficulty.DIFFICULTY_LUNATIC:
                    StartCoroutine(string.Format("Minion_Pattern{0}_Lunatic", enemyStatus.GetEnemyPatternNumber()));
                    yield return new WaitForSeconds(repeatTime);
                    break;
                default:
                    break;
            }
        }
    }

    #region 미니언 패턴

    public IEnumerator Minion_Pattern1_Lunatic()
    {
        Vector2 bulletFirePosition = transform.position;

        // 탄막 1 이펙트
        effect = effectTransform.GetChild(0).gameObject;
        CreateBulletFireEffect(effect, 3, 0.6f, 0.1f, 0.6f, bulletFirePosition);

        yield return new WaitForSeconds(0.1f);

        // 탄막 1 발사
        bullet = bulletTransform[0].GetChild(0).gameObject;
        CircleBulletFire
            (bullet, 0, LayerMask.NameToLayer("BULLET_ENEMY_INNER1"), bulletFirePosition, new Vector3(1.8f, 1.8f, 1.0f),
            bulletParent[0], 0.04f, 1.0f, 20,
            BulletType.BULLETTYPE_NORMAL, player, BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING, 4.0f,
            0.1f, 7.0f,
            0.0f, 0.0f, false, false,
            BulletRotateState.BULLETROTATESTATE_NONE, 0.0f, 0.0f,
            3, player.transform.position, 0.0f);
    }

    #endregion

    #endregion

    #region GET, SET

    public float GetEnemyAttackWaitTime()
    {
        return enemyAttackWaitTime;
    }

    public float GetEnemyAttackRepeatTime()
    {
        return enemyAttackRepeatTime;
    }

    public void SetEnemyAttackWaitTime(float time)
    {
        enemyAttackWaitTime = time;
    }

    public void SetEnemyAttackRepeatTime(float time)
    {
        enemyAttackRepeatTime = time;
    }

    #endregion
}
