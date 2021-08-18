using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EffectAlpha : MonoBehaviour
{
    private Transform effectPool;

    private void Start()
    {
        effectPool = GameObject.Find("EFFECT").transform.Find("Effect");
    }

    // 이펙트 알파값 증가 코루틴
    public IEnumerator EffectAlphaUp(GameObject obj, float alphaUpSpeed)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        float alpha = 0.0f;

        while (alpha < 1.0f)
        {
            yield return null;

            alpha += alphaUpSpeed;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            // yield return new WaitForEndOfFrame();
        }
    }

    // 이펙트 알파값 증가 후 감소 처리 실행 코루틴
    public IEnumerator EffectAlphaUp(GameObject obj, float alphaUpSpeed, float alphaDownSpeed, float alphaRemainTime)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        float alpha = 0.0f;

        while (alpha < 1.0f)
        {
            yield return null;

            alpha += alphaUpSpeed;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            // yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(alphaRemainTime);

        StartCoroutine(EffectAlphaDown(obj, alphaDownSpeed));
    }

    // 이펙트 알파값 감소 후 제거 코루틴
    private IEnumerator EffectAlphaDown(GameObject obj, float alphaDownSpeed)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        float alpha = 1.0f;

        while (alpha > 0.0f)
        {
            yield return null;

            alpha -= alphaDownSpeed;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            // yield return new WaitForEndOfFrame();
        }

        ClearEffect(obj);
    }

    // 이펙트 스케일값 증가 코루틴
    public IEnumerator EffectScaleUp(GameObject obj, float scaleUpSpeed, float scaleLimit)
    {
        obj.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);

        while (true)
        {
            yield return null;

            if (obj.transform.localScale.x >= scaleLimit || obj.transform.localScale.y >= scaleLimit)
            {
                obj.transform.localScale = new Vector3(scaleLimit, scaleLimit, 0.0f);
                break;
            }
            else
            {
                obj.transform.localScale = new Vector3(obj.transform.localScale.x + scaleUpSpeed, obj.transform.localScale.y + scaleUpSpeed, 0.0f);
            }

            // yield return new WaitForEndOfFrame();
        }
    }

    // 이펙트 스케일값 감소 후 제거 코루틴
    public IEnumerator EffectScaleDown(GameObject obj, float scaleDownSpeed, float scaleDownTime)
    {
        float delay = 0.0f;

        while (delay < scaleDownTime)
        {
            yield return null;

            delay += Time.deltaTime;
            obj.transform.localScale = new Vector3(obj.transform.localScale.x - scaleDownSpeed, obj.transform.localScale.y - scaleDownSpeed, 0.0f);

            // yield return new WaitForEndOfFrame();
        }

        ClearEffect(obj);
    }

    // 이펙트 제거 함수
    private void ClearEffect(GameObject obj)
    {
        obj.transform.SetParent(effectPool);
        obj.transform.position = new Vector2(0.0f, 0.0f);
        obj.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        obj.transform.localScale = new Vector3(10.0f, 10.0f, 1.0f);

        // 비활성화
        obj.SetActive(false);
    }
}
