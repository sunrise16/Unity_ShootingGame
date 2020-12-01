using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerHitPoint : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private SpriteRenderer hitPointSprite;
    
    private float alphaValue;

	private void Start ()
    {
        playerStatus = GameObject.Find("CHARACTER").transform.Find("Player").GetComponent<PlayerStatus>();
        hitPointSprite = GetComponent<SpriteRenderer>();
        
        alphaValue = 0.0f;
	}

    private void Update()
    {
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
        hitPointSprite.color = new Color(hitPointSprite.color.r, hitPointSprite.color.g, hitPointSprite.color.b, alphaValue);
    }
}
