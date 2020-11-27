using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BulletMove : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private BulletState bulletState;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        bulletState = GetComponent<BulletState>();
    }

    private void Update()
    {
        bulletState.bulletRotateTime += Time.deltaTime;

        // 탄속 변경
        if (bulletState.bulletSpeedState.Equals(BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING))
        {
            bulletState.bulletMoveSpeed += bulletState.bulletAccelerationMoveSpeed;
            if (!bulletState.bulletAccelerationMoveSpeedMax.Equals(0.0f))
            {
                if (bulletState.bulletMoveSpeed >= bulletState.bulletAccelerationMoveSpeedMax)
                {
                    bulletState.bulletMoveSpeed = bulletState.bulletAccelerationMoveSpeedMax;
                }
            }
        }
        else if (bulletState.bulletSpeedState.Equals(BulletSpeedState.BULLETSPEEDSTATE_DECELERATING))
        {
            bulletState.bulletMoveSpeed -= bulletState.bulletDecelerationMoveSpeed;
            if (!bulletState.bulletDecelerationMoveSpeedMin.Equals(0.0f))
            {
                if (bulletState.bulletMoveSpeed <= bulletState.bulletDecelerationMoveSpeedMin)
                {
                    bulletState.bulletMoveSpeed = bulletState.bulletDecelerationMoveSpeedMin;
                }
            }
            else
            {
                if (bulletState.bulletMoveSpeed <= 0.0f)
                {
                    bulletState.bulletMoveSpeed = 0.0f;
                }
            }
        }
        else if (bulletState.bulletSpeedState.Equals(BulletSpeedState.BULLETSPEEDSTATE_LOOP))
        {
            if (bulletState.bulletMoveSpeedLoopBool.Equals(true))
            {
                bulletState.bulletMoveSpeed += bulletState.bulletAccelerationMoveSpeed;
                if (bulletState.bulletMoveSpeed >= bulletState.bulletAccelerationMoveSpeedMax)
                {
                    bulletState.bulletMoveSpeedLoopBool = false;
                }
            }
            else
            {
                bulletState.bulletMoveSpeed -= bulletState.bulletDecelerationMoveSpeed;
                if (bulletState.bulletMoveSpeed <= bulletState.bulletDecelerationMoveSpeedMin)
                {
                    bulletState.bulletMoveSpeedLoopBool = true;
                }
            }
        }
        else if (bulletState.bulletSpeedState.Equals(BulletSpeedState.BULLETSPEEDSTATE_LOOPONCE))
        {
            if (bulletState.bulletMoveSpeedLoopBool.Equals(true))
            {
                bulletState.bulletMoveSpeed += bulletState.bulletAccelerationMoveSpeed;
                if (bulletState.bulletMoveSpeed >= bulletState.bulletAccelerationMoveSpeedMax)
                {
                    bulletState.bulletMoveSpeedLoopBool = false;
                    bulletState.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_DECELERATING;
                }
            }
            else
            {
                bulletState.bulletMoveSpeed -= bulletState.bulletDecelerationMoveSpeed;
                if (bulletState.bulletMoveSpeed <= bulletState.bulletDecelerationMoveSpeedMin)
                {
                    bulletState.bulletMoveSpeedLoopBool = true;
                    bulletState.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
                }
            }
        }

        // 탄도 변경 (지속 증감)
        if (!bulletState.bulletRotateSpeed.Equals(0.0f))
        {
            if (bulletState.bulletRotateState.Equals(BulletRotateState.BULLETROTATESTATE_NORMAL))
            {
                transform.Rotate(Vector3.forward * bulletState.bulletRotateSpeed * Time.deltaTime);
            }
            else if (bulletState.bulletRotateState.Equals(BulletRotateState.BULLETROTATESTATE_LIMIT))
            {
                transform.Rotate(Vector3.forward * bulletState.bulletRotateSpeed * Time.deltaTime);
                if (bulletState.bulletRotateTime >= bulletState.bulletRotateLimit)
                {
                    bulletState.bulletRotateSpeed = 0.0f;
                }
            }
            else if (bulletState.bulletRotateState.Equals(BulletRotateState.BULLETROTATESTATE_ROTATEAROUND))
            {
                RotateAround(bulletState.bulletDestination);
            }
        }
        // 탄도 변경 (지속적으로 대상을 향해 바라보기)
        else if (bulletState.bulletRotateState.Equals(BulletRotateState.BULLETROTATESTATE_LOOKAT))
        {
            bulletState.bulletDestination = bulletState.GetBulletDestination(bulletState.targetObject.transform.position);
            float angle = Mathf.Atan2(bulletState.bulletDestination.y, bulletState.bulletDestination.x) * Mathf.Rad2Deg;
            if (bulletState.bulletType.Equals(BulletType.BULLETTYPE_LASER_MOVE))
            {
                ChangeRotateAngle(angle - 180.0f);
            }
            else
            {
                ChangeRotateAngle(angle - 90.0f);
            }
        }

        // 탄막 이동
        if (bulletState.bulletType.Equals(BulletType.BULLETTYPE_NORMAL))
        {
            transform.Translate(Vector2.up * bulletState.bulletMoveSpeed * Time.deltaTime);
            // transform.localPosition = Vector2.MoveTowards(transform.position, GetBulletDestination(targetPosition), bulletMoveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.right * bulletState.bulletMoveSpeed * Time.deltaTime);
            // transform.localPosition = Vector2.MoveTowards(transform.position, GetBulletDestination(targetPosition), bulletMoveSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (bulletState.isLookAt.Equals(true))
        {
            float angle = Mathf.Atan2(rigidbody2D.velocity.y, rigidbody2D.velocity.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0.0f, 0.0f, angle - 90.0f);
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
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // 탄도 변경 (공전)
    public void RotateAround(Vector2 target)
    {
        transform.RotateAround(target, Vector3.forward, bulletState.bulletRotateSpeed * Time.deltaTime);
    }
}
