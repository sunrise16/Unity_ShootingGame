using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private PlayerHitPoint playerHitPoint;
    private GameObject grazeCircle;
    private Animator animator;
    private Vector2 margin;
    private Vector2 moveSpeedVector;

    private float moveSpeed;
    public bool isSlowMoveMode;
    public bool isDamaged;

    void Start()
    {
        playerHitPoint = transform.Find("HitPoint").GetComponent<PlayerHitPoint>();
        grazeCircle = GameObject.Find("GRAZECIRCLE");
        animator = transform.Find("Body").GetComponent<Animator>();
        margin = new Vector2(0.03f, 0.03f);
        moveSpeedVector = Vector2.zero;

        moveSpeed = 3.0f;
        isSlowMoveMode = false;
        isDamaged = false;
    }
    
    void Update ()
    {
        grazeCircle.transform.position = transform.position;

        if (isDamaged == false)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isSlowMoveMode = true;
                StartCoroutine(playerHitPoint.AlphaUp());
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isSlowMoveMode = false;
                StartCoroutine(playerHitPoint.AlphaDown());
            }

            if (isSlowMoveMode == true)
            {
                moveSpeed = 1.2f;
            }
            else
            {
                moveSpeed = 3.0f;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveSpeedVector.x = -moveSpeed;
                animator.SetTrigger("isLeftMove");
                animator.ResetTrigger("isIdle");
                animator.ResetTrigger("isRightMove");
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                moveSpeedVector.x = moveSpeed;
                animator.SetTrigger("isRightMove");
                animator.ResetTrigger("isIdle");
                animator.ResetTrigger("isLeftMove");
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveSpeedVector.y = moveSpeed;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                moveSpeedVector.y = -moveSpeed;
            }
            GetComponent<Rigidbody2D>().velocity = moveSpeedVector;

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
