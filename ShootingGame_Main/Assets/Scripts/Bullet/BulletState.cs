using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BulletState : MonoBehaviour
{
    [HideInInspector] public GameObject bulletObject;                     // 탄막 오브젝트
    [HideInInspector] public GameObject targetObject;                     // 타겟 오브젝트

    [HideInInspector] public BulletType bulletType;                       // 탄막의 종류
    [HideInInspector] public BulletReflectState bulletReflectState;       // 탄막 반사 여부
    [HideInInspector] public BulletSpeedState bulletSpeedState;           // 탄막 속도 조절
    [HideInInspector] public BulletRotateState bulletRotateState;         // 탄막 회전 조절

    [HideInInspector] public Vector2 bulletPosition;                      // 탄막 오브젝트 위치
    [HideInInspector] public Vector2 targetPosition;                      // 타겟 오브젝트 위치
    [HideInInspector] public Vector2 bulletDestination;                   // 탄막 목표 지점

    [HideInInspector] public int bulletNumber;                            // 탄막 구분용 넘버
    [HideInInspector] public float distance;                              // 탄막과 타겟 간의 거리
    [HideInInspector] public bool isGrazed;                               // 탄막 그레이즈 상태

    // 탄막 속도
    [HideInInspector] public float bulletMoveSpeed;                       // 탄막 이동 속도
    [HideInInspector] public float bulletAccelerationMoveSpeed;           // 탄막 가속 속도
    [HideInInspector] public float bulletAccelerationMoveSpeedMax;        // 탄막 가속 속도 제한
    [HideInInspector] public float bulletDecelerationMoveSpeed;           // 탄막 감속 속도
    [HideInInspector] public float bulletDecelerationMoveSpeedMin;        // 탄막 감속 속도 제한
    [HideInInspector] public bool bulletMoveSpeedLoopBool;                // 탄막 속도 주기적 변화 여부
    [HideInInspector] public bool bulletMoveSpeedLoopOnceBool;            // 탄막 속도 1회 변화 여부

    // 탄막 회전
    [HideInInspector] public float bulletRotateTime;                      // 탄막 회전 진행 시간
    [HideInInspector] public float bulletRotateLimit;                     // 탄막 회전 제한 시간
    [HideInInspector] public float bulletRotateSpeed;                     // 탄막 회전 속도
    [HideInInspector] public bool isLookAt;                               // 대상 지정 공전 여부

    // 탄막 반사
    [HideInInspector] public int reflectCount;                            // 탄막 반사 횟수
    [HideInInspector] public int reflectLimit;                            // 탄막 반사 횟수 제한
    [HideInInspector] public bool isSpriteChange;                         // 반사 시 스프라이트 변경 여부
    [HideInInspector] public bool isEffectOutput;                         // 반사 시 이펙트 출력 여부

    public void InitBulletState()
    {
        bulletType = BulletType.BULLETTYPE_NONE;
        bulletReflectState = BulletReflectState.BULLETREFLECTSTATE_NONE;
        bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NONE;
        bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;

        bulletPosition = new Vector2(0.0f, 0.0f);
        targetPosition = new Vector2(0.0f, 0.0f);
        bulletDestination = new Vector2(0.0f, 0.0f);

        bulletNumber = 0;
        distance = 0.0f;
        isGrazed = false;

        bulletMoveSpeed = 0.0f;
        bulletAccelerationMoveSpeed = 0.0f;
        bulletAccelerationMoveSpeedMax = 0.0f;
        bulletDecelerationMoveSpeed = 0.0f;
        bulletDecelerationMoveSpeedMin = 0.0f;
        bulletMoveSpeedLoopBool = false;
        bulletMoveSpeedLoopOnceBool = false;

        bulletRotateTime = 0.0f;
        bulletRotateLimit = 0.0f;
        bulletRotateSpeed = 0.0f;
        isLookAt = false;

        reflectCount = 0;
        reflectLimit = 0;
        isSpriteChange = false;
        isEffectOutput = false;
    }

    public Vector2 GetBulletDestination(Vector2 targetPosition)
    {
        bulletPosition = bulletObject.transform.position;
        distance = Vector2.Distance(targetPosition, bulletPosition);

        if (!distance.Equals(0))
        {
            bulletDestination.x = targetPosition.x - bulletPosition.x;
            bulletDestination.y = targetPosition.y - bulletPosition.y;
            return bulletDestination;
        }
        else
        {
            bulletDestination.x = 0;
            bulletDestination.y = 0;
            return bulletDestination;
        }
    }

    public Vector2 GetRandomAimedBulletDestination()
    {
        bulletPosition = bulletObject.transform.position;
        Vector2 targetPosition = new Vector2(Random.Range(-30.0f, 30.0f), Random.Range(-30.0f, 30.0f));
        distance = Vector2.Distance(targetPosition, bulletPosition);

        if (!distance.Equals(0))
        {
            bulletDestination.x = targetPosition.x - bulletPosition.x;
            bulletDestination.y = targetPosition.y - bulletPosition.y;
            return bulletDestination;
        }
        else
        {
            bulletDestination.x = 0;
            bulletDestination.y = 0;
            return bulletDestination;
        }
    }

    public Vector2 GetAimedBulletDestination()
    {
        bulletPosition = bulletObject.transform.position;
        targetPosition = targetObject.transform.position;
        distance = Vector2.Distance(targetPosition, bulletPosition);

        if (!distance.Equals(0))
        {
            bulletDestination.x = targetPosition.x - bulletPosition.x;
            bulletDestination.y = targetPosition.y - bulletPosition.y;
            return bulletDestination;
        }
        else
        {
            bulletDestination.x = 0;
            bulletDestination.y = 0;
            return bulletDestination;
        }
    }
}
