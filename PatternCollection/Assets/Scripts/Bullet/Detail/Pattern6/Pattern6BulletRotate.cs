using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pattern6BulletRotate : MonoBehaviour
{
    private InitializeBullet initializeBullet;
    private MovingBullet movingBullet;

    void Start()
    {
        initializeBullet = GetComponent<InitializeBullet>();
        movingBullet = GetComponent<MovingBullet>();

        StartCoroutine(RotateBullet());
    }

    public IEnumerator RotateBullet()
    {
        while (true)
        {
            if (movingBullet.bulletMoveSpeed <= 0.0f)
            {
                movingBullet.bulletRotateTime = 0.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletMoveSpeed = 1.8f;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
                movingBullet.ChangeRotateAngle(movingBullet.GetAngle() + ((initializeBullet.bulletNumber % 2).Equals(0) ? 90.0f : -90.0f));
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
