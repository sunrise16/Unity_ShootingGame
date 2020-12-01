using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private float playerMoveSpeed;
    private bool isInvincible;

    private void Start()
    {
        playerMoveSpeed = 0.0f;
        isInvincible = false;
	}

    #region GET, SET

    public float GetPlayerMoveSpeed()
    {
        return playerMoveSpeed;
    }

    public bool GetInvincible()
    {
        return isInvincible;
    }

    public void SetPlayerMoveSpeed(float speed)
    {
        playerMoveSpeed = speed;
    }

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    #endregion
}
