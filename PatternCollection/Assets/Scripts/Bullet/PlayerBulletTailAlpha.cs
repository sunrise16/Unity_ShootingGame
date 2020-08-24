using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletTailAlpha : MonoBehaviour
{
    private Color color;

    public float tailAlpha { get; set; }
	
    void Start()
    {
        color = GetComponent<SpriteRenderer>().color;

        tailAlpha = 0.0f;
    }

	void Update()
    {
        tailAlpha += 0.1f;
        if (tailAlpha >= 0.5f)
        {
            tailAlpha = 0.5f;
        }

        color = new Color(color.r, color.g, color.b, tailAlpha);
	}
}
