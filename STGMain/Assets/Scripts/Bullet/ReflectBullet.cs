using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ReflectBullet : MonoBehaviour
{
    /// 아직 반사 효과를 줄 패턴이 없으므로 나중에 수정할 것
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌 대상이 적 탄막인지 확인
        if (collision.CompareTag("BULLET_ENEMY"))
        {
            BulletState bulletState = collision.gameObject.GetComponent<BulletState>();
            BulletMove bulletMove = collision.gameObject.GetComponent<BulletMove>();

            // 대상 탄막이 반사 특성을 가지고 있는지 확인
            if (!bulletState.bulletReflectState.Equals(BulletReflectState.BULLETREFLECTSTATE_NONE) && gameObject.CompareTag("REFLECTZONE"))
            {
                // 이미 1회 이상 반사된 탄막인지 체크
                if (bulletState.reflectCount < bulletState.reflectLimit)
                {
                    // 최초 반사일 경우 반사 카운트 증가
                    bulletState.reflectCount++;
                    // 충돌한 영역이 좌우 반사 영역일 경우의 처리
                    if (gameObject.layer.Equals(LayerMask.NameToLayer("REFLECTZONE_LEFTRIGHT")))
                    {
                        bulletMove.ChangeRotateAngle(bulletMove.GetAngle() * -1);
                    }
                    // 충돌한 영역이 상하 반사 영역일 경우의 처리
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

                    // 탄막이 반사될 때 스프라이트 변화 및 이펙트 출력이 설정되어 있을 경우의 처리
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
