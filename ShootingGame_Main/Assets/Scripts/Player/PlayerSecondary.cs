using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerSecondary : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public GameObject fastMoveModePoint;
    public GameObject slowMoveModePoint;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(SecondaryPosition());
        StartCoroutine(SecondaryVisible());
    }

    public IEnumerator SecondaryPosition()
    {
        Vector3 vector = Vector3.zero;

        while (true)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.position = Vector3.SmoothDamp(transform.position, slowMoveModePoint.transform.position, ref vector, 0.05f);
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, fastMoveModePoint.transform.position, ref vector, 0.05f);
            }

            yield return new WaitForEndOfFrame();
        }
	}

    public IEnumerator SecondaryVisible()
    {
        while (true)
        {
            if (GlobalData.currentPower < 2.0f)
            {
                spriteRenderer.enabled = false;
            }
            else if (GlobalData.currentPower >= 2.0f && GlobalData.currentPower < 4.0f)
            {
                if (gameObject.name.Equals("SecondaryPoint1") || gameObject.name.Equals("SecondaryPoint2"))
                {
                    spriteRenderer.enabled = true;
                }
                else
                {
                    spriteRenderer.enabled = false;
                }
            }
            else
            {
                spriteRenderer.enabled = true;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
