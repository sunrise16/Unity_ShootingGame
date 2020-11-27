using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ReflectBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BULLET_ENEMY"))
        {
            BulletState bulletState = collision.gameObject.GetComponent<BulletState>();
            BulletMove bulletMove = collision.gameObject.GetComponent<BulletMove>();

            if (!bulletState.bulletReflectState.Equals(BulletReflectState.BULLETREFLECTSTATE_NONE) && gameObject.CompareTag("REFLECTZONE"))
            {
                if (bulletState.reflectCount < bulletState.reflectLimit)
                {
                    bulletState.reflectCount++;
                    if (gameObject.layer.Equals(LayerMask.NameToLayer("REFLECTZONE_LEFTRIGHT")))
                    {
                        bulletMove.ChangeRotateAngle(bulletMove.GetAngle() * -1);
                    }
                    else if ((gameObject.layer.Equals(LayerMask.NameToLayer("REFLECTZONE_TOP"))) ||
                        (gameObject.layer.Equals(LayerMask.NameToLayer("REFLECTZONE_BOTTOM")) && bulletState.bulletReflectState.Equals(BulletReflectState.BULLETREFLECTSTATE_CONTAINBOTTOM)))
                    {
                        if (bulletMove.GetAngle() >= 0.0f && bulletMove.GetAngle() <= 180.0f)
                        {
                            bulletMove.ChangeRotateAngle(180.0f - bulletMove.GetAngle());
                        }
                        else
                        {
                            bulletMove.ChangeRotateAngle(-180.0f - bulletMove.GetAngle());
                        }
                    }

                    if (!(gameObject.layer.Equals(LayerMask.NameToLayer("REFLECTZONE_BOTTOM")) && !bulletState.bulletReflectState.Equals(BulletReflectState.BULLETREFLECTSTATE_CONTAINBOTTOM)))
                    {
                        if (bulletState.isSpriteChange.Equals(true))
                        {
                            // spriteRenderer.sprite = enemyFire.spriteCollection[changeSpriteNumber];
                        }
                        if (bulletState.isEffectOutput.Equals(true))
                        {
                            // StartCoroutine(enemyFire.CreateBulletFireEffect(effectSpriteNumber, scaleDownSpeed, scaleDownTime, alphaUpSpeed, transform.position));
                        }
                    }
                }
            }
        }
    }
}
