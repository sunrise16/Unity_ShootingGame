using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EraseBullet : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private BoxCollider2D boxCollider;
    private CapsuleCollider2D capsuleCollider;
    private BulletManager bulletManager;
    private Transform enemyBullet;
    private PlayerDatabase playerDatabase;
    private InitializeBullet initializeBullet;
    private MovingBullet movingBullet;
    private LaserBullet laserBullet;

    // 커스텀 컴포넌트를 만들 경우 계속 추가할 것!
    private Stage1BulletFragmentation stage1BulletFragmentation;
    private Stage5BulletCreate stage5BulletCreate;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        bulletManager = GameObject.Find("BulletManager").transform.Find("EnemyBullet").GetComponent<BulletManager>();
        enemyBullet = GameObject.Find("BULLET").transform.Find("EnemyBullet");
        playerDatabase = GameObject.Find("PLAYER").GetComponent<PlayerDatabase>();
        initializeBullet = GetComponent<InitializeBullet>();
        movingBullet = GetComponent<MovingBullet>();
        laserBullet = GetComponent<LaserBullet>();
        stage1BulletFragmentation = GetComponent<Stage1BulletFragmentation>();
        stage5BulletCreate = GetComponent<Stage5BulletCreate>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER1") && collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_INNER1"))
        {
            ClearBullet();
        }
        else if (gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_INNER2") && collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_INNER2"))
        {
            ClearBullet();
        }
        else if (gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER1") && collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_OUTER1"))
        {
            ClearBullet();
        }
        else if (gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE_OUTER2") && collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_OUTER2"))
        {
            ClearBullet();
        }
        else if ((gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_LASER") && gameObject.GetComponent<InitializeBullet>().bulletType == BulletType.BULLETTYPE_LASER_MOVE) &&
            collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_LASER"))
        {
            ClearBullet();
        }
        else if ((gameObject.tag == "BULLET_ENEMY" || gameObject.tag == "BULLET_ENEMY_EMPTY") && collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_ALL"))
        {
            ClearBullet();
        }
        else if (gameObject.tag == "BULLET_ENEMY" && collision.gameObject.tag == "PLAYER")
        {
            playerDatabase.hitCount++;
            // StartCoroutine(GameObject.Find("PLAYER").GetComponent<PlayerDie>().CreateDieEffect());
            ClearBullet();
        }
    }

    public void ClearBullet()
    {
        EnemyFire.ClearChild(gameObject);
        
        bulletManager.bulletPool.Enqueue(gameObject);
        gameObject.transform.SetParent(enemyBullet.transform);
        gameObject.transform.position = new Vector2(0.0f, 0.0f);
        gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        gameObject.transform.localScale = new Vector3(1.8f, 1.8f, 0.0f);

        // 컴포넌트 제거
        Destroy(spriteRenderer);
        Destroy(circleCollider);
        Destroy(boxCollider);
        Destroy(capsuleCollider);
        Destroy(initializeBullet);
        Destroy(movingBullet);
        Destroy(laserBullet);

        // 커스텀 컴포넌트 제거
        Destroy(stage1BulletFragmentation);
        Destroy(stage5BulletCreate);

        // 최종 컴포넌트 제거 후 비활성화
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
    public IEnumerator AutoClearBullet(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        ClearBullet();
    }
}
