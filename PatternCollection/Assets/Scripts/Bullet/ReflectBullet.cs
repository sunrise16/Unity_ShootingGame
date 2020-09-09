using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ReflectBullet : MonoBehaviour
{
    private EnemyFire enemyFire;

    public int reflectCount;
    public int reflectLimit;
    public bool isSpriteChange;
    public bool isEffectOutput;
    public int changeSpriteNumber;
    public int effectSpriteNumber;
    public float scaleDownSpeed;
    public float scaleDownTime;
    public float alphaUpSpeed;

    void Start()
    {
        enemyFire = GameObject.Find("ENEMY").GetComponent<EnemyFire>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        InitializeBullet initializeBullet = this.GetComponent<InitializeBullet>();
        MovingBullet movingBullet = this.GetComponent<MovingBullet>();
        
        if (!initializeBullet.bulletReflect.Equals(BulletReflect.BULLETREFLECT_NONE) && collision.tag.Equals("REFLECTZONE"))
        {
            if (this.gameObject.activeSelf.Equals(true) && reflectCount < reflectLimit)
            {
                reflectCount++;
                if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("REFLECTZONE_LEFTRIGHT")))
                {
                    movingBullet.ChangeRotateAngle(movingBullet.GetAngle() * -1);
                }
                else if ((collision.gameObject.layer.Equals(LayerMask.NameToLayer("REFLECTZONE_TOP"))) ||
                    (collision.gameObject.layer.Equals(LayerMask.NameToLayer("REFLECTZONE_BOTTOM")) && initializeBullet.bulletReflect.Equals(BulletReflect.BULLETREFLECT_CONTAINBOTTOM)))
                {
                    if (movingBullet.GetAngle() >= 0.0f && movingBullet.GetAngle() <= 180.0f)
                    {
                        movingBullet.ChangeRotateAngle(180.0f - movingBullet.GetAngle());
                    }
                    else
                    {
                        movingBullet.ChangeRotateAngle(-180.0f - movingBullet.GetAngle());
                    }
                }

                if (!(collision.gameObject.layer.Equals(LayerMask.NameToLayer("REFLECTZONE_BOTTOM")) && !initializeBullet.bulletReflect.Equals(BulletReflect.BULLETREFLECT_CONTAINBOTTOM)))
                {
                    if (isSpriteChange.Equals(true))
                    {
                        spriteRenderer.sprite = enemyFire.spriteCollection[changeSpriteNumber];
                    }
                    if (isEffectOutput.Equals(true))
                    {
                        StartCoroutine(enemyFire.CreateBulletFireEffect(effectSpriteNumber, scaleDownSpeed, scaleDownTime, alphaUpSpeed, transform.position));
                    }
                }
            }
        }
    }
}
