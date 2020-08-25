using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EraseBullet : MonoBehaviour
{
    // 커스텀 컴포넌트를 만들 경우 계속 추가할 것!
    private BulletManager bulletManager;
    private InitializeBullet initializeBullet;
    private MovingBullet movingBullet;
    private EraseBullet eraseBullet;
    private Stage1BulletFragmentation stage1BulletFragmentation;
    private Stage5BulletCreate stage5BulletCreate;
    private Stage5BulletCloneFire stage5BulletCloneFire;

    void Start()
    {
        initializeBullet = GetComponent<InitializeBullet>();
        movingBullet = GetComponent<MovingBullet>();
        eraseBullet = GetComponent<EraseBullet>();
        stage1BulletFragmentation = GetComponent<Stage1BulletFragmentation>();
        stage5BulletCreate = GetComponent<Stage5BulletCreate>();
        stage5BulletCloneFire = GetComponent<Stage5BulletCloneFire>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1") && (collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_INNER"))) ||
            (gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE2") && (collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_OUTER"))))
        {
            if (gameObject.tag == "BULLET")
            {
                ClearBullet();
            }
            else if (gameObject.tag == "BULLET_EMPTY")
            {
                ClearEmptyBullet();
            }
        }
        else if (((gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1") || gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE2") ||
            gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_LASER")) && gameObject.tag != "BULLET_EMPTY") && collision.gameObject.tag == "PLAYER")
        {
            GameObject.Find("PLAYER").GetComponent<PlayerDatabase>().hitCount++;
            // StartCoroutine(GameObject.Find("PLAYER").GetComponent<PlayerDie>().CreateDieEffect());
            ClearBullet();
        }
        else if (gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY") && collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_INNER"))
        {
            ClearPlayerBullet(1);
        }
        else if (gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_SECONDARY") && collision.gameObject.layer == LayerMask.NameToLayer("DESTROYZONE_OUTER"))
        {
            ClearPlayerBullet(2);
        }
    }
    
    public void ClearBullet()
    {
        bulletManager = GameObject.Find("BulletManager").transform.GetChild(GetComponent<InitializeBullet>().bulletPoolIndex).
            transform.GetChild(GetComponent<InitializeBullet>().bulletPoolChildIndex).GetComponent<BulletManager>();
        bulletManager.bulletPool.Enqueue(gameObject);
        gameObject.transform.SetParent(GameObject.Find("BULLET").transform.GetChild(GetComponent<InitializeBullet>().bulletPoolIndex).
            transform.GetChild(GetComponent<InitializeBullet>().bulletPoolChildIndex).transform);
        gameObject.transform.position = new Vector2(0.0f, 0.0f);

        // 컴포넌트 제거
        Destroy(initializeBullet);
        Destroy(movingBullet);
        Destroy(stage1BulletFragmentation);
        Destroy(stage5BulletCreate);
        Destroy(stage5BulletCloneFire);

        Destroy(eraseBullet);
        gameObject.SetActive(false);
    }
    public void ClearEmptyBullet()
    {
        bulletManager = GameObject.Find("BulletManager").transform.GetChild(GetComponent<InitializeBullet>().bulletPoolIndex).GetComponent<BulletManager>();
        bulletManager.bulletPool.Enqueue(gameObject);
        gameObject.transform.SetParent(GameObject.Find("BULLET").transform.GetChild(GetComponent<InitializeBullet>().bulletPoolIndex).transform);
        gameObject.transform.position = new Vector2(0.0f, 0.0f);

        // 컴포넌트 제거
        Destroy(initializeBullet);
        Destroy(movingBullet);
        Destroy(stage1BulletFragmentation);
        Destroy(stage5BulletCreate);
        Destroy(stage5BulletCloneFire);

        Destroy(eraseBullet);
        gameObject.SetActive(false);
    }
    public void ClearPlayerBullet(int bulletPoolIndex)
    {
        bulletManager = GameObject.Find("BulletManager").transform.GetChild(bulletPoolIndex).GetComponent<BulletManager>();
        bulletManager.bulletPool.Enqueue(gameObject);
        gameObject.transform.SetParent(GameObject.Find("BULLET").transform.GetChild(bulletPoolIndex).transform);
        gameObject.transform.position = new Vector2(0.0f, 0.0f);

        // 컴포넌트 제거
        Destroy(initializeBullet);
        Destroy(movingBullet);
        Destroy(stage1BulletFragmentation);
        Destroy(stage5BulletCreate);
        Destroy(stage5BulletCloneFire);

        Destroy(eraseBullet);
        gameObject.SetActive(false);
    }
}
