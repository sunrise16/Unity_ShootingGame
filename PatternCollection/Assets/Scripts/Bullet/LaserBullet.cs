using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 middlePoint1;
    public Vector3 middlePoint2;
    public Vector3 endPoint;

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
        GetComponent<CapsuleCollider2D>().enabled = false;
        laserEnableDelay = 0.0f;
        laserDisableDelay = 0.0f;
        isLaserEnabled = false;
        isLaserDisabled = true;

        // 라인 렌더러 설정
        for (int i = 0; i < 4; i++)
        {
            GetComponent<LineRenderer>().SetPosition(i, new Vector3(transform.position.x, transform.position.y, transform.position.z));
        }
        GetComponent<LineRenderer>().startWidth = 0.5f;
        GetComponent<LineRenderer>().endWidth = 0.5f;
    }
	
	void Update()
    {
        // 라인 렌더러 포지션 설정
        if (GetComponent<InitializeBullet>().bulletType == BulletType.BULLETTYPE_LASER_HOLD)
        {
            startPoint = transform.Find("LaserStartPoint").transform.position;
            middlePoint1 = transform.Find("LaserMiddlePoint1").transform.position;
            middlePoint2 = transform.Find("LaserMiddlePoint2").transform.position;
            endPoint = transform.Find("LaserEndPoint").transform.position;
            GetComponent<LineRenderer>().SetPosition(0, startPoint);
            GetComponent<LineRenderer>().SetPosition(1, middlePoint1);
            GetComponent<LineRenderer>().SetPosition(2, middlePoint2);
            GetComponent<LineRenderer>().SetPosition(3, endPoint);
        }
        else if (GetComponent<InitializeBullet>().bulletType == BulletType.BULLETTYPE_LASER_MOVE)
        {
            transform.Find("LaserStartPoint").transform.position = transform.position;
            startPoint = transform.Find("LaserStartPoint").transform.position;
            GetComponent<LineRenderer>().SetPosition(0, startPoint);

            float distance = Vector2.Distance(startPoint, GetComponent<LineRenderer>().GetPosition(3));
            if (distance <= laserLength)
            {
                transform.Find("LaserEndPoint").transform.position = GetComponent<LineRenderer>().GetPosition(3);
                endPoint = transform.Find("LaserEndPoint").transform.position;
            }
            else
            {
                endPoint = transform.Find("LaserEndPoint").transform.position;
            }
            GetComponent<LineRenderer>().SetPosition(3, endPoint);
            transform.Find("LaserMiddlePoint1").transform.position = new Vector3((startPoint.x - endPoint.x) * (2 / 3), (startPoint.y - endPoint.y) * (2 / 3), 0.0f);
            middlePoint1 = transform.Find("LaserMiddlePoint1").transform.position;
            GetComponent<LineRenderer>().SetPosition(1, middlePoint1);
            transform.Find("LaserMiddlePoint2").transform.position = new Vector3((startPoint.x - endPoint.x) * (1 / 3), (startPoint.y - endPoint.y) * (1 / 3), 0.0f);
            middlePoint2 = transform.Find("LaserMiddlePoint2").transform.position;
            GetComponent<LineRenderer>().SetPosition(2, middlePoint2);

            GetComponent<CapsuleCollider2D>().size = new Vector2(GetComponent<CapsuleCollider2D>().size.x, distance);
        }

        // 레이저 활성화, 비활성화
        if (GetComponent<InitializeBullet>().bulletType == BulletType.BULLETTYPE_LASER_HOLD)
        {
            if (isLaserDisabled == true)
            {
                laserEnableDelay += Time.deltaTime;
                if (laserEnableDelay >= laserEnableTime)
                {
                    laserEnableDelay = laserEnableTime;
                    GetComponent<LineRenderer>().startWidth += laserEnableSpeed;
                    GetComponent<LineRenderer>().endWidth = GetComponent<LineRenderer>().startWidth;
                    if (GetComponent<LineRenderer>().startWidth >= 2.5f)
                    {
                        GetComponent<LineRenderer>().startWidth = 2.5f;
                        isLaserDisabled = false;
                        isLaserEnabled = true;
                        GetComponent<CapsuleCollider2D>().enabled = true;
                    }
                }
            }
            else if (isLaserEnabled == true)
            {
                laserDisableDelay += Time.deltaTime;
                if (laserDisableDelay >= laserDisableTime)
                {
                    laserDisableDelay = laserDisableTime;
                    GetComponent<LineRenderer>().startWidth -= laserDisableSpeed;
                    GetComponent<LineRenderer>().endWidth = GetComponent<LineRenderer>().startWidth;
                    if (GetComponent<LineRenderer>().startWidth <= 0.0f)
                    {
                        GetComponent<LineRenderer>().startWidth = 0.0f;
                        isLaserEnabled = false;
                        GetComponent<CapsuleCollider2D>().enabled = false;
                        GetComponent<EraseBullet>().ClearBullet();
                    }
                }
            }
        }
    }
}
