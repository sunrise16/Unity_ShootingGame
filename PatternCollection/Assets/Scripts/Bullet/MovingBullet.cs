using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum BulletSpeedState
{
    BULLETSPEEDSTATE_NORMAL,
    BULLETSPEEDSTATE_ACCELERATING,
    BULLETSPEEDSTATE_DECELERATING,
    BULLETSPEEDSTATE_LOOP,
    BULLETSPEEDSTATE_LOOPONCE,
}

public enum BulletRotateState
{
    BULLETROTATESTATE_NONE,
    BULLETROTATESTATE_NORMAL,
    BULLETROTATESTATE_LIMIT,
    BULLETROTATESTATE_ROTATEAROUND,
    BULLETROTATESTATE_LOOKAT,
}

public class MovingBullet : MonoBehaviour
{
    private CircleCollider2D circleCollider2D;
    private CapsuleCollider2D capsuleCollider2D;
    private BoxCollider2D boxCollider2D;
    private GameManager gameManager;
    private GameObject playerObject;
    private InitializeBullet initializeBullet;
    private MovingBullet movingBullet;
    private PlayerDatabase playerDatabase;

    public BulletSpeedState bulletSpeedState;
    public BulletRotateState bulletRotateState;
    public Vector2 bulletDestination;

    // 탄속 관련
    public float bulletMoveSpeed;
    public bool bulletMoveSpeedLoopBool;
    public float bulletAccelerationMoveSpeed;
    public float bulletAccelerationMoveSpeedMax;
    public float bulletDecelerationMoveSpeed;
    public float bulletDecelerationMoveSpeedMin;

    // 탄도 관련
    public float bulletRotateTime;
    public float bulletRotateLimit;
    public float bulletRotateSpeed;

    // 그레이즈 관련
    private float grazeDelay;
    private bool isGrazing;

    void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        gameManager = GameObject.Find("MANAGER").transform.GetChild(0).GetComponent<GameManager>();
        playerObject = GameObject.Find("PLAYER");
        initializeBullet = GetComponent<InitializeBullet>();
        movingBullet = GetComponent<MovingBullet>();
        playerDatabase = playerObject.GetComponent<PlayerDatabase>();

        bulletRotateTime = 0.0f;
        grazeDelay = 0.0f;
        isGrazing = false;
    }

    void Update()
    {
        bulletRotateTime += Time.deltaTime;

        // 탄속 변경
        if (bulletSpeedState.Equals(BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING))
        {
            bulletMoveSpeed += bulletAccelerationMoveSpeed;
            if (!bulletAccelerationMoveSpeedMax.Equals(0.0f))
            {
                if (bulletMoveSpeed >= bulletAccelerationMoveSpeedMax)
                {
                    bulletMoveSpeed = bulletAccelerationMoveSpeedMax;
                }
            }
        }
        else if (bulletSpeedState.Equals(BulletSpeedState.BULLETSPEEDSTATE_DECELERATING))
        {
            bulletMoveSpeed -= bulletDecelerationMoveSpeed;
            if (!bulletDecelerationMoveSpeedMin.Equals(0.0f))
            {
                if (bulletMoveSpeed <= bulletDecelerationMoveSpeedMin)
                {
                    bulletMoveSpeed = bulletDecelerationMoveSpeedMin;
                }
            }
            else
            {
                if (bulletMoveSpeed <= 0.0f)
                {
                    bulletMoveSpeed = 0.0f;
                }
            }
        }
        else if (bulletSpeedState.Equals(BulletSpeedState.BULLETSPEEDSTATE_LOOP))
        {
            if (bulletMoveSpeedLoopBool.Equals(true))
            {
                bulletMoveSpeed += bulletAccelerationMoveSpeed;
                if (bulletMoveSpeed >= bulletAccelerationMoveSpeedMax)
                {
                    bulletMoveSpeedLoopBool = false;
                }
            }
            else
            {
                bulletMoveSpeed -= bulletDecelerationMoveSpeed;
                if (bulletMoveSpeed <= bulletDecelerationMoveSpeedMin)
                {
                    bulletMoveSpeedLoopBool = true;
                }
            }
        }
        else if (bulletSpeedState.Equals(BulletSpeedState.BULLETSPEEDSTATE_LOOPONCE))
        {
            if (bulletMoveSpeedLoopBool.Equals(true))
            {
                bulletMoveSpeed += bulletAccelerationMoveSpeed;
                if (bulletMoveSpeed >= bulletAccelerationMoveSpeedMax)
                {
                    bulletMoveSpeedLoopBool = false;
                    bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                }
            }
            else
            {
                bulletMoveSpeed -= bulletDecelerationMoveSpeed;
                if (bulletMoveSpeed <= bulletDecelerationMoveSpeedMin)
                {
                    bulletMoveSpeedLoopBool = true;
                    bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
                }
            }
        }

        // 탄도 변경 (지속 증감)
        if (!bulletRotateSpeed.Equals(0.0f))
        {
            if (bulletRotateState.Equals(BulletRotateState.BULLETROTATESTATE_NORMAL))
            {
                transform.Rotate(Vector3.forward * bulletRotateSpeed * Time.deltaTime);
            }
            else if (bulletRotateState.Equals(BulletRotateState.BULLETROTATESTATE_LIMIT))
            {
                transform.Rotate(Vector3.forward * bulletRotateSpeed * Time.deltaTime);
                if (bulletRotateTime >= bulletRotateLimit)
                {
                    bulletRotateSpeed = 0.0f;
                }
            }
            else if (bulletRotateState.Equals(BulletRotateState.BULLETROTATESTATE_ROTATEAROUND))
            {
                RotateAround(bulletDestination);
            }
        }
        // 탄도 변경 (지속적으로 대상을 향해 바라보기)
        else if (bulletRotateState.Equals(BulletRotateState.BULLETROTATESTATE_LOOKAT))
        {
            bulletDestination = initializeBullet.GetAimedBulletDestination(initializeBullet.targetObject.transform.position);
            float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
            if (initializeBullet.bulletType.Equals(BulletType.BULLETTYPE_LASER_MOVE))
            {
                ChangeRotateAngle(angle - 180.0f);
            }
            else
            {
                ChangeRotateAngle(angle - 90.0f);
            }
        }

        // 탄막 이동
        if (initializeBullet.bulletType.Equals(BulletType.BULLETTYPE_NORMAL))
        {
            transform.Translate(Vector2.up * bulletMoveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.right * bulletMoveSpeed * Time.deltaTime);
        }

        // 탄막 그레이즈
        if (isGrazing.Equals(true))
        {
            grazeDelay += Time.deltaTime;
            if (grazeDelay >= 0.1f)
            {
                playerDatabase.grazeCount++;
                grazeDelay = 0.0f;
            }
        }
        else
        {
            grazeDelay = 0.0f;
        }

        // 콜라이더 활성화
        if (Vector2.Distance(transform.position, playerObject.transform.position) <= 0.5f)
        {
            if (!circleCollider2D.Equals(null))
            {
                circleCollider2D.enabled = true;
            }
            else if (!capsuleCollider2D.Equals(null))
            {
                capsuleCollider2D.enabled = true;
            }
            else if (!boxCollider2D.Equals(null))
            {
                boxCollider2D.enabled = true;
            }
        }
        else
        {
            Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            if (((targetScreenPos.x > Screen.width - 0.5f || targetScreenPos.x < 0.5f) || (targetScreenPos.y > Screen.height - 0.5f || targetScreenPos.y < 0.5f)) ||
                (gameManager.isCleared.Equals(true)))
            {
                if (!circleCollider2D.Equals(null))
                {
                    circleCollider2D.enabled = true;
                }
                else if (!capsuleCollider2D.Equals(null))
                {
                    capsuleCollider2D.enabled = true;
                }
                else if (!boxCollider2D.Equals(null))
                {
                    boxCollider2D.enabled = true;
                }
            }
        }
    }

    // 탄도 산출
    public float GetAngle()
    {
        return transform.eulerAngles.z;
    }

    // 탄도 변경 (일시적 변화)
    public void ChangeRotateAngle(float angle)
    {
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // 탄도 변경 (공전)
    public void RotateAround(Vector2 target)
    {
        transform.RotateAround(target, Vector3.forward, bulletRotateSpeed * Time.deltaTime);
    }

    // 탄막 그레이즈
    private void OnTriggerEnter2D(Collider2D collision)
    {
        InitializeBullet initializeBullet = gameObject.GetComponent<InitializeBullet>();

        if ((CompareTag("BULLET_ENEMY").Equals(true) && collision.CompareTag("GRAZECIRCLE").Equals(true)) && initializeBullet.isGrazed.Equals(false))
        {
            if (gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_ENEMY_LASER")))
            {
                grazeDelay = 0.099f;
                isGrazing = true;
            }
            else
            {
                playerDatabase.grazeCount++;
                initializeBullet.isGrazed = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        InitializeBullet initializeBullet = gameObject.GetComponent<InitializeBullet>();
        grazeDelay = 0.0f;
        isGrazing = false;

        if (gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_ENEMY_LASER")) && collision.CompareTag("GRAZECIRCLE"))
        {
            if (initializeBullet.bulletType.Equals(BulletType.BULLETTYPE_LASER_MOVE))
            {
                initializeBullet.isGrazed = true;
            }
        }
    }
}
