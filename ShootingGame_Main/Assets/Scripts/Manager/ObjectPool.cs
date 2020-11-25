using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject poolObject;
    public GameObject poolParent;
    public Queue<GameObject> objectPool;

    private int poolSize;
    
    void Start()
    {
        if (poolObject.CompareTag("ENEMY"))
        {
            poolSize = 100;
        }
        else if (poolObject.CompareTag("BULLET_PLAYER"))
        {
            poolSize = 400;
        }
        else if (poolObject.CompareTag("BULLET_ENEMY"))
        {
            poolSize = 3000;
        }
        else if (poolObject.CompareTag("EFFECT"))
        {
            poolSize = 6000;
        }

        objectPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(poolObject);
            bullet.SetActive(false);
            bullet.transform.SetParent(poolParent.transform);
            objectPool.Enqueue(bullet);
        }
    }
}
