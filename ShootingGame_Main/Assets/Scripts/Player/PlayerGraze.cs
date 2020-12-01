using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerGraze : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BULLET_ENEMY"))
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
