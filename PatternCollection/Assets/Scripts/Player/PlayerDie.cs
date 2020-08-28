using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    private PlayerMove playerMove;

    public GameObject dieEffect;
    public bool isInvincible;

    void Start()
    {
        playerMove = GameObject.Find("PLAYER").GetComponent<PlayerMove>();
        isInvincible = false;
    }

    public IEnumerator CreateDieEffect()
    {
        if (playerMove.isDamaged == false && isInvincible == false)
        {
            playerMove.isDamaged = true;
            isInvincible = true;
        
            GameObject body = transform.Find("Body").gameObject;
            GameObject effect = Instantiate(dieEffect);
            effect.transform.SetParent(transform.Find("Effect").transform);
            effect.transform.position = transform.position;
            body.SetActive(false);
        
          yield return new WaitForSeconds(1.5f);
        
            Destroy(effect);
            transform.position = new Vector2(0.0f, -5.5f);
            body.SetActive(true);
            StartCoroutine(PlayerReviving());
            StartCoroutine(PlayerBlinking());
            StopCoroutine(CreateDieEffect());
        }
    }

    IEnumerator PlayerReviving()
    {
        float moveSpeed = 2.0f;
        float reviveDelay = 0.0f;

        while (true)
        {
            reviveDelay += 0.1f;
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);

            if (reviveDelay <= 5.0f) break;

            yield return new WaitForEndOfFrame();
        }

        playerMove.isDamaged = false;
        StopCoroutine(PlayerReviving());
    }

    IEnumerator PlayerBlinking()
    {
        int blinkCount = 0;
        Color bodyColor = transform.Find("Body").GetComponent<SpriteRenderer>().color;

        while (true)
        {
            blinkCount++;
            if (bodyColor.a >= 0.8f)
            {
                bodyColor = new Color(bodyColor.r, bodyColor.g, bodyColor.b, 0.2f);
            }
            else if (bodyColor.a <= 0.2f)
            {
                bodyColor = new Color(bodyColor.r, bodyColor.g, bodyColor.b, 0.8f);
            }

            if (blinkCount < 30) break;

            yield return new WaitForSeconds(0.08f);
        }

        blinkCount = 0;
        isInvincible = false;
        StopCoroutine(PlayerBlinking());
    }
}
