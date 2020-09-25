using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pattern19BulletRotate : MonoBehaviour
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
                movingBullet.bulletMoveSpeed = initializeBullet.bulletNumber.Equals(0) ? 5.0f : 6.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.ChangeRotateAngle(movingBullet.GetAngle() + 180.0f + (initializeBullet.bulletNumber.Equals(0) ? -35.0f : 35.0f));
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
