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
        switch (poolObject.tag)
        {
            case "BULLET_PLAYER":
                poolSize = 80;
                break;
            case "ENEMY":
                poolSize = 100;
                break;
            case "ITEM":
                poolSize = 1000;
                break;
            case "BULLET_ENEMY":
                poolSize = 2000;
                break;
            case "EFFECT":
                poolSize = 3000;
                break;
            default:
                poolSize = 500;
                break;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(poolObject);
            obj.SetActive(false);
            obj.transform.SetParent(poolParent.transform);
        }
    }
}
