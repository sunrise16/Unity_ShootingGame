using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pattern14BulletRotate : MonoBehaviour
{
    private EnemyFire enemyFire;
    private SpriteRenderer spriteRenderer;
    private InitializeBullet initializeBullet;
    private MovingBullet movingBullet;

    private int rotateCount;

    void Start()
    {
        enemyFire = GameObject.Find("ENEMY").GetComponent<EnemyFire>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                movingBullet.bulletDecelerationMoveSpeed = 0.06f;
                if (initializeBullet.bulletNumber <= 2)
                {
                    movingBullet.ChangeRotateAngle(movingBullet.GetAngle() + 25.0f + 180.0f);
                }
                else
                {
                    movingBullet.ChangeRotateAngle(movingBullet.GetAngle() - 25.0f + 180.0f);
                }

                if (rotateCount >= 1)
                {
                    spriteRenderer.sprite = enemyFire.spriteCollection[79];
                    movingBullet.bulletMoveSpeed = 2.0f;
                    movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                    break;
                }
                else
                {
                    spriteRenderer.sprite = enemyFire.spriteCollection[78];
                    rotateCount++;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
