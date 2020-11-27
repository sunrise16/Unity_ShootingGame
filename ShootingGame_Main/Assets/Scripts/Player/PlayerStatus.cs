using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private float moveSpeed;
    private bool isInvincible;

	public void Status()
    {
        moveSpeed = 0.0f;
        isInvincible = false;
	}

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public bool GetInvincible()
    {
        return isInvincible;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }
}
