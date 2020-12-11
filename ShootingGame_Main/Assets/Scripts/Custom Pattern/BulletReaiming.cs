using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// 특정 지점 또는 오브젝트를 향해 재조준하는 탄막 커스텀 스크립트
public class BulletReaiming : MonoBehaviour
{
    private BulletState bulletState;
    private BulletMove bulletMove;
    private Transform playerPosition;                               // 플레이어 오브젝트의 위치
    private Vector3 targetPosition;                                 // 탄막 재조준 시의 목표 위치

    private int changeCount;                                        // 스크립트 내부에서의 탄막 변화 횟수
    
    [HideInInspector] public BulletSpeedState bulletSpeedState;     // 재조준 시의 탄막 속도 속성 변화값
    [HideInInspector] public BulletRotateState bulletRotateState;   // 재조준 시의 탄막 회전 속성 변화값

    [HideInInspector] public int repeatCount;                       // 재조준 반복 횟수

    [HideInInspector] public float waitTime;                        // 탄막 변화 대기 시간
    [HideInInspector] public float bulletMoveSpeed;                 // 재조준 시의 탄막 속도
    [HideInInspector] public float bulletRotateSpeed;               // 재조준 시의 회전 속도
    [HideInInspector] public float bulletRotateLimit;               // 재조준 시의 회전 제한값

    [HideInInspector] public bool isPlayerAimed;                    // 플레이어 위치 조준인지 체크
    [HideInInspector] public bool isRandomAimed;                    // 랜덤 위치 조준인지 체크
    [HideInInspector] public bool isSpeedDown;                      // 일정 속도까지 느려진 뒤 재조준 체크
    [HideInInspector] public bool isTimer;                          // 일정 시간이 지난 뒤 재조준 체크

    private void Start()
    {
        bulletState = GetComponent<BulletState>();
        bulletMove = GetComponent<BulletMove>();
        playerPosition = GameObject.Find("CHARACTER").transform.Find("Player");

        changeCount = 0;

        StartCoroutine(Reaiming());
	}

    private IEnumerator Reaiming()
    {
        if (isSpeedDown.Equals(true))
        {
            while (true)
            {
                yield return null;

                if (bulletState.bulletMoveSpeed <= 0.0f)
                {
                    if (isPlayerAimed.Equals(true))
                    {
                        targetPosition = playerPosition.position;
                    }
                    else if (isRandomAimed.Equals(true))
                    {
                        targetPosition = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), 0.0f);
                    }
                    bulletState.bulletSpeedState = bulletSpeedState;
                    bulletState.bulletRotateState = bulletRotateState;
                    bulletState.bulletMoveSpeed = bulletMoveSpeed;
                    bulletState.bulletRotateSpeed = bulletRotateSpeed;
                    bulletState.bulletRotateLimit = bulletRotateLimit;
                    bulletState.bulletDestination = bulletState.GetBulletDestination(targetPosition);
                    float angle = Mathf.Atan2(bulletState.bulletDestination.y, bulletState.bulletDestination.x) * Mathf.Rad2Deg;
                    bulletMove.ChangeRotateAngle(angle - 90.0f);
                    changeCount++;
                    break;
                }
            }

            if (changeCount < repeatCount)
            {
                StartCoroutine(Reaiming());
            }
        }
        else if (isTimer.Equals(true))
        {
            yield return new WaitForSeconds(waitTime);

            if (isPlayerAimed.Equals(true))
            {
                targetPosition = playerPosition.position;
            }
            else if (isRandomAimed.Equals(true))
            {
                targetPosition = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), 0.0f);
            }
            bulletState.bulletSpeedState = bulletSpeedState;
            bulletState.bulletRotateState = bulletRotateState;
            bulletState.bulletMoveSpeed = bulletMoveSpeed;
            bulletState.bulletRotateSpeed = bulletRotateSpeed;
            bulletState.bulletRotateLimit = bulletRotateLimit;
            bulletState.bulletDestination = bulletState.GetBulletDestination(targetPosition);
            float angle = Mathf.Atan2(bulletState.bulletDestination.y, bulletState.bulletDestination.x) * Mathf.Rad2Deg;
            bulletMove.ChangeRotateAngle(angle - 90.0f);
            changeCount++;

            if (changeCount < repeatCount)
            {
                StartCoroutine(Reaiming());
            }
        }
    }
}
