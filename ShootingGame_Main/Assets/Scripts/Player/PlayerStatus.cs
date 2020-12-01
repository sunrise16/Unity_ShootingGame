using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private float playerMoveSpeed;

    private bool isSlowMode;
    private bool isAutoCollect;
    private bool isInvincible;

    private void Start()
    {
        playerMoveSpeed = 0.0f;

        isSlowMode = false;
        isAutoCollect = false;
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

    public bool GetAutoCollect()
    {
        return isAutoCollect;
    }

    public bool GetInvincible()
    {
        return isInvincible;
    }

    public void SetPlayerMoveSpeed(float speed)
    {
        playerMoveSpeed = speed;
    }

    public void SetSlowMode(bool slowMode)
    {
        isSlowMode = slowMode;
    }

    public void SetAutoCollect(bool autoCollect)
    {
        isAutoCollect = autoCollect;
    }

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    #endregion
}
