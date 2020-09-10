using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Stage2BulletFragmentation : MonoBehaviour
{
    private Transform enemyBullet;
    private GameObject playerObject;
    private BulletManager bulletManager;
    private MovingBullet movingBullet;
    private EraseBullet eraseBullet;
    private EnemyFire enemyFire;

    void Start()
    {
        enemyBullet = GameObject.Find("BULLET").transform.GetChild(4);
        playerObject = GameObject.Find("PLAYER");
        bulletManager = GameObject.Find("BulletManager").transform.GetChild(0).GetComponent<BulletManager>();
        movingBullet = GetComponent<MovingBullet>();
        eraseBullet = GetComponent<EraseBullet>();
        enemyFire = GameObject.Find("ENEMY").GetComponent<EnemyFire>();

        StartCoroutine(Fragmentation());
    }

    public IEnumerator Fragmentation()
    {
        while (true)
        {
            if (movingBullet.bulletMoveSpeed <= 0.0f)
            {
                Vector2 bulletFirePosition = transform.position;

                // 탄막 2 이펙트
                StartCoroutine(enemyFire.CreateBulletFireEffect(303, 1.0f, 12.0f, 0.3f, 0.1f, 0.5f, bulletFirePosition));

                // 탄막 2 발사 (초록색 / 노란색 소형 테두리 원탄) (랜덤탄)
                StartCoroutine(FragmentationAttack1(bulletFirePosition));
                // 탄막 3 발사 (초록색 / 노란색 소형 별탄) (랜덤탄)
                StartCoroutine(FragmentationAttack2(bulletFirePosition));
                // 탄막 4 발사 (초록색 / 노란색 대형 별탄) (랜덤탄)
                StartCoroutine(FragmentationAttack3(bulletFirePosition));

                yield return new WaitForSeconds(0.1f);

                bulletManager.bulletPool.Enqueue(gameObject);
                transform.SetParent(enemyBullet);
                eraseBullet.ClearBullet();
                gameObject.SetActive(false);
            }

            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator FragmentationAttack1(Vector2 bulletFirePosition)
    {
        int spriteIndex = 0;

        for (int i = 0; i < 3; i++)
        {
            spriteIndex++;

            // 충돌체 생성
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                EnemyFire.ClearChild(bullet);
                bullet.transform.position = bulletFirePosition;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                bullet.transform.SetParent(enemyFire.enemyBulletTemp2);
                if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<ObjectRotate>()) bullet.AddComponent<ObjectRotate>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                ObjectRotate objectRotate = bullet.GetComponent<ObjectRotate>();
                Rigidbody2D rigidbody2D = bullet.GetComponent<Rigidbody2D>();
                spriteRenderer.sprite = enemyFire.spriteCollection[203 + ((spriteIndex % 3) * 2)];
                spriteRenderer.sortingOrder = 3;
                circleCollider2D.isTrigger = true;
                circleCollider2D.radius = 0.015f;
                circleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 0.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f + Random.Range(-40.0f, 40.0f));
                objectRotate.rotateSpeed = 120.0f;
                rigidbody2D.velocity = movingBullet.bulletDestination.normalized * Random.Range(1.0f, 2.5f);
                rigidbody2D.gravityScale = 0.2f;
            }
            else enemyFire.AddBulletPool();
        }
        
        yield return null;
    }
    public IEnumerator FragmentationAttack2(Vector2 bulletFirePosition)
    {
        int spriteIndex = 0;

        for (int i = 0; i < 3; i++)
        {
            spriteIndex++;

            // 충돌체 생성
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                EnemyFire.ClearChild(bullet);
                bullet.transform.position = bulletFirePosition;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                bullet.transform.SetParent(enemyFire.enemyBulletTemp2);
                if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<ObjectRotate>()) bullet.AddComponent<ObjectRotate>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                ObjectRotate objectRotate = bullet.GetComponent<ObjectRotate>();
                Rigidbody2D rigidbody2D = bullet.GetComponent<Rigidbody2D>();
                spriteRenderer.sprite = enemyFire.spriteCollection[171 + ((spriteIndex % 3) * 2)];
                spriteRenderer.sortingOrder = 3;
                circleCollider2D.isTrigger = true;
                circleCollider2D.radius = 0.015f;
                circleCollider2D.offset = new Vector2(0.0f, -0.005f);
                circleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 0.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f + Random.Range(-40.0f, 40.0f));
                objectRotate.rotateSpeed = 120.0f;
                rigidbody2D.velocity = movingBullet.bulletDestination.normalized * Random.Range(1.0f, 2.5f);
                rigidbody2D.gravityScale = 0.2f;
            }
            else enemyFire.AddBulletPool();
        }

        yield return null;
    }
    public IEnumerator FragmentationAttack3(Vector2 bulletFirePosition)
    {
        for (int i = 0; i < 3; i++)
        {
            // 충돌체 생성
            if (bulletManager.bulletPool.Count > 0)
            {
                GameObject bullet = bulletManager.bulletPool.Dequeue();
                bullet.SetActive(true);
                EnemyFire.ClearChild(bullet);
                bullet.transform.position = bulletFirePosition;
                bullet.gameObject.tag = "BULLET_ENEMY";
                bullet.gameObject.layer = LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2");
                bullet.transform.SetParent(enemyFire.enemyBulletTemp2);
                if (!bullet.GetComponent<CircleCollider2D>()) bullet.AddComponent<CircleCollider2D>();
                if (!bullet.GetComponent<InitializeBullet>()) bullet.AddComponent<InitializeBullet>();
                if (!bullet.GetComponent<MovingBullet>()) bullet.AddComponent<MovingBullet>();
                if (!bullet.GetComponent<EraseBullet>()) bullet.AddComponent<EraseBullet>();
                if (!bullet.GetComponent<SpriteRenderer>()) bullet.AddComponent<SpriteRenderer>();
                if (!bullet.GetComponent<ObjectRotate>()) bullet.AddComponent<ObjectRotate>();
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                CircleCollider2D circleCollider2D = bullet.GetComponent<CircleCollider2D>();
                InitializeBullet initializeBullet = bullet.GetComponent<InitializeBullet>();
                MovingBullet movingBullet = bullet.GetComponent<MovingBullet>();
                ObjectRotate objectRotate = bullet.GetComponent<ObjectRotate>();
                Rigidbody2D rigidbody2D = bullet.GetComponent<Rigidbody2D>();
                spriteRenderer.sprite = enemyFire.spriteCollection[294 + i];
                spriteRenderer.sortingOrder = 3;
                circleCollider2D.isTrigger = true;
                circleCollider2D.radius = 0.045f;
                circleCollider2D.offset = new Vector2(0.0f, -0.01f);
                circleCollider2D.enabled = false;
                initializeBullet.bulletType = BulletType.BULLETTYPE_NORMAL;
                initializeBullet.bulletObject = bullet.gameObject;
                initializeBullet.targetObject = playerObject;
                initializeBullet.isGrazed = false;
                movingBullet.bulletMoveSpeed = 0.0f;
                movingBullet.bulletSpeedState = BulletSpeedState.BULLETSPEEDSTATE_NORMAL;
                movingBullet.bulletRotateState = BulletRotateState.BULLETROTATESTATE_NONE;
                movingBullet.bulletDestination = initializeBullet.GetRandomAimedBulletDestination();
                float angle = Mathf.Atan2(movingBullet.bulletDestination.y, movingBullet.bulletDestination.x) * Mathf.Rad2Deg;
                movingBullet.ChangeRotateAngle(angle - 90.0f + Random.Range(-40.0f, 40.0f));
                objectRotate.rotateSpeed = 120.0f;
                rigidbody2D.velocity = movingBullet.bulletDestination.normalized * Random.Range(1.0f, 2.5f);
                rigidbody2D.gravityScale = 0.2f;
            }
            else enemyFire.AddBulletPool();
        }

        yield return null;
    }
}
