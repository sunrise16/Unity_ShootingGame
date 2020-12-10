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
        // 부딫힌 오브젝트가 적 탄환일 경우
        if (collision.CompareTag("BULLET_ENEMY") & playerStatus.GetSpriteOff().Equals(false))
        {
            BulletState bulletState = collision.GetComponent<BulletState>();

            // 해당 탄이 아직 그레이즈 되지 않은 상태일 때에만 그레이즈 횟수 체크
            if (bulletState.isGrazed.Equals(false))
            {
                // 효과음 재생
                SoundManager.instance.PlaySE(18);

                bulletState.isGrazed = true;
                GameData.currentGraze++;
                GameData.currentScore += (10 * GameData.currentGraze);
            }
        }
    }
}
