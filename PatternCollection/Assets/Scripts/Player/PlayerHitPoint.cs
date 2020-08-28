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
	}

    public IEnumerator AlphaUp()
    {
        while (true)
        {
            alphaValue += 0.1f;
            hitPointSprite.color = new Color(hitPointSprite.color.r, hitPointSprite.color.g, hitPointSprite.color.b, alphaValue);

            if (alphaValue >= 1.0f)
            {
                alphaValue = 1.0f;
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator AlphaDown()
    {
        while (true)
        {
            alphaValue -= 0.1f;
            hitPointSprite.color = new Color(hitPointSprite.color.r, hitPointSprite.color.g, hitPointSprite.color.b, alphaValue);

            if (alphaValue <= 0.0f)
            {
                alphaValue = 0.0f;
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
