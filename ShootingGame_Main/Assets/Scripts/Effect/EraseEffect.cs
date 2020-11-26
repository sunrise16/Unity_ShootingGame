using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EraseEffect : MonoBehaviour
{
    private ObjectPool effect;
    private Transform effectParent;

    void Start()
    {
        effect = GameObject.Find("EffectPool").GetComponent<ObjectPool>();
        effectParent = GameObject.Find("EFFECT").transform;
    }

    public void ClearEffect()
    {
        effect.objectPool.Enqueue(gameObject);
        gameObject.transform.SetParent(effectParent);
        gameObject.transform.position = new Vector2(0.0f, 0.0f);
        gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        gameObject.transform.localScale = new Vector3(10.0f, 10.0f, 0.0f);

        // 비활성화
        gameObject.SetActive(false);
    }
}