using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public GameObject bulletObject;
    public GameObject bulletParent;
    // public Queue<GameObject> bulletPool;
    public Stack<GameObject> bulletPool;

    int bulletPoolSize = 10000;
    
    void Start()
    {
        InitObjectPooling();
    }
    
    void InitObjectPooling()
    {
        if (gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_PRIMARY") || gameObject.layer == LayerMask.NameToLayer("BULLET_PLAYER_SECONDARY"))
        {
            bulletPoolSize = 1000;
        }

        // bulletPool = new Queue<GameObject>();
        bulletPool = new Stack<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletObject);
            bullet.SetActive(false);
            bullet.transform.SetParent(bulletParent.transform);
            // bulletPool.Enqueue(bullet);
            bulletPool.Push(bullet);
        }
    }
}
