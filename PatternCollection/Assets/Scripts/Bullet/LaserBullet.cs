using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    // 공통 변수
    public float laserWidth;

    // 고정 레이저 타입일 경우의 변수
    public float laserEnableTime;
    public float laserEnableSpeed;
    private float laserEnableDelay;
    public float laserDisableTime;
    public float laserDisableSpeed;
    private float laserDisableDelay;
    public bool isLaserEnabled;
    public bool isLaserDisabled;

    // 무빙 레이저 타입일 경우의 변수
    public float laserLength;

    void Start()
    {
        // 최초 변수 초기화 및 박스 콜라이더 해제
        laserEnableDelay = 0.0f;
        laserDisableDelay = 0.0f;
        isLaserEnabled = false;
        isLaserDisabled = true;

        if (GetComponent<InitializeBullet>().bulletType == BulletType.BULLETTYPE_LASER_HOLD)
        {
            transform.localScale = new Vector3(0.15f, transform.localScale.y, transform.localScale.z);
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            transform.localScale = new Vector3(0.05f, transform.localScale.y, transform.localScale.z);
        }
    }
	
	void Update()
    {
        // 레이저 활성화, 비활성화
        if (GetComponent<InitializeBullet>().bulletType == BulletType.BULLETTYPE_LASER_HOLD)
        {
            if (isLaserDisabled == true)
            {
                laserEnableDelay += Time.deltaTime;
                if (laserEnableDelay >= laserEnableTime)
                {
                    laserEnableDelay = laserEnableTime;
                    transform.localScale = new Vector3(transform.localScale.x + laserEnableSpeed, transform.localScale.y, transform.localScale.z);
                    if (transform.localScale.x >= laserWidth)
                    {
                        transform.localScale = new Vector3(laserWidth, transform.localScale.y, transform.localScale.z);
                        isLaserDisabled = false;
                        isLaserEnabled = true;
                        GetComponent<BoxCollider2D>().enabled = true;
                    }
                }
            }
            else if (isLaserEnabled == true)
            {
                laserDisableDelay += Time.deltaTime;
                if (laserDisableDelay >= laserDisableTime)
                {
                    laserDisableDelay = laserDisableTime;
                    transform.localScale = new Vector3(transform.localScale.x - laserDisableSpeed, transform.localScale.y, transform.localScale.z);
                    if (transform.localScale.x <= 0.0f)
                    {
                        transform.localScale = new Vector3(0.0f, transform.localScale.y, transform.localScale.z);
                        isLaserEnabled = false;
                        GetComponent<BoxCollider2D>().enabled = false;
                        GetComponent<EraseBullet>().ClearBullet();
                    }
                }
            }
        }
        else
        {
            float scaleValue = (GetComponent<MovingBullet>().bulletMoveSpeed * Time.deltaTime) * 0.8f;
            transform.localScale = new Vector3(transform.localScale.x + scaleValue, transform.localScale.y, transform.localScale.z);
            if (transform.localScale.x >= laserLength)
            {
                transform.localScale = new Vector3(laserLength, transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
