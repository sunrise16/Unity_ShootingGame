using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BulletMove : BulletState
{
    private Rigidbody2D rigidbody2D;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
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
            bulletDestination = GetBulletDestination(targetObject.transform.position);
            float angle = Mathf.Atan2(bulletDestination.y, bulletDestination.x) * Mathf.Rad2Deg;
            if (bulletType.Equals(BulletType.BULLETTYPE_LASER_MOVE))
            {
                ChangeRotateAngle(angle - 180.0f);
            }
            else
            {
                ChangeRotateAngle(angle - 90.0f);
            }
        }

        // 탄막 이동
        if (bulletType.Equals(BulletType.BULLETTYPE_NORMAL))
        {
            transform.Translate(Vector2.up * bulletMoveSpeed * Time.deltaTime);
            // transform.localPosition = Vector2.MoveTowards(transform.position, GetBulletDestination(targetPosition), bulletMoveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.right * bulletMoveSpeed * Time.deltaTime);
            // transform.localPosition = Vector2.MoveTowards(transform.position, GetBulletDestination(targetPosition), bulletMoveSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (isLookAt.Equals(true))
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
        transform.RotateAround(target, Vector3.forward, bulletRotateSpeed * Time.deltaTime);
    }
}
