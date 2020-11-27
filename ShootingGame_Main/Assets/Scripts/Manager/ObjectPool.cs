using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private int poolSize;

    public GameObject poolObject;
    public GameObject poolParent;
    
    private void Start()
    {
        if (poolObject.CompareTag("BULLET_PLAYER"))
        {
            poolSize = 80;
        }
        else if (poolObject.CompareTag("ENEMY"))
        {
            poolSize = 100;
        }
        else if (poolObject.CompareTag("BULLET_ENEMY"))
        {
            poolSize = 2000;
        }
        else if (poolObject.CompareTag("EFFECT"))
        {
            poolSize = 3000;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(poolObject);
            obj.SetActive(false);
            obj.transform.SetParent(poolParent.transform);
        }
    }
}
