﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum BulletSpeedState
{
    BULLETSPEEDSTATE_NORMAL,
    BULLETSPEEDSTATE_ACCELERATING,
    BULLETSPEEDSTATE_DECELERATING,
    BULLETSPEEDSTATE_LOOP,
}

public enum BulletRotateState
{
    BULLETROTATESTATE_NONE,
    BULLETROTATESTATE_NORMAL,
    BULLETROTATESTATE_LIMIT,
    BULLETROTATESTATE_ROTATEAROUND,
}

public class MovingBullet : MonoBehaviour
{
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

    void Start()
    {
        bulletRotateTime = 0.0f;
    }

    void Update()
    {
        bulletRotateTime += Time.deltaTime;

        // 탄속 변경
        if (bulletSpeedState == BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING)
        {
            bulletMoveSpeed += bulletAccelerationMoveSpeed;
            if (bulletAccelerationMoveSpeedMax != 0.0f)
            {
                if (bulletMoveSpeed >= bulletAccelerationMoveSpeedMax)
                {
                    bulletMoveSpeed = bulletAccelerationMoveSpeedMax;
                }
            }
        }
        else if (bulletSpeedState == BulletSpeedState.BULLETSPEEDSTATE_DECELERATING)
        {
            bulletMoveSpeed -= bulletDecelerationMoveSpeed;
            if (bulletDecelerationMoveSpeedMin != 0.0f)
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
        else if (bulletSpeedState == BulletSpeedState.BULLETSPEEDSTATE_LOOP)
        {
            if (bulletMoveSpeedLoopBool == true)
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

        // 탄도 변경 (지속 증감)
        if (bulletRotateSpeed != 0.0f)
        {
            if (bulletRotateState == BulletRotateState.BULLETROTATESTATE_NORMAL)
            {
                transform.Rotate(Vector3.forward * bulletRotateSpeed * Time.deltaTime);
            }
            else if (bulletRotateState == BulletRotateState.BULLETROTATESTATE_LIMIT)
            {
                transform.Rotate(Vector3.forward * bulletRotateSpeed * Time.deltaTime);
                if (bulletRotateTime >= bulletRotateLimit)
                {
                    bulletRotateSpeed = 0.0f;
                }
            }
            else if (bulletRotateState == BulletRotateState.BULLETROTATESTATE_ROTATEAROUND)
            {
                RotateAround(bulletDestination);
            }
        }

        // 탄막 이동
        transform.Translate(Vector2.up * bulletMoveSpeed * Time.deltaTime);
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
}
