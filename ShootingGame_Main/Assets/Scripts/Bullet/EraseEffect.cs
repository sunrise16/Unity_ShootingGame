using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EraseEffect : MonoBehaviour
{
    private BulletManager effectManager;
    private Transform effectParent;

    void Start()
    {
        effectManager = GameObject.Find("EffectManager").GetComponent<BulletManager>();
        effectParent = GameObject.Find("EFFECT").transform;
    }

    public void ClearEffect()
    {
        effectManager.bulletPool.Enqueue(gameObject);
        gameObject.transform.SetParent(effectParent);
        gameObject.transform.position = new Vector2(0.0f, 0.0f);
        gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        gameObject.transform.localScale = new Vector3(14.0f, 14.0f, 0.0f);

        // 비활성화
        gameObject.SetActive(false);
    }
}