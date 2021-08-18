using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerSecondary : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private SpriteRenderer spriteRenderer;
    private Vector3 vector;

    public GameObject fastMoveModePoint;                // 고속 이동 시 보조무기 위치
    public GameObject slowMoveModePoint;                // 저속 이동 시 보조무기 위치

    private void Start()
    {
        playerStatus = GameObject.Find("CHARACTER").transform.Find("Player").GetComponent<PlayerStatus>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vector = Vector3.zero;
    }

    private void Update()
    {
        // 저속 이동 시 보조무기 이동
        if (playerStatus.GetSlowMove().Equals(true))
        {
            transform.position = Vector3.SmoothDamp(transform.position, slowMoveModePoint.transform.position, ref vector, 0.05f);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, fastMoveModePoint.transform.position, ref vector, 0.05f);
        }

        // 보조무기 활성, 비활성
        if (playerStatus.GetSpriteOff().Equals(true))
        {
            // 플레이어 피탄 시 비활성
            spriteRenderer.enabled = false;
        }
        else
        {
            // 플레이어의 현재 파워에 따라 보조무기 출현 (2.0 단위)
            if (GameData.currentPower < 2.0f)
            {
                spriteRenderer.enabled = false;
            }
            else if (GameData.currentPower >= 2.0f && GameData.currentPower < 4.0f)
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
        }
    }
}
