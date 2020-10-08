using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pattern21BulletRotate : MonoBehaviour
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
            if (movingBullet.bulletMoveSpeed <= 2.0f)
            {
                movingBullet.bulletMoveSpeed = 2.0f;
                movingBullet.bulletAccelerationMoveSpeed = Random.Range(0.01f, 0.02f);
                movingBullet.bulletAccelerationMoveSpeedMax = Random.Range(4.0f, 6.0f);
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
                movingBullet.bulletRotateSpeed = Random.Range(30.0f, 75.0f) * ((initializeBullet.bulletNumber % 2).Equals(0) ? 1 : -1);
                movingBullet.bulletRotateLimit = Random.Range(1.0f, 2.0f);
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
