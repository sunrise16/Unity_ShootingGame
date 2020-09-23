using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Stage13BulletRotate : MonoBehaviour
{
    private MovingBullet movingBullet;

    void Start()
    {
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
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_ACCELERATING;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_LIMIT;
                break;
            }

            yield return new WaitForEndOfFrame();
        }
	}
}
