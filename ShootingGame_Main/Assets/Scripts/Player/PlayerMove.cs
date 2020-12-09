using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private GameObject grazeCircle;
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    private PlayerStatus playerStatus;

    private Vector2 margin;
    private Vector2 moveSpeedVector;

    private void Start()
    {
        grazeCircle = GameObject.Find("GrazeCircle").gameObject;
        animator = GameObject.Find("Body").GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerStatus = GetComponent<PlayerStatus>();

        margin = new Vector2(0.03f, 0.03f);
        moveSpeedVector = Vector2.zero;
    }
    
    private void Update()
    {
        grazeCircle.transform.position = transform.position;

        if (playerStatus.GetSpriteOff().Equals(true))
        {
            playerStatus.SetSlowMode(false);
            playerStatus.SetPlayerMoveSpeed(0.0f);
            moveSpeedVector = Vector2.zero;
            rigidbody2D.velocity = moveSpeedVector;
        }

        if (playerStatus.GetSpriteOff().Equals(false) && playerStatus.GetRespawn().Equals(false))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerStatus.SetSlowMode(true);
                playerStatus.SetPlayerMoveSpeed(1.5f);
            }
            else
            {
                playerStatus.SetSlowMode(false);
                playerStatus.SetPlayerMoveSpeed(4.0f);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveSpeedVector.x = -playerStatus.GetPlayerMoveSpeed();
                animator.SetTrigger("isLeftMove");
                animator.ResetTrigger("isIdle");
                animator.ResetTrigger("isRightMove");
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                moveSpeedVector.x = playerStatus.GetPlayerMoveSpeed();
                animator.SetTrigger("isRightMove");
                animator.ResetTrigger("isIdle");
                animator.ResetTrigger("isLeftMove");
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveSpeedVector.y = playerStatus.GetPlayerMoveSpeed();
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                moveSpeedVector.y = -playerStatus.GetPlayerMoveSpeed();
            }
            rigidbody2D.velocity = moveSpeedVector;

            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                moveSpeedVector = Vector2.zero;
                animator.SetTrigger("isIdle");
                animator.ResetTrigger("isLeftMove");
                animator.ResetTrigger("isRightMove");
            }
        }

        MoveInScreen();
    }
    
    private void MoveInScreen()
    {
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        position.x = Mathf.Clamp(position.x, 0.0f + margin.x, 1.0f - margin.x);
        position.y = Mathf.Clamp(position.y, 0.0f + margin.y, 1.0f - margin.y);

        transform.position = Camera.main.ViewportToWorldPoint(position);
    }
}
