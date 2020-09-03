using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ReflectBullet : MonoBehaviour
{
    public int reflectCount;
    public int reflectLimit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        InitializeBullet initializeBullet = GetComponent<InitializeBullet>();
        MovingBullet movingBullet = GetComponent<MovingBullet>();

        if (!initializeBullet.bulletReflect.Equals(BulletReflect.BULLETREFLECT_NONE) && collision.tag.Equals("REFLECTZONE"))
        {
            if (reflectCount < reflectLimit)
            {
                reflectCount++;
                if (collision.gameObject.layer.Equals("REFLECTZONE_LEFTRIGHT"))
                {
                    movingBullet.ChangeRotateAngle(movingBullet.GetAngle() * -1);
                }
                else if (collision.gameObject.layer.Equals("REFLECTZONE_TOP"))
                {
                    movingBullet.ChangeRotateAngle(movingBullet.GetAngle() * -1);
                }
                else if (collision.gameObject.layer.Equals("REFLECTZONE_BOTTOM") && initializeBullet.bulletReflect.Equals(BulletReflect.BULLETREFLECT_CONTAINBOTTOM))
                {
                    if (movingBullet.GetAngle() >= 0.0f && movingBullet.GetAngle() <= 180.0f)
                    {
                        movingBullet.ChangeRotateAngle(180.0f - movingBullet.GetAngle());
                    }
                }
            }
        }
    }
}
