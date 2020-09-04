using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public GameObject bulletObject;
    public GameObject bulletParent;
    public Queue<GameObject> bulletPool;

    int bulletPoolSize = 10000;
    
    void Start()
    {
        InitObjectPooling();
    }
    
    void InitObjectPooling()
    {
        if (gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY")) || gameObject.layer.Equals(LayerMask.NameToLayer("BULLET_PLAYER_SECONDARY")))
        {
            bulletPoolSize = 1000;
        }
        else if (gameObject.tag.Equals("EFFECT"))
        {
            bulletPoolSize = 500;
        }

        bulletPool = new Queue<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletObject);
            bullet.SetActive(false);
            bullet.transform.SetParent(bulletParent.transform);
            bulletPool.Enqueue(bullet);
        }
    }
}
