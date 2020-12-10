using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EraseBullet : MonoBehaviour
{
    private Transform circleBulletParent;
    private Transform capsuleBulletParent;
    private Transform boxBulletParent;

    private void Start()
    {
        circleBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Circle");
        capsuleBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Capsule");
        boxBulletParent = GameObject.Find("BULLET").transform.Find("EnemyBullet").transform.Find("EnemyBullet_Rectangle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌 대상이 적 탄막일 경우
        if (collision.CompareTag("BULLET_ENEMY"))
        {
            // 충돌당한 오브젝트가 탄막 제거 영역일 경우
            if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_INNER1")) &&
            collision.gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_ENEMY_INNER1")))
            {
                ClearBullet(collision.gameObject);
            }
            else if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_INNER2")) &&
                collision.gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_ENEMY_INNER2")))
            {
                ClearBullet(collision.gameObject);
            }
            else if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_OUTER1")) &&
                collision.gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_ENEMY_OUTER1")))
            {
                ClearBullet(collision.gameObject);
            }
            else if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_OUTER2")) &&
                collision.gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_ENEMY_OUTER2")))
            {
                ClearBullet(collision.gameObject);
            }
        }
        // 충돌당한 오브젝트가 전체 제거 영역일 경우 화면 전체 탄 소거 실행
        else if (gameObject.layer.Equals(LayerMask.NameToLayer("DESTROYZONE_ALL")) && collision.CompareTag("BULLET_ENEMY"))
        {
            ClearBulletAll(collision.gameObject);
        }
    }

    // 탄막 제거 함수
    public void ClearBullet(GameObject bullet)
    {
        if (bullet.GetComponent<CircleCollider2D>())
        {
            bullet.transform.SetParent(circleBulletParent);
        }
        else if (bullet.GetComponent<CapsuleCollider2D>())
        {
            bullet.transform.SetParent(capsuleBulletParent);
        }
        else if (bullet.GetComponent<BoxCollider2D>())
        {
            bullet.transform.SetParent(boxBulletParent);
        }
        bullet.transform.position = new Vector2(0.0f, 0.0f);
        bullet.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        bullet.transform.localScale = new Vector3(1.8f, 1.8f, 1.0f);

        bullet.SetActive(false);
    }

    // 탄막 전체 제거 함수 (제작중)
    public void ClearBulletAll(GameObject bullet)
    {

    }
}
