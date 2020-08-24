using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerHitPoint : MonoBehaviour
{
    private SpriteRenderer hitPointSprite;

    private float rotateValue;
    private float alphaValue;

	void Start ()
    {
        hitPointSprite = GetComponent<SpriteRenderer>();

        rotateValue = 0.0f;
        alphaValue = 0.0f;
	}
	
	void Update ()
    {
        rotateValue += 2.0f;
        transform.rotation = Quaternion.Euler(0, 0, rotateValue);

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
	}
}
