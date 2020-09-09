using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerHitPoint : MonoBehaviour
{
    private PlayerMove playerMove;
    private SpriteRenderer hitPointSprite;
    
    private float alphaValue;

	void Start ()
    {
        playerMove = GameObject.Find("PLAYER").GetComponent<PlayerMove>();
        hitPointSprite = GetComponent<SpriteRenderer>();
        
        alphaValue = 0.0f;

        StartCoroutine(HitPointAlpha());
	}

    public IEnumerator HitPointAlpha()
    {
        while (true)
        {
            if (playerMove.isSlowMoveMode.Equals(true))
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

            yield return new WaitForEndOfFrame();
        }
    }
}
