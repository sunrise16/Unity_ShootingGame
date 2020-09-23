using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pattern9BulletRotate : MonoBehaviour
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
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletMoveSpeed = 2.0f;
                if (initializeBullet.bulletNumber < 8)
                {
                    movingBullet.ChangeRotateAngle(-110.0f - (20.0f * initializeBullet.bulletNumber) + Random.Range(-2.0f, 2.0f));
                }
                else
                {
                    movingBullet.ChangeRotateAngle(110.0f + (20.0f * (initializeBullet.bulletNumber - 8) + Random.Range(-2.0f, 2.0f)));
                }
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
