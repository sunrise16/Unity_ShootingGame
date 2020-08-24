using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerGraze : MonoBehaviour
{
    void Update()
    {
        transform.position = GameObject.Find("PLAYER").transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((collision.gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE1") || collision.gameObject.layer == LayerMask.NameToLayer("BULLET_ENEMY_DESTROYZONE2")) &&
            collision.gameObject.tag != "BULLET_EMPTY") && collision.GetComponent<InitializeBullet>().isGrazed == false)
        {
            GameObject.Find("PLAYER").GetComponent<PlayerDatabase>().grazeCount++;
            collision.GetComponent<InitializeBullet>().isGrazed = true;
        }
    }
}
