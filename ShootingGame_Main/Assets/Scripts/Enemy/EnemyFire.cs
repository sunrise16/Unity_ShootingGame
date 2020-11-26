using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    // 원형 판정 탄막 발사
    // public void CircleBulletFire
    //     (GameObject obj, int bulletNumber, string bulletTag, int bulletLayer, Vector2 bulletFirePosition, Vector3 bulletScale,
    //     Transform bulletParent, float circleColliderRadius, float spriteAlpha, int spriteNumber,
    //     // float capsuleColliderSizeX, float capsuleColliderSizeY, float capsuleColliderOffsetX, float capsuleColliderOffsetY,
    //     // float boxColliderSizeX, float boxColliderSizeY, float boxColliderOffsetX, float boxColliderOffsetY,
    //     BulletType bulletType, GameObject targetObject,
    //     BulletSpeedState bulletSpeedState, float bulletMoveSpeed,
    //     float bulletAccelerationMoveSpeed, float bulletAccelerationMoveSpeedMax,
    //     float bulletDecelerationMoveSpeed, float bulletDecelerationMoveSpeedMin, bool bulletMoveSpeedLoopBool,
    //     BulletRotateState bulletRotateState, float bulletRotateSpeed, float bulletRotateLimit,
    //     int bulletDestinationType, Vector2 targetPosition, float addRotateAngle, int addCustomScript = 0,
    //     bool isSpriteRotate = false, float spriteRotateSpeed = 0.0f, bool isGravity = false, float velocity = 0.0f, float gravityScale = 0.0f, bool isLookAt = false,
    //     bool isLaserType = false, float laserWidth = 1.8f, bool isLaserEnabled = false, bool isLaserDisabled = false,
    //     float laserEnableTime = 0.0f, float laserEnableSpeed = 0.0f, float laserDisableTime = 0.0f, float laserDisableSpeed = 0.0f,
    //     BulletRotateState laserRotateState = BulletRotateState.BULLETROTATESTATE_NONE, float laserRotateSpeed = 0.0f,
    //     bool isLaserRotateEnabled = false, bool isLaserRotateDisabled = false)
    // {
    //     if (bulletManager.bulletPool.Count > 0)
    //     {
    //         GameObject bullet = bulletManager.bulletPool.Dequeue();
    //         bullet.SetActive(true);
    //         ClearChild(bullet);
    //         bullet.transform.position = bulletFirePosition;
    //         bullet.transform.localScale = bulletScale;
    //         bullet.gameObject.tag = bulletTag;
    //         bullet.gameObject.layer = bulletLayer;
    //         bullet.transform.SetParent(bulletParent);
    //         if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
    //         if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
    //         if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
    //         if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
    //         SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
    //         InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
    //         MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
    //         switch (colliderType)
    //         {
    //             default:
    //             case 1:
    //                 if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
    //                 CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
    //                 circleCollider2D.isTrigger = true;
    //                 circleCollider2D.radius = circleColliderRadius;
    //                 circleCollider2D.enabled = false;
    //                 break;
    //             case 2:
    //                 if (!bullet.GetComponent<CapsuleCollider2D>()) bullet.AddComponent<CapsuleCollider2D>();
    //                 CapsuleCollider2D capsuleCollider2D = bullet.GetComponent<CapsuleCollider2D>();
    //                 capsuleCollider2D.isTrigger = true;
    //                 capsuleCollider2D.size = new Vector2(capsuleColliderSizeX, capsuleColliderSizeY);
    //                 capsuleCollider2D.offset = new Vector2(capsuleColliderOffsetX, capsuleColliderOffsetY);
    //                 capsuleCollider2D.enabled = false;
    //                 break;
    //             case 3:
    //                 if (!bullet.GetComponent<BoxCollider2D>()) bullet.AddComponent<BoxCollider2D>();
    //                 BoxCollider2D boxCollider2D = bullet.GetComponent<BoxCollider2D>();
    //                 boxCollider2D.isTrigger = true;
    //                 boxCollider2D.size = new Vector2(boxColliderSizeX, boxColliderSizeY);
    //                 boxCollider2D.offset = new Vector2(boxColliderOffsetX, boxColliderOffsetY);
    //                 boxCollider2D.enabled = false;
    //                 break;
    //         }
    //         spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, spriteAlpha);
    //         spriteRenderer.sprite = spriteCollection[spriteNumber];
    //         spriteRenderer.sortingOrder = 3;
    //         initializeBullet.bulletNumber = bulletNumber;
    //         initializeBullet.bulletType = bulletType;
    //         initializeBullet.bulletObject = bullet.gameObject;
    //         initializeBullet.targetObject = targetObject;
    //         initializeBullet.isGrazed = false;
    //         movingBullet.bulletSpeedState = bulletSpeedState;
    //         movingBullet.bulletMoveSpeed = bulletMoveSpeed;
    //         movingBullet.bulletAccelerationMoveSpeed = bulletAccelerationMoveSpeed;
    //         movingBullet.bulletAccelerationMoveSpeedMax = bulletAccelerationMoveSpeedMax;
    //         movingBullet.bulletDecelerationMoveSpeed = bulletDecelerationMoveSpeed;
    //         movingBullet.bulletDecelerationMoveSpeedMin = bulletDecelerationMoveSpeedMin;
    //         movingBullet.bulletMoveSpeedLoopBool = bulletMoveSpeedLoopBool;
    //         movingBullet.bulletRotateState = bulletRotateState;
    //         movingBullet.bulletRotateSpeed = bulletRotateSpeed;
    //         movingBullet.bulletRotateLimit = bulletRotateLimit;
    //         switch (bulletDestinationType)
    //         {
    //             case 1:
    //                 movingBullet.bulletDestination = initializeBullet.GetBulletDestination(targetPosition);
    //                 break;
    //             case 2:
    //                 movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
    //                 break;
    //             case 3:
    //                 movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
    //                 break;
    //             case 4:
    //                 movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(targetPosition);
    //                 break;
    //             default:
    //                 movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector2(bullet.transform.position.x, bullet.transform.position.y - 5.0f));
    //                 break;
    //         }
    //         float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //         movingBullet.ChangeRotateAngle(angle - 90.0f + addRotateAngle);
    //         if (isLaserType.Equals(true))
    //         {
    //             if (!bullet.GetComponent<LaserBullet>()) bullet.AddComponent<LaserBullet>();
    //             LaserBullet laserBullet = bullet.GetComponent<LaserBullet>();
    //             laserBullet.laserWidth = laserWidth;
    //             laserBullet.isLaserEnabled = isLaserEnabled;
    //             laserBullet.isLaserDisabled = isLaserDisabled;
    //             laserBullet.laserEnableTime = laserEnableTime;
    //             laserBullet.laserEnableSpeed = laserEnableSpeed;
    //             laserBullet.laserDisableTime = laserDisableTime;
    //             laserBullet.laserDisableSpeed = laserDisableSpeed;
    //             laserBullet.laserRotateState = laserRotateState;
    //             laserBullet.laserRotateSpeed = laserRotateSpeed;
    //             laserBullet.isLaserRotateEnabled = isLaserRotateEnabled;
    //             laserBullet.isLaserRotateDisabled = isLaserRotateDisabled;
    //         }
    //         if (isSpriteRotate.Equals(true))
    //         {
    //             if (!bullet.GetComponent<ObjectRotate>()) bullet.AddComponent<ObjectRotate>();
    //             ObjectRotate objectRotate = bullet.GetComponent<ObjectRotate>();
    //             objectRotate.rotateSpeed = spriteRotateSpeed;
    //         }
    //         if (isGravity.Equals(true))
    //         {
    //             Rigidbody2D rigidbody2D = bullet.GetComponent<Rigidbody2D>();
    //             // rigidbody2D.AddForce(bullet.transform.position * velocity);
    //             rigidbody2D.AddForce(movingBullet.bulletDestination.normalized * velocity);
    //             rigidbody2D.gravityScale = gravityScale;
    //             movingBullet.isLookAt = isLookAt;
    //         }
    //         // 패턴 별 커스텀 스크립트는 여기에 집어넣기 !! 
    //         switch (addCustomScript)
    //         {
    //             case 1:
    //                 if (!bullet.GetComponent<Pattern6BulletRotate>()) bullet.AddComponent<Pattern6BulletRotate>();
    //                 break;
    //             case 2:
    //                 if (!bullet.GetComponent<Pattern6BulletAiming>()) bullet.AddComponent<Pattern6BulletAiming>();
    //                 break;
    //             case 3:
    //                 if (!bullet.GetComponent<Pattern7BulletAiming>()) bullet.AddComponent<Pattern7BulletAiming>();
    //                 break;
    //             case 4:
    //                 if (!bullet.GetComponent<Pattern8BulletAiming>()) bullet.AddComponent<Pattern8BulletAiming>();
    //                 break;
    //             case 5:
    //                 if (!bullet.GetComponent<Pattern9BulletRotate>()) bullet.AddComponent<Pattern9BulletRotate>();
    //                 break;
    //             case 6:
    //                 if (!bullet.GetComponent<Pattern14BulletRotate>()) bullet.AddComponent<Pattern14BulletRotate>();
    //                 break;
    //             case 7:
    //                 if (!bullet.GetComponent<Pattern19BulletRotate>()) bullet.AddComponent<Pattern19BulletRotate>();
    //                 break;
    //             case 8:
    //                 if (!bullet.GetComponent<Pattern20BulletRotate>()) bullet.AddComponent<Pattern20BulletRotate>();
    //                 break;
    //             case 9:
    //                 if (!bullet.GetComponent<Pattern21BulletRotate>()) bullet.AddComponent<Pattern21BulletRotate>();
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    // }
    // 
    // // 빈 탄막
    // public void EmptyBulletFire
    //     (int bulletNumber, GameObject emptyBullet, Vector2 bulletFirePosition, Vector3 bulletScale, string bulletTag,
    //     int bulletLayer, Transform bulletParent, int colliderType, float circleColliderRadius,
    //     float capsuleColliderSizeX, float capsuleColliderSizeY, float capsuleColliderOffsetX, float capsuleColliderOffsetY,
    //     float boxColliderSizeX, float boxColliderSizeY, float boxColliderOffsetX, float boxColliderOffsetY,
    //     float spriteAlpha, int spriteNumber, BulletType bulletType, GameObject targetObject,
    //     BulletSpeedState bulletSpeedState, float bulletMoveSpeed,
    //     float bulletAccelerationMoveSpeed, float bulletAccelerationMoveSpeedMax,
    //     float bulletDecelerationMoveSpeed, float bulletDecelerationMoveSpeedMin, bool bulletMoveSpeedLoopBool,
    //     BulletRotateState bulletRotateState, float bulletRotateSpeed, float bulletRotateLimit,
    //     int bulletDestinationType, Vector2 targetPosition, float addRotateAngle, int addCustomScript = 0,
    //     bool isSpriteRotate = false, float spriteRotateSpeed = 0.0f, bool isGravity = false, float velocity = 0.0f, float gravityScale = 0.0f, bool isLookAt = false,
    //     bool isLaserType = false, float laserWidth = 1.8f, bool isLaserEnabled = false, bool isLaserDisabled = false,
    //     float laserEnableTime = 0.0f, float laserEnableSpeed = 0.0f, float laserDisableTime = 0.0f, float laserDisableSpeed = 0.0f,
    //     BulletRotateState laserRotateState = BulletRotateState.BULLETROTATESTATE_NONE, float laserRotateSpeed = 0.0f,
    //     bool isLaserRotateEnabled = false, bool isLaserRotateDisabled = false)
    // {
    //     if (bulletManager.bulletPool.Count > 0)
    //     {
    //         emptyBullet.SetActive(true);
    //         ClearChild(emptyBullet);
    //         emptyBullet.transform.position = bulletFirePosition;
    //         emptyBullet.transform.localScale = bulletScale;
    //         emptyBullet.gameObject.tag = bulletTag;
    //         emptyBullet.gameObject.layer = bulletLayer;
    //         emptyBullet.transform.SetParent(bulletParent);
    //         if (!emptyBullet.GetComponent<SpriteRenderer>()) emptyBullet.AddComponent<SpriteRenderer>();
    //         if (!emptyBullet.GetComponent<InitializeBullet>()) emptyBullet.AddComponent<InitializeBullet>();
    //         if (!emptyBullet.GetComponent<MovingBullet>()) emptyBullet.AddComponent<MovingBullet>();
    //         if (!emptyBullet.GetComponent<EraseBullet>()) emptyBullet.AddComponent<EraseBullet>();
    //         SpriteRenderer spriteRenderer = emptyBullet.GetComponent<SpriteRenderer>();
    //         InitializeBullet initializeBullet = emptyBullet.GetComponent<InitializeBullet>();
    //         MovingBullet movingBullet = emptyBullet.GetComponent<MovingBullet>();
    //         switch (colliderType)
    //         {
    //             default:
    //             case 1:
    //                 if (!emptyBullet.GetComponent<CircleCollider2D>()) emptyBullet.AddComponent<CircleCollider2D>();
    //                 CircleCollider2D circleCollider2D = emptyBullet.GetComponent<CircleCollider2D>();
    //                 circleCollider2D.isTrigger = true;
    //                 circleCollider2D.radius = circleColliderRadius;
    //                 circleCollider2D.enabled = false;
    //                 break;
    //             case 2:
    //                 if (!emptyBullet.GetComponent<CapsuleCollider2D>()) emptyBullet.AddComponent<CapsuleCollider2D>();
    //                 CapsuleCollider2D capsuleCollider2D = emptyBullet.GetComponent<CapsuleCollider2D>();
    //                 capsuleCollider2D.isTrigger = true;
    //                 capsuleCollider2D.size = new Vector2(capsuleColliderSizeX, capsuleColliderSizeY);
    //                 capsuleCollider2D.offset = new Vector2(capsuleColliderOffsetX, capsuleColliderOffsetY);
    //                 capsuleCollider2D.enabled = false;
    //                 break;
    //             case 3:
    //                 if (!emptyBullet.GetComponent<BoxCollider2D>()) emptyBullet.AddComponent<BoxCollider2D>();
    //                 BoxCollider2D boxCollider2D = emptyBullet.GetComponent<BoxCollider2D>();
    //                 boxCollider2D.isTrigger = true;
    //                 boxCollider2D.size = new Vector2(boxColliderSizeX, boxColliderSizeY);
    //                 boxCollider2D.offset = new Vector2(boxColliderOffsetX, boxColliderOffsetY);
    //                 boxCollider2D.enabled = false;
    //                 break;
    //         }
    //         spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, spriteAlpha);
    //         spriteRenderer.sprite = spriteCollection[spriteNumber];
    //         spriteRenderer.sortingOrder = 3;
    //         initializeBullet.bulletNumber = bulletNumber;
    //         initializeBullet.bulletType = bulletType;
    //         initializeBullet.bulletObject = emptyBullet.gameObject;
    //         initializeBullet.targetObject = targetObject;
    //         initializeBullet.isGrazed = false;
    //         movingBullet.bulletSpeedState = bulletSpeedState;
    //         movingBullet.bulletMoveSpeed = bulletMoveSpeed;
    //         movingBullet.bulletAccelerationMoveSpeed = bulletAccelerationMoveSpeed;
    //         movingBullet.bulletAccelerationMoveSpeedMax = bulletAccelerationMoveSpeedMax;
    //         movingBullet.bulletDecelerationMoveSpeed = bulletDecelerationMoveSpeed;
    //         movingBullet.bulletDecelerationMoveSpeedMin = bulletDecelerationMoveSpeedMin;
    //         movingBullet.bulletMoveSpeedLoopBool = bulletMoveSpeedLoopBool;
    //         movingBullet.bulletRotateState = bulletRotateState;
    //         movingBullet.bulletRotateSpeed = bulletRotateSpeed;
    //         movingBullet.bulletRotateLimit = bulletRotateLimit;
    //         switch (bulletDestinationType)
    //         {
    //             case 1:
    //                 movingBullet.bulletDestination = initializeBullet.GetBulletDestination(targetPosition);
    //                 break;
    //             case 2:
    //                 movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
    //                 break;
    //             case 3:
    //                 movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination();
    //                 break;
    //             case 4:
    //                 movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(targetPosition);
    //                 break;
    //             default:
    //                 movingBullet.bulletDestination = initializeBullet.GetAimedBulletDestination(new Vector2(emptyBullet.transform.position.x, emptyBullet.transform.position.y - 5.0f));
    //                 break;
    //         }
    //         float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
    //         movingBullet.ChangeRotateAngle(angle - 90.0f + addRotateAngle);
    //         if (isLaserType.Equals(true))
    //         {
    //             if (!emptyBullet.GetComponent<LaserBullet>()) emptyBullet.AddComponent<LaserBullet>();
    //             LaserBullet laserBullet = emptyBullet.GetComponent<LaserBullet>();
    //             laserBullet.laserWidth = laserWidth;
    //             laserBullet.isLaserEnabled = isLaserEnabled;
    //             laserBullet.isLaserDisabled = isLaserDisabled;
    //             laserBullet.laserEnableTime = laserEnableTime;
    //             laserBullet.laserEnableSpeed = laserEnableSpeed;
    //             laserBullet.laserDisableTime = laserDisableTime;
    //             laserBullet.laserDisableSpeed = laserDisableSpeed;
    //             laserBullet.laserRotateState = laserRotateState;
    //             laserBullet.laserRotateSpeed = laserRotateSpeed;
    //             laserBullet.isLaserRotateEnabled = isLaserRotateEnabled;
    //             laserBullet.isLaserRotateDisabled = isLaserRotateDisabled;
    //         }
    //         if (isSpriteRotate.Equals(true))
    //         {
    //             if (!emptyBullet.GetComponent<ObjectRotate>()) emptyBullet.AddComponent<ObjectRotate>();
    //             ObjectRotate objectRotate = emptyBullet.GetComponent<ObjectRotate>();
    //             objectRotate.rotateSpeed = spriteRotateSpeed;
    //         }
    //         if (isGravity.Equals(true))
    //         {
    //             Rigidbody2D rigidbody2D = emptyBullet.GetComponent<Rigidbody2D>();
    //             rigidbody2D.velocity = movingBullet.bulletDestination.normalized * velocity;
    //             rigidbody2D.gravityScale = gravityScale;
    //             movingBullet.isLookAt = isLookAt;
    //         }
    //         // 패턴 별 커스텀 스크립트는 여기에 집어넣기 !! 
    //         switch (addCustomScript)
    //         {
    //             case 1:
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    // }
}
