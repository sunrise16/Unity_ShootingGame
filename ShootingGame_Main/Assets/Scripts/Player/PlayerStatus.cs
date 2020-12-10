using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private float playerMoveSpeed;          // 플레이어 이동 속도

    private bool isSlowMode;                // 플레이어가 저속 이동 중인지 체크
    private bool isInvincible;              // 플레이어가 무적 상태인지 체크
    private bool isSpriteOff;               // 플레이어가 피탄된 후 리스폰 되기까지의 간격 체크
    private bool isRespawn;                 // 플레이어가 리스폰된 후 조작 가능해지기까지의 간격 체크
    private bool isBlinking;                // 플레이어 스프라이트 이미지 깜빡거리는 연출 체크

    private void Start()
    {
        playerMoveSpeed = 0.0f;

        isSlowMode = false;
        isInvincible = false;
	}

    #region GET, SET

    public float GetPlayerMoveSpeed()
    {
        return playerMoveSpeed;
    }

    public bool GetSlowMove()
    {
        return isSlowMode;
    }

    public bool GetInvincible()
    {
        return isInvincible;
    }

    public bool GetSpriteOff()
    {
        return isSpriteOff;
    }

    public bool GetRespawn()
    {
        return isRespawn;
    }

    public bool GetBlinking()
    {
        return isBlinking;
    }

    public void SetPlayerMoveSpeed(float speed)
    {
        playerMoveSpeed = speed;
    }

    public void SetSlowMode(bool slowMode)
    {
        isSlowMode = slowMode;
    }

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    public void SetSpriteOff(bool spriteOff)
    {
        isSpriteOff = spriteOff;
    }

    public void SetRespawn(bool respawn)
    {
        isRespawn = respawn;
    }

    public void SetBlinking(bool blinking)
    {
        isBlinking = blinking;
    }

    #endregion
}
