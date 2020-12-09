using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private float playerMoveSpeed;

    private bool isSlowMode;
    private bool isInvincible;
    private bool isSpriteOff;
    private bool isRespawn;
    private bool isBlinking;

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
