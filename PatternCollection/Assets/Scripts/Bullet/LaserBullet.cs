using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    private InitializeBullet initializeBullet;
    private MovingBullet movingBullet;
    private EraseBullet eraseBullet;
    private CapsuleCollider2D capsuleCollider2D;

    public float laserWidth;

    // 레이저 활성화, 비활성화 관련 변수
    public float laserEnableTime;
    public float laserEnableSpeed;
    private float laserEnableDelay;
    public float laserDisableTime;
    public float laserDisableSpeed;
    private float laserDisableDelay;
    public bool isLaserEnabled;
    public bool isLaserDisabled;

    // 레이저 회전 관련 변수
    public BulletRotateState laserRotateState;
    public float laserRotateSpeed;
    public float laserRotateLimit;
    public bool isLaserRotateEnabled;
    public bool isLaserRotateDisabled;

    void Start()
    {
        initializeBullet = GetComponent<InitializeBullet>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        movingBullet = GetComponent<MovingBullet>();
        eraseBullet = GetComponent<EraseBullet>();

        // 최초 변수 초기화 및 박스 콜라이더 해제
        laserEnableDelay = 0.0f;
        laserDisableDelay = 0.0f;
        isLaserEnabled = false;
        isLaserDisabled = true;

        if (initializeBullet.bulletType.Equals(BulletType.BULLETTYPE_LASER_HOLD))
        {
            transform.localScale = new Vector3(0.25f, transform.localScale.y, transform.localScale.z);
            capsuleCollider2D.enabled = false;
        }
        else
        {
            transform.localScale = new Vector3(0.15f, transform.localScale.y, transform.localScale.z);
        }
    }
	
	void Update()
    {
        // 레이저 활성화, 비활성화
        if (initializeBullet.bulletType.Equals(BulletType.BULLETTYPE_LASER_HOLD))
        {
            if (isLaserDisabled.Equals(true))
            {
                laserEnableDelay += Time.deltaTime;
                if (laserEnableDelay >= laserEnableTime)
                {
                    laserEnableDelay = laserEnableTime;
                    transform.localScale = new Vector3(transform.localScale.x + laserEnableSpeed, transform.localScale.y, transform.localScale.z);
                    if (transform.localScale.x >= laserWidth)
                    {
                        if (isLaserRotateEnabled.Equals(true))
                        {
                            movingBullet.bulletRotateState = laserRotateState;
                            movingBullet.bulletRotateSpeed = laserRotateSpeed;
                            movingBullet.bulletRotateLimit = laserRotateLimit;
                            isLaserRotateEnabled = false;
                        }
                        transform.localScale = new Vector3(laserWidth, transform.localScale.y, transform.localScale.z);
                        isLaserDisabled = false;
                        isLaserEnabled = true;
                        capsuleCollider2D.enabled = true;
                    }
                }
            }
            else if (isLaserEnabled.Equals(true))
            {
                laserDisableDelay += Time.deltaTime;
                if (laserDisableDelay >= laserDisableTime)
                {
                    laserDisableDelay = laserDisableTime;
                    transform.localScale = new Vector3(transform.localScale.x - laserDisableSpeed, transform.localScale.y, transform.localScale.z);
                    if (transform.localScale.x <= 0.0f)
                    {
                        if (isLaserRotateDisabled.Equals(true))
                        {
                            movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                            movingBullet.bulletRotateSpeed = 0.0f;
                            movingBullet.bulletRotateLimit = 0.0f;
                            isLaserRotateDisabled = false;
                        }
                        transform.localScale = new Vector3(0.0f, transform.localScale.y, transform.localScale.z);
                        isLaserEnabled = false;
                        capsuleCollider2D.enabled = false;
                        eraseBullet.ClearBullet();
                    }
                }
            }
        }
        // 무빙 레이저의 스케일 조정
        // else
        // {
        //     float scaleValue = (GetComponent<MovingBullet>().bulletMoveSpeed * Time.deltaTime) * 0.8f;
        //     transform.localScale = new Vector3(transform.localScale.x + scaleValue, transform.localScale.y, transform.localScale.z);
        //     if (transform.localScale.x >= laserLength)
        //     {
        //         transform.localScale = new Vector3(laserLength, transform.localScale.y, transform.localScale.z);
        //     }
        // }
    }
}
