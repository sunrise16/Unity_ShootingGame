using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private SpriteRenderer spriteRenderer;

    private float alphaValue;                   // 스프라이트 이미지의 알파값 (투명도)
    private float blinkDelay;                   // 깜빡거리는 시간 간격

	private void Start()
    {
        playerStatus = GameObject.Find("CHARACTER").transform.Find("Player").GetComponent<PlayerStatus>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        alphaValue = 0.4f;
        blinkDelay = 0.0f;
	}
	
	private void Update()
    {
		if (playerStatus.GetBlinking().Equals(true))
        {
            blinkDelay += Time.deltaTime;
            if (blinkDelay >= 0.03f)
            {
                blinkDelay = 0.0f;
                if (alphaValue <= 0.4f)
                {
                    alphaValue = 1.0f;
                }
                else if (alphaValue >= 1.0f)
                {
                    alphaValue = 0.4f;
                }
            }
        }
        else
        {
            alphaValue = 1.0f;
        }
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, alphaValue);
	}
}
