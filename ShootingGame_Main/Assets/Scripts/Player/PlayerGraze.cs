using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerGraze : MonoBehaviour
{
    private PlayerStatus playerStatus;

    private void Start()
    {
        playerStatus = GameObject.Find("CHARACTER").transform.Find("Player").GetComponent<PlayerStatus>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BULLET_ENEMY") & playerStatus.GetSpriteOff().Equals(false))
        {
            BulletState bulletState = collision.GetComponent<BulletState>();

            if (bulletState.isGrazed.Equals(false))
            {
                bulletState.isGrazed = true;
                GameData.currentGraze++;
                GameData.currentScore += (10 * GameData.currentGraze);
            }
        }
    }
}
