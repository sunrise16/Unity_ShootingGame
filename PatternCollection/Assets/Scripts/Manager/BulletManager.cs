using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public GameObject bulletObject;
    public GameObject bulletParent;
    public Queue<GameObject> bulletPool;

    int bulletPoolSize = 1000;
    
    void Start()
    {
        InitObjectPooling();
    }
    
    void InitObjectPooling()
    {
        bulletPool = new Queue<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletObject);
            bullet.SetActive(false);
            bullet.transform.SetParent(bulletParent.transform);
            bulletPool.Enqueue(bullet);

            if (bullet.layer == LayerMask.NameToLayer("BULLET_ENEMY_LASER"))
            {
                bullet.GetComponent<LineRenderer>().startWidth = 0.0f;
                bullet.GetComponent<LineRenderer>().endWidth = 0.0f;
            }
        }
    }
}
