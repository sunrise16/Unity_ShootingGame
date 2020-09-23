using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pattern8BulletAiming : MonoBehaviour
{
    private InitializeBullet initializeBullet;
    private MovingBullet movingBullet;
    private GameObject playerObject;

    void Start()
    {
        initializeBullet = GetComponent<InitializeBullet>();
        movingBullet = GetComponent<MovingBullet>();
        playerObject = GameObject.Find("PLAYER");

        StartCoroutine(SetBullet());
    }

    public IEnumerator SetBullet()
    {
        while (true)
        {
            if (movingBullet.bulletMoveSpeed <= 0.0f)
            {
                Vector2 targetPosition = playerObject.transform.position;

                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletMoveSpeed = 5.0f;
                movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(targetPosition);
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f);
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
