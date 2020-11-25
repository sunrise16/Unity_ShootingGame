using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerHitPoint : MonoBehaviour
{
    private SpriteRenderer hitPointSprite;
    
    private float alphaValue;

	void Start ()
    {
        hitPointSprite = GetComponent<SpriteRenderer>();
        
        alphaValue = 0.0f;

        StartCoroutine(HitPointAlpha());
	}

    public IEnumerator HitPointAlpha()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.LeftShift))
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
