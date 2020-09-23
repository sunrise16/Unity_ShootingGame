using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pattern14BulletRotate : MonoBehaviour
{
    private InitializeBullet initializeBullet;
    private MovingBullet movingBullet;

    private int rotateCount;

    void Start()
    {
        initializeBullet = GetComponent<InitializeBullet>();
        movingBullet = GetComponent<MovingBullet>();

        rotateCount = 0;

        StartCoroutine(RotateBullet());
    }

    public IEnumerator RotateBullet()
    {
        while (true)
        {
            if (movingBullet.bulletMoveSpeed <= 0.0f)
            {
                movingBullet.bulletMoveSpeed = 3.0f;
                movingBullet.bulletDecelerationMoveSpeed = 0.04f;
                
                if (rotateCount >= 1)
                {
                    movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    if ((initializeBullet.bulletNumber % 2).Equals(0))
                    {
                        movingBullet.ChangeRotateAngle(movingBullet.GetAngle() + 60.0f);
                    }
                    else
                    {
                        movingBullet.ChangeRotateAngle(movingBullet.GetAngle() - 60.0f);
                    }
                    break;
                }
                else
                {
                    movingBullet.ChangeRotateAngle(movingBullet.GetAngle() + 150.0f);
                    rotateCount++;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
