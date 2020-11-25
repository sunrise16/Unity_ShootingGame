using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum BulletType
{
    BULLETTYPE_EMPTY,
    BULLETTYPE_NORMAL,
    BULLETTYPE_LASER_HOLD,
    BULLETTYPE_LASER_MOVE,
}

public enum BulletReflect
{
    BULLETREFLECT_NONE,
    BULLETREFLECT_NORMAL,
    BULLETREFLECT_CONTAINBOTTOM,
}

public class InitializeBullet : MonoBehaviour
{
    public GameObject bulletObject;
    public GameObject targetObject;
    public BulletType bulletType;
    public BulletReflect bulletReflect;
    public Vector2 bulletPosition;
    public Vector2 targetPosition;
    public Vector2 bulletDestination;

    public int bulletNumber;
    public float distance;
    public bool isGrazed;

    public Vector2 GetBulletDestination(Vector2 targetPosition)
    {
        bulletPosition = bulletObject.transform.position;
        distance = Vector2.Distance(targetPosition, bulletPosition);
        
        if (!distance.Equals(0))
        {
            bulletDestination.x = targetPosition.x - bulletPosition.x;
            bulletDestination.y = targetPosition.y - bulletPosition.y;
            return bulletDestination;
        }
        else
        {
            bulletDestination.x = 0;
            bulletDestination.y = 0;
            return bulletDestination;
        }
    }
    public Vector2 GetRandomAimedBulletDestination()
    {
        bulletPosition = bulletObject.transform.position;
        Vector2 targetPosition = new Vector2(Random.Range(-30.0f, 30.0f), Random.Range(-30.0f, 30.0f));
        distance = Vector2.Distance(targetPosition, bulletPosition);

        if (!distance.Equals(0))
        {
            bulletDestination.x = targetPosition.x - bulletPosition.x;
            bulletDestination.y = targetPosition.y - bulletPosition.y;
            return bulletDestination;
        }
        else
        {
            bulletDestination.x = 0;
            bulletDestination.y = 0;
            return bulletDestination;
        }
    }
    public Vector2 GetAimedBulletDestination()
    {
        bulletPosition = bulletObject.transform.position;
        targetPosition = targetObject.transform.position;
        distance = Vector2.Distance(targetPosition, bulletPosition);

        if (!distance.Equals(0))
        {
            bulletDestination.x = targetPosition.x - bulletPosition.x;
            bulletDestination.y = targetPosition.y - bulletPosition.y;
            return bulletDestination;
        }
        else
        {
            bulletDestination.x = 0;
            bulletDestination.y = 0;
            return bulletDestination;
        }
    }
    public Vector2 GetAimedBulletDestination(Vector2 targetPosition)
    {
        bulletPosition = bulletObject.transform.position;
        distance = Vector2.Distance(targetPosition, bulletPosition);

        if (!distance.Equals(0))
        {
            bulletDestination.x = targetPosition.x - bulletPosition.x;
            bulletDestination.y = targetPosition.y - bulletPosition.y;
            return bulletDestination;
        }
        else
        {
            bulletDestination.x = 0;
            bulletDestination.y = 0;
            return bulletDestination;
        }
    }
}
