using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerHitPoint : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private SpriteRenderer hitPointSprite;
    
    private float alphaValue;                   // 피탄점 알파값 (투명도)

	private void Start ()
    {
        playerStatus = GameObject.Find("CHARACTER").transform.Find("Player").GetComponent<PlayerStatus>();
        hitPointSprite = GetComponent<SpriteRenderer>();
        
        alphaValue = 0.0f;
	}

    private void Update()
    {
        if (playerStatus.GetSpriteOff().Equals(false))
        {
            // 저속 이동 모드 여부에 따라 피탄점 스프라이트 출력
            if (playerStatus.GetSlowMove().Equals(true))
            {
                alphaValue += 0.1f;
                if (alphaValue >= 1.0f)
                {
                    alphaValue = 1.0f;
                }
            }
            else
            {
                alphaValue -= 0.1f;
                if (alphaValue <= 0.0f)
                {
                    alphaValue = 0.0f;
                }
            }
        }
        else
        {
            alphaValue = 0.0f;
        }
        hitPointSprite.color = new Color(hitPointSprite.color.r, hitPointSprite.color.g, hitPointSprite.color.b, alphaValue);
    }
}
